using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sprintbox
{
	public class MainMenuManager : MonoBehaviour
	{
		public void StartGame()
		{
			SceneManager.LoadScene("Level1");
		}

		public void QuitGame()
		{
			Application.Quit();
		}
	}
}