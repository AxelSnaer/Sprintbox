using UnityEngine;

namespace Sprintbox
{
	public class Goal : PuzzleObject
	{
		public override void LandedOn(PuzzleObject other)
		{
			if (other is PushableBox box)
				PuzzleManager.Instance.DestroyPuzzleObject(other);
		}
	}
}