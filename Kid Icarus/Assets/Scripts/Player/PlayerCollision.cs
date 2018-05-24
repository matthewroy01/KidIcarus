using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCollision : MonoBehaviour
{
	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("InstantDeath"))
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
	}
}
