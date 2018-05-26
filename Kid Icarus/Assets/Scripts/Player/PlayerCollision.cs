using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCollision : MonoBehaviour
{
	[Header("Score")]
	public int hearts;

	private PlayerAudio refPlayerAudio;

	void Start()
	{
		refPlayerAudio = GetComponent<PlayerAudio>();
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("InstantDeath"))
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}

		if (other.CompareTag("Pickup"))
		{
			if (other.name == "Heart1(Clone)")
			{
				refPlayerAudio.PlayHeart();
				hearts += 1;
			}
			else if (other.name == "Heart5(Clone)")
			{
				refPlayerAudio.PlayHeart();
				hearts += 5;
			}
			else if (other.name == "Heart10(Clone)")
			{
				refPlayerAudio.PlayHeart();
				hearts += 10;
			}

			Destroy(other.gameObject);
		}
	}
}
