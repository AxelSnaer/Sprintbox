using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Sprintbox
{
    public class PuzzleManager : MonoBehaviour
    {
        public static PuzzleManager Instance { get; private set; }

        public float objectSpeed = 16.0f;

        public Action OnWin;
        
        private Dictionary<Vector3Int, List<PuzzleObject>> _puzzleObjects = new();
        private Tilemap _tilemap;

        private List<PushableBox> _boxes = new();
        
        private void Awake()
        {
            Instance = this;
            _tilemap = GetComponentInChildren<Tilemap>();
        }

        public void Register(PuzzleObject obj)
        {
            obj.coord = WorldToTile(obj.transform.position);
            
            if (!_puzzleObjects.ContainsKey(obj.coord))
                _puzzleObjects.Add(obj.coord, new());
            
            _puzzleObjects[obj.coord].Add(obj);
            
            if (obj is PushableBox box)
                _boxes.Add(box);
        }

        public void Move(PuzzleObject obj, Vector3Int direction, Action callback = null)
        {
            var objCoord = _puzzleObjects.First(kv => kv.Value.Contains(obj)).Key;
            var endCoord = objCoord + direction;

            if (obj.inTransit || !CanLandOn(direction, endCoord))
                return;

            if (_puzzleObjects.ContainsKey(endCoord))
            {
                foreach (var item in _puzzleObjects[endCoord].ToArray())
                    item.LandOn(direction);
            }

            obj.inTransit = true;
            obj.transform.DOMove(TileToWorld(endCoord), 1.0f / objectSpeed).OnComplete(() =>
            {
                obj.coord = endCoord;
                _puzzleObjects[objCoord].Remove(obj);
                
                if (!_puzzleObjects.ContainsKey(endCoord))
                    _puzzleObjects.Add(endCoord, new());

                _puzzleObjects[endCoord].Add(obj);
                obj.inTransit = false;

                foreach (var item in _puzzleObjects[endCoord].ToArray())
                    if (item != obj)
                        item.LandedOn(obj);
                
                callback?.Invoke();
            });
        }

        public void DestroyPuzzleObject(PuzzleObject obj)
        {
            _puzzleObjects[obj.coord].Remove(obj);

            if (obj is PushableBox box)
            {
                _boxes.Remove(box);
                if (_boxes.Count == 0)
                    OnWin?.Invoke();
            }
            
            Destroy(obj.gameObject);
        }

        public void MoveContinuous(PuzzleObject obj, Vector3Int direction)
        {
            Move(obj, direction, () => MoveContinuous(obj, direction));
        }

        public bool CanLandOn(Vector3Int fromDir, Vector3Int coord)
        {
            if (!_tilemap.HasTile(coord))
                return false;
            
            if (_puzzleObjects.ContainsKey(coord))
                return _puzzleObjects[coord].All(obj => obj.CanLandOn(fromDir));
            
            return true;
        }

        private Vector3Int WorldToTile(Vector3 pos)
        {
            var offset = _tilemap.transform.position;
            return _tilemap.WorldToCell(pos - offset);
        }
        
        private Vector3 TileToWorld(Vector3Int coord)
        {
            var offset = _tilemap.transform.position;
            return _tilemap.CellToWorld(coord) + offset;
        }
    }
}