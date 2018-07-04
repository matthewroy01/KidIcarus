using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MiscLoadScene : MonoBehaviour
{
	[Header("Scene to switch to")]
	public int sceneNum;

	[Header("Input")]
	public KeyCode keyToPress;

	void Update ()
	{
		if (Input.GetKeyDown(keyToPress))
		{
			SceneManager.LoadScene(sceneNum);
		}
	}
}
