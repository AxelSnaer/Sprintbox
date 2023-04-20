using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Sprintbox
{
    public class PlayerController : MonoBehaviour
    {
        public Tilemap tilemap;

        [Tooltip("Speed of the player in tiles per second")]
        public float speed = 32.0f;
        
        private Controls _controls;

        private Dictionary<Vector3Int, GameObject> _boxes = new();
        private Dictionary<Vector3Int, GameObject> _goals = new();
        private Vector3Int _queued;

        private bool _moving;

        private void Awake()
        {
            _controls = new();
            _controls.Player.MoveLeft.performed  += _ => StartMove(Vector3Int.left);
            _controls.Player.MoveRight.performed += _ => StartMove(Vector3Int.right);
            _controls.Player.MoveUp.performed    += _ => StartMove(Vector3Int.up);
            _controls.Player.MoveDown.performed  += _ => StartMove(Vector3Int.down);
        }

        private void Start()
        {
            _boxes = FindTiledObjectsWithTag("Box");
            _goals = FindTiledObjectsWithTag("Goal");
        }

        private Dictionary<Vector3Int, GameObject> FindTiledObjectsWithTag(string searchTag)
        {
            var offset = tilemap.transform.position;
            var dict = new Dictionary<Vector3Int, GameObject>();
            
            foreach (var box in GameObject.FindGameObjectsWithTag(searchTag))
            {
                var coord = tilemap.WorldToCell(box.transform.position - offset);
                dict[coord] = box;
            }

            return dict;
        }

        private void Update()
        {
            // Check if the player is stationary and there is an input queued
            // If there is, then submit that input as a movement
            if (!_moving && _queued != Vector3Int.zero)
                StartMove(_queued);
        }

        private void StartMove(Vector3Int dir)
        {
            // If the player is already moving, add the input to an input queue instead
            if (_moving)
            {
                _queued = dir;
                return;
            }

            _queued = Vector3Int.zero;

            StartCoroutine(Move(dir, speed));
        }

        private IEnumerator Move(Vector3Int direction, float speed)
        {
            _moving = true;
            var step = 0.0f;

            while (true)
            {
                var start = transform.position;
                var end = start + direction;

                var offset = tilemap.transform.position;
                var startCoord = tilemap.WorldToCell(start - offset);
                var endCoord = startCoord + direction;

                var boxes = new List<GameObject>();
                while (_boxes.ContainsKey(endCoord))
                {
                    boxes.Add(_boxes[endCoord]);
                    endCoord += direction;
                }
                
                if (!tilemap.HasTile(endCoord))
                    break;
                
                // TODO: Smoothen movement of boxes
                for (int i = boxes.Count - 1; i >= 0; i--)
                {
                    var box = boxes[i];
                    var coord = tilemap.WorldToCell(box.transform.position - offset);
                    _boxes.Remove(coord);
                    _boxes.Add(coord + direction, box);
                    box.transform.position += direction;
                }

                while (step < 1.0f)
                {
                    yield return null;
                    step += Time.deltaTime * speed;
                
                    transform.position = Vector3.Lerp(start, end, step);
                }
                
                if (boxes.Count > 0 && _goals.ContainsKey(endCoord))
                {
                    var box = boxes[^1];
                    _boxes.Remove(endCoord);
                    boxes.Remove(box);
                    
                    Destroy(box);
                }

                step -= 1.0f;
                yield return null;
            }
            
            _moving = false;
        }
        
        private void OnEnable() => _controls.Enable();
        private void OnDisable() => _controls.Disable();
    }
}