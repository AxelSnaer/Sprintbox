using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace Sprintbox
{
	public class LevelUIManager : MonoBehaviour
	{
		public Image winScreen;
		public Image pauseScreen;
		
		private void Start()
		{
			PuzzleManager.Instance.OnWin += () =>
			{
				winScreen.gameObject.SetActive(true);
				winScreen.DOColor(Color.white, 0.5f);
			};

			PlayerController.Controls.Player.ToggleMenu.performed += _ => TogglePauseMenu();
		}
		
		public void TogglePauseMenu()
		{
			if (pauseScreen.gameObject.activeSelf)
				ClosePauseMenu();
			else
				OpenPauseMenu();
		}

		public void OpenPauseMenu()
		{
			pauseScreen.gameObject.SetActive(true);
		}
		
		public void ClosePauseMenu()
		{
			pauseScreen.gameObject.SetActive(false);
		}

		public void ToMenu()
		{
			SceneManager.LoadScene("MainMenu");
		}

		public void ToNextLevel()
		{
			var idx = SceneManager.GetActiveScene().buildIndex;
			if (IsSceneInProject($"Level{idx + 1}"))
				SceneManager.LoadScene($"Level{idx + 1}");
			else
				ToMenu();
		}
		
		// Code by swingingtom taken from https://forum.unity.com/threads/scene-management-verify-if-a-scene-exists.910934/
		public static bool IsSceneInProject(string name)
		{
			return EditorBuildSettings.scenes.Any(scene => scene.enabled && scene.path.Contains($"/{name}.unity"));
		}
	}
}