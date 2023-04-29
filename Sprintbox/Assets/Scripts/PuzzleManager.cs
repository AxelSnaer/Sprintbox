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

        private Dictionary<Vector3Int, List<PuzzleObject>> _puzzleObjects = new();
        private Tilemap _tilemap;

        private void Awake()
        {
            Instance = this;
            _tilemap = GetComponentInChildren<Tilemap>();
        }

        public void Register(PuzzleObject obj)
        {
            obj.coord = WorldToTile(obj.transform.position);
            _puzzleObjects[obj.coord] ??= new List<PuzzleObject>();
            _puzzleObjects[obj.coord].Add(obj);
        }

        public void Move(PuzzleObject obj, Vector3Int direction)
        {
            var objCoord = _puzzleObjects.First(kv => kv.Value.Contains(obj)).Key;
            var endCoord = objCoord + direction;

            if (CanLandOn(endCoord))
            {
                if (_puzzleObjects.ContainsKey(endCoord))
                    _puzzleObjects[endCoord].ForEach(o => o.LandOn?.Invoke(objCoord));
                
                obj.transform.DOMove(endCoord, 1.0f / objectSpeed);
                _puzzleObjects[objCoord].Remove(obj);
                _puzzleObjects[endCoord] ??= new List<PuzzleObject>();
                _puzzleObjects[endCoord].Add(obj);
            }
        }

        public bool CanLandOn(Vector3Int coord)
        {
            if (_puzzleObjects.ContainsKey(coord))
                return _puzzleObjects[coord].All(obj => obj.CanLandOn?.Invoke(coord) ?? true);

            if (!_tilemap.HasTile(coord))
                return false;
            
            return true;
        }

        private Vector3Int WorldToTile(Vector3 pos)
        {
            var offset = _tilemap.transform.position;
            return _tilemap.WorldToCell(pos - offset);
        }
    }
}