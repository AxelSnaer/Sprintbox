using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Sprintbox
{
	public class LevelUIManager : MonoBehaviour
	{
		public Image winScreen;

		private void Start()
		{
			PuzzleManager.Instance.OnWin += () =>
			{
				winScreen.gameObject.SetActive(true);
				winScreen.DOColor(Color.white, 0.5f);
			};
		}
	}
}