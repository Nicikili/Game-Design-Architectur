using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
	public void PlayGame()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}

	public void QuitGame()
	{
		Debug.Log("Quti!");
		Application.Quit();
	}
}
