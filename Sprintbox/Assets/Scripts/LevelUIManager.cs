using UnityEngine;

namespace Sprintbox
{
	public class LevelUIManager : MonoBehaviour
	{
		public GameObject winScreen;

		private void Start()
		{
			PlayerController.OnWin += () => winScreen.SetActive(true);
		}
	}
}