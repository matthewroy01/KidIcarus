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
	public bool inRange;
	public bool playMusic;

	[Header("Sound")]
	public AudioSource orneTheme;
	public AudioSource mainTheme;

	private Transform refPlayer;
	private Rigidbody2D rb;
	private Animator refAnimator;

	void Start ()
	{
		rb = GetComponent<Rigidbody2D>();
		refPlayer = GameObject.Find("Player").transform;
		refAnimator = GetComponent<Animator>();
		partsFire.Play();
		partsSkulls.Stop();
		orneTheme.volume = 0.0f;
	}

	void Update ()
	{
		CheckRange();
		SkullParticles();
		OrneMusic();
		SeekToPlayer();
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
}
