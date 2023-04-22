using UnityEngine;

namespace Sprintbox
{
	public class LevelUIManager : MonoBehaviour
	{
		public GameObject winScreen;

		private void Start()
		{
			PlayerController.Instance.OnWin += () => winScreen.SetActive(true);
		}
	}
}