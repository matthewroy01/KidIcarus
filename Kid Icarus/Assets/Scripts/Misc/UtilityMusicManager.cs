using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilityMusicManager : MonoBehaviour
{
	[Header("Music")]
	public AudioSource orne;
	public AudioSource main;
	public AudioSource shop;
	public AudioSource death;
	public AudioSource reap;

	[Header("Status")]
	[SerializeField]
	private MusicStatus musicStatus;

	void Start ()
	{
		// set the status to playing the main theme by default
		musicStatus = MusicStatus.mainTheme;

		// start playing all music
		orne.Play();
		main.Play();
		shop.Play();
		reap.Play();

		death.Stop();

		// set volumes to 0
		orne.volume = 0.0f;
		shop.volume = 0.0f;
		reap.volume = 0.0f;
	}

	void Update ()
	{
		UpdateMusic();
	}

	private void UpdateMusic()
	{
		// lerp volumes according to the status
		switch (musicStatus)
		{
			case MusicStatus.orneTheme:
			{
				orne.volume = Mathf.Lerp(orne.volume, 1.0f, 0.1f);
				main.volume = Mathf.Lerp(main.volume, 0.0f, 0.1f);
				shop.volume = Mathf.Lerp(shop.volume, 0.0f, 0.1f);
				reap.volume = Mathf.Lerp(reap.volume, 0.0f, 0.1f);
				break;
			}
			case MusicStatus.mainTheme:
			{
				orne.volume = Mathf.Lerp(orne.volume, 0.0f, 0.1f);
				main.volume = Mathf.Lerp(main.volume, 1.0f, 0.1f);
				shop.volume = Mathf.Lerp(shop.volume, 0.0f, 0.1f);
				reap.volume = Mathf.Lerp(reap.volume, 0.0f, 0.1f);
				break;
			}
			case MusicStatus.shopTheme:
			{
				orne.volume = Mathf.Lerp(orne.volume, 0.0f, 0.1f);
				main.volume = Mathf.Lerp(main.volume, 0.0f, 0.1f);
				shop.volume = Mathf.Lerp(shop.volume, 0.8f, 0.1f);
				reap.volume = Mathf.Lerp(reap.volume, 0.0f, 0.1f);
				break;
			}
			case MusicStatus.reaperTheme:
			{
				orne.volume = Mathf.Lerp(orne.volume, 0.0f, 0.1f);
				main.volume = Mathf.Lerp(main.volume, 0.0f, 0.1f);
				shop.volume = Mathf.Lerp(shop.volume, 0.0f, 0.1f);
				reap.volume = Mathf.Lerp(reap.volume, 1.0f, 0.1f);
				break;
			}
			case MusicStatus.death:
			{
				if (death.isPlaying == false)
				{
					death.Play();
				}
				orne.volume = Mathf.Lerp(orne.volume, 0.0f, 0.1f);
				main.volume = Mathf.Lerp(main.volume, 0.0f, 0.1f);
				shop.volume = Mathf.Lerp(shop.volume, 0.0f, 0.1f);
				reap.volume = Mathf.Lerp(reap.volume, 0.0f, 0.1f);
				break;
			}
		}
	}

	public void SetMusicStatus(MusicStatus newStatus)
	{
		// special case for if we've died so that music can't be updated accidentally while we are dead
		if (musicStatus == MusicStatus.death)
		{
			return;
		}

		// update the status
		musicStatus = newStatus;
		// Debug.Log("Changing sound to " + newStatus);
	}

	public MusicStatus GetMusicStatus()
	{
		return musicStatus;
	}
}

public enum MusicStatus { orneTheme, mainTheme, shopTheme, reaperTheme, death};