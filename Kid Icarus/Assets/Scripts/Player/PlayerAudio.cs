using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
	private UtilityAudioManager refAudioManager;

	[Header("Audio clips")]
	public Sound jump;
	public Sound shoot;
	public Sound flap;
	public Sound recharge;
	public Sound heart;
	public Sound hurt;
	public Sound death;

	void Start()
	{
		refAudioManager = GameObject.FindObjectOfType<UtilityAudioManager>();
	}

	public void PlayJump()
	{
		refAudioManager.PlaySound(jump.clip, jump.volume, true);
	}
		
	public void PlayShoot()
	{
		refAudioManager.PlaySound(shoot.clip, shoot.volume, true);
	}	

	public void PlayFlap(int jumps)
	{
		refAudioManager.PlaySound(flap.clip, flap.volume, 0.9f + (0.05f * jumps));
	}	

	public void PlayRecharge()
	{
		refAudioManager.PlaySound(recharge.clip, recharge.volume, false);
	}

	public void PlayHeart()
	{
		refAudioManager.PlaySound(heart.clip, heart.volume, false);
	}

	public void PlayHurt()
	{
		refAudioManager.PlaySound(hurt.clip, hurt.volume, true);
	}

	public void PlayDead()
	{
		refAudioManager.PlaySound(death.clip, death.volume, false);
	}
}

[System.Serializable]
public struct Sound
{
	public AudioClip clip;
	public float volume;
}