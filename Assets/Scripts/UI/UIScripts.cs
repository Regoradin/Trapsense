using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIScripts : MonoBehaviour
{
    public void LoadScene(int scene_index)
	{
		SceneManager.LoadScene(scene_index);
	}

	public void Quit()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}

	public void SetHat(int value)
	{
		PlayerPrefs.SetInt("Hat", value);
	}


}
