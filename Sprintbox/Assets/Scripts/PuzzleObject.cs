using System;
using DG.Tweening;
using UnityEngine;

namespace Sprintbox
{
    public class PuzzleObject : MonoBehaviour
    {
        public Vector3Int coord;
        public bool inTransit;
        
        private void Start()
        {
            PuzzleManager.Instance.Register(this);
        }

        public virtual bool CanLandOn(Vector3Int fromDir)
        {
            return true;
        }

        public virtual void LandOn(Vector3Int fromDir) { }
        public virtual void LandedOn(PuzzleObject other) { }
    }
}