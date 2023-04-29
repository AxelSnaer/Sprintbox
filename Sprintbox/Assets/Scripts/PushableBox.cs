using UnityEngine;

namespace Sprintbox
{
    public class PushableBox : PuzzleObject
    {
        public override bool CanLandOn(Vector3Int fromDir)
        {
            return PuzzleManager.Instance.CanLandOn(fromDir, coord + fromDir);
        }

        public override void LandOn(Vector3Int fromDir)
        {
            PuzzleManager.Instance.Move(this, fromDir);
        }
    }
}
