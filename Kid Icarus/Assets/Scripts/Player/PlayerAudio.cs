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
	public Sound getHealth;
	public Sound cursed;
	public Sound hammerSwing;
	public Sound hammerHit;
   public Sound arrowRecharge;

	void Start()
	{
		refAudioManager = GameObject.FindObjectOfType<UtilityAudioManager>();
	}

	public void PlayJump()
	{
		refAudioManager.PlaySound(jump.clip, jump.volume, true);
	}
		
	public void PlayShoot(float volumeMultiplier, float pitch)
	{
		refAudioManager.PlaySound(shoot.clip, shoot.volume * volumeMultiplier, pitch);
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

	public void PlayGetHealth()
	{
		refAudioManager.PlaySound(getHealth.clip, getHealth.volume, false);
	}

	public void PlayCursed(float pitch)
	{
		refAudioManager.PlaySound(cursed.clip, cursed.volume, pitch);
	}

	public void PlayHammerSwing()
	{
		refAudioManager.PlaySound(hammerSwing.clip, hammerSwing.volume, true);
	}

	public void PlayHammerHit()
	{
		refAudioManager.PlaySound(hammerHit.clip, hammerHit.volume, true);
	}

   public void PlayArrowRecharge()
   {
      refAudioManager.PlaySound(arrowRecharge.clip, arrowRecharge.volume, false);
   }
}

[System.Serializable]
public struct Sound
{
	public AudioClip clip;
	public float volume;
}