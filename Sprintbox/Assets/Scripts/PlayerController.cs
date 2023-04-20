using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Sprintbox
{
    public class PlayerController : MonoBehaviour
    {
        public Tilemap tilemap;
        
        private Controls _controls;

        private void Awake()
        {
            _controls = new();
            _controls.Player.MoveLeft.performed  += _ => Move(Vector3Int.left);
            _controls.Player.MoveRight.performed += _ => Move(Vector3Int.right);
            _controls.Player.MoveUp.performed    += _ => Move(Vector3Int.up);
            _controls.Player.MoveDown.performed  += _ => Move(Vector3Int.down);
        }

        private void Move(Vector3Int dir)
        {
            var offset = tilemap.transform.position;
            
            var currentCell = tilemap.WorldToCell(transform.position - offset);
            var finalCoord = currentCell;

            while (tilemap.HasTile(finalCoord + dir))
                finalCoord += dir;

            transform.position = tilemap.CellToWorld(finalCoord) + offset;
        }

        private void OnEnable() => _controls.Enable();
        private void OnDisable() => _controls.Disable();
    }
}