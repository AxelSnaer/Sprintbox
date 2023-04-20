using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Sprintbox
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        public Tilemap tilemap;

        [Space]

        [Tooltip("Speed of the player in tiles per second")]
        public float speed = 64.0f;
        
        private Controls _controls;
        private Rigidbody2D _rigidbody;

        private Vector3 _targetPos;
        private Vector3Int _moveDir;
        private Dictionary<Vector3Int, GameObject> _boxes = new();

        private Vector3Int _queued;

        private bool _moving;

        private void Awake()
        {
            _controls = new();
            _controls.Player.MoveLeft.performed  += _ => StartMove(Vector3Int.left);
            _controls.Player.MoveRight.performed += _ => StartMove(Vector3Int.right);
            _controls.Player.MoveUp.performed    += _ => StartMove(Vector3Int.up);
            _controls.Player.MoveDown.performed  += _ => StartMove(Vector3Int.down);

            _rigidbody = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            var offset = tilemap.transform.position;
            foreach (var box in GameObject.FindGameObjectsWithTag("Box"))
            {
                var coord = tilemap.WorldToCell(box.transform.position - offset);
                _boxes[coord] = box;
            }
        }

        private void Update()
        {
            if (!_moving && _queued != Vector3Int.zero)
                StartMove(_queued);
        }

        private void StartMove(Vector3Int dir)
        {
            if (_moving)
            {
                _queued = dir;
                return;
            }

            _queued = Vector3Int.zero;

            StartCoroutine(Move(dir, 32.0f));
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

                step -= 1.0f;
                yield return null;
            }
            
            _moving = false;
        }
        
        private void OnEnable() => _controls.Enable();
        private void OnDisable() => _controls.Disable();
    }
}