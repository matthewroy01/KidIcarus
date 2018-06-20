using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOrne : MonoBehaviour
{
	[Header("Particle Systems")]
	public ParticleSystem partsFire;
	public ParticleSystem partsSkulls;
	public float skullActivationDistance;
	public float musicActivationDistance;

	[Header("Movement")]
	public float movSpeed;
	private float defaultMovSpeed;
	public float increaseMovSpeedBy;
	public int everyThisAmountOfMeters;
	public bool inRange;
	public bool playMusic;

	private CameraFollow refCamFollow;

	private Transform refPlayer;
	private Rigidbody2D rb;
	private Animator refAnimator;
	private PlayerCollision refPlayerCollision;
	private UtilityMusicManager refMusicManager;

	void Start ()
	{
		rb = GetComponent<Rigidbody2D>();
		refPlayer = GameObject.FindGameObjectWithTag("Player").transform;
		refPlayerCollision = refPlayer.GetComponent<PlayerCollision>();
		refAnimator = GetComponent<Animator>();
		refCamFollow = GameObject.Find("Camera").GetComponent<CameraFollow>();
		refMusicManager = GameObject.FindObjectOfType<UtilityMusicManager>();
		partsFire.Play();
		partsSkulls.Stop();

		defaultMovSpeed = movSpeed;
	}

	void Update ()
	{
		// don't move if the player is in a safe zone
		if (refPlayerCollision.inSafeZone == false)
		{
			if (refPlayerCollision.getCurrentMeters() > 20)
			{
				SeekToPlayer();
			}
		}
		// move away if we're in range and the player is in a safe zone
		else if (playMusic)
		{
			FleeFromPlayer();
		}
		// stop moving once we're far enough away
		else
		{
			rb.velocity = Vector2.zero;
		}

		CheckRange();
		IncreaseMovSpeed();
		CameraShake();
		SkullParticles();
		OrneMusic();
	}

	private void CheckRange()
	{
		if (Vector2.Distance(transform.position, refPlayer.position) < skullActivationDistance)
		{
			inRange = true;
		}
		else
		{
			inRange = false;
		}

		if (Vector2.Distance(transform.position, refPlayer.position) < musicActivationDistance)
		{
			playMusic = true;
		}
		else
		{
			playMusic = false;
		}


		refAnimator.SetBool("inRange", inRange);
	}

	private void CameraShake()
	{
		if (inRange == true)
		{
			refCamFollow.shakeIntensity = Mathf.Lerp(refCamFollow.shakeIntensity, 0.5f, 0.01f);
		}
		else if (playMusic == true)
		{
			refCamFollow.shakeIntensity = Mathf.Lerp(refCamFollow.shakeIntensity, 0.2f, 0.01f);
		}
		else
		{
			refCamFollow.shakeIntensity = Mathf.Lerp(refCamFollow.shakeIntensity, 0.0f, 0.1f);
		}
	}

	private void SkullParticles()
	{
		if (inRange == true && partsSkulls.isPlaying == false)
		{
			partsSkulls.Play();	
		}
		else if (inRange == false)
		{
			partsSkulls.Stop();
		}
	}

	private void OrneMusic()
	{
		if (playMusic)
		{
			refMusicManager.SetMusicStatus(MusicStatus.orneTheme);
		}
		else if (refMusicManager.GetMusicStatus() == MusicStatus.orneTheme)
		{
			refMusicManager.SetMusicStatus(MusicStatus.mainTheme);
		}
	}

	private void SeekToPlayer()
	{
		rb.velocity = (refPlayer.position - transform.position).normalized * movSpeed;
	}

	private void FleeFromPlayer()
	{
		rb.velocity = (refPlayer.position - transform.position).normalized * movSpeed * -1;
	}

	private void IncreaseMovSpeed()
	{
		movSpeed = defaultMovSpeed + (int)(refPlayerCollision.getCurrentMeters() / everyThisAmountOfMeters) * increaseMovSpeedBy;
	}
}