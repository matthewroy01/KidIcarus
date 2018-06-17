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

	[Header("Sound")]
	public AudioSource orneTheme;
	public AudioSource mainTheme;

	private CameraFollow refCamFollow;

	private Transform refPlayer;
	private Rigidbody2D rb;
	private Animator refAnimator;
	private PlayerCollision refPlayerCollision;

	void Start ()
	{
		rb = GetComponent<Rigidbody2D>();
		refPlayer = GameObject.FindGameObjectWithTag("Player").transform;
		refPlayerCollision = refPlayer.GetComponent<PlayerCollision>();
		refAnimator = GetComponent<Animator>();
		refCamFollow = GameObject.Find("Camera").GetComponent<CameraFollow>();
		partsFire.Play();
		partsSkulls.Stop();
		orneTheme.volume = 0.0f;

		defaultMovSpeed = movSpeed;
	}

	void Update ()
	{
		CheckRange();
		SkullParticles();
		OrneMusic();
		SeekToPlayer();
		CameraShake();
		IncreaseMovSpeed();
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
			orneTheme.volume = Mathf.Lerp(orneTheme.volume, 1.0f, 0.1f);
			mainTheme.volume = Mathf.Lerp(mainTheme.volume, 0.0f, 0.1f);
		}
		else
		{
			orneTheme.volume = Mathf.Lerp(orneTheme.volume, 0.0f, 0.1f);
			mainTheme.volume = Mathf.Lerp(mainTheme.volume, 1.0f, 0.1f);
		}
	}

	private void SeekToPlayer()
	{
		rb.velocity = (refPlayer.position - transform.position).normalized * movSpeed;
	}

	private void IncreaseMovSpeed()
	{
		movSpeed = defaultMovSpeed + (int)(refPlayerCollision.getCurrentMeters() / everyThisAmountOfMeters) * increaseMovSpeedBy;
	}
}