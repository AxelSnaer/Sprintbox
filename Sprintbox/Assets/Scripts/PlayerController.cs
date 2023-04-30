using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace Sprintbox
{
    [RequireComponent(typeof(PuzzleObject), typeof(SpriteRenderer))]
    public class PlayerController : MonoBehaviour
    {
        public Tilemap tilemap;

        [Tooltip("Speed of the player in tiles per second")]
        public float speed = 32.0f;
        
        public Sprite upSprite;
        public Sprite downSprite;
        public Sprite leftSprite;
        public Sprite rightSprite;

        public static PlayerController Instance { get; private set; }
        public static Controls Controls;

        private Vector3Int _queued;

        private bool _moving;
        private bool _levelComplete;

        private PuzzleObject _puzzleObject;
        private SpriteRenderer _spriteRenderer;
        
        private void Awake()
        {
            Instance = this;
            _puzzleObject = GetComponent<PuzzleObject>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            
            Controls = new();
            Controls.Player.MoveLeft.performed  += _ => Move(Vector3Int.left);
            Controls.Player.MoveRight.performed += _ => Move(Vector3Int.right);
            Controls.Player.MoveUp.performed    += _ => Move(Vector3Int.up);
            Controls.Player.MoveDown.performed  += _ => Move(Vector3Int.down);

            Controls.Player.LevelRestart.performed += _ => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void Move(Vector3Int dir)
        {
            // If the level is complete, ignore input
            if (_levelComplete)
                return;
            
            // If the player is already moving, add the input to an input queue instead
            if (_moving)
            {
                _queued = dir;
                return;
            }
            
            // Reset queue on move
            _queued = Vector3Int.zero;

            if (dir.x != 0)
                _spriteRenderer.sprite = dir.x > 0 ? rightSprite : leftSprite;
            if (dir.y != 0)
                _spriteRenderer.sprite = dir.y > 0 ? upSprite : downSprite;

            PuzzleManager.Instance.MoveContinuous(_puzzleObject, dir);
        }

        private void Update()
        {
            // Check if the player is stationary and there is an input queued
            // If there is, then submit that input as a movement
            if (!_moving && _queued != Vector3Int.zero)
                Move(_queued);
        }

        private void OnEnable() => Controls.Enable();
        private void OnDisable() => Controls.Disable();
    }
}