using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMonitorReaper : MonoBehaviour
{
	private EnemyReaper refEnemyReaper;
	private Enemy refEnemy;
	private bool origFacingRight;
	private UtilityAudioManager refAudioManager;

	[Header("Panic sound")]
	public Sound panic;

	void Start ()
	{
		// grab references
		refEnemyReaper = transform.parent.GetComponent<EnemyReaper>();
		refEnemy = transform.parent.GetComponent<Enemy>();
		origFacingRight = refEnemyReaper.facingRight;
		refAudioManager = GameObject.FindObjectOfType<UtilityAudioManager>();

		// remove parent to prevent weird translation side effects
		transform.parent = null;
	}

	void Update ()
	{
		// destroy if the reaper changes directions
		if (refEnemyReaper.facingRight != origFacingRight)
		{
			Destroy(gameObject);
		}

		// destroy if the reaper is panicked
		if (refEnemyReaper.isPanicked == true)
		{
			Destroy(gameObject);
		}

		// destroy if the reaper is dead
		if (refEnemy.isDead == true)
		{
			Destroy(gameObject);
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			// but not that Panic...
			refAudioManager.PlaySound(panic.clip, panic.volume, true);
			refEnemyReaper.Panic();
		}
	}
}