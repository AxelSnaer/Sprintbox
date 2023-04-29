using System;
using DG.Tweening;
using UnityEngine;

namespace Sprintbox
{
    public class PuzzleObject : MonoBehaviour
    {
        public Vector3Int coord;

        public Func<Vector3Int, bool> CanLandOn;
        public Action<Vector3Int> LandOn;

        private void Start()
        {
            PuzzleManager.Instance.Register(this);
        }
    }
}