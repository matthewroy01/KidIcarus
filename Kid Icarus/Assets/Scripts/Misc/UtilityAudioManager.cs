using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this code taken from Battle Beetles
// written by Matthew Roy

public class UtilityAudioManager : MonoBehaviour
{
	public AudioSource[] audioSources;

	void Start()
	{
		for (int i = 0; i < audioSources.Length; i++)
		{
			gameObject.AddComponent<AudioSource>();
		}

		// populating array found here:
		// http://answers.unity3d.com/questions/795797/gather-audiosources-in-an-array.html
		audioSources = Object.FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
	}

	public void PlaySound (AudioClip newAudio, float volume)
	{
		for (int i = 0; i < audioSources.Length; i++)
		{
			if (audioSources[i].isPlaying == false)
			{
				audioSources[i].clip = newAudio;
				audioSources[i].pitch = Random.Range(0.95f, 1.05f);
				audioSources[i].volume = volume;
				audioSources[i].Play();
				return;
			}
		}
	}

	public void PlaySound (AudioClip newAudio, float volume, bool doRandomPitch)
	{
		for (int i = 0; i < audioSources.Length; i++)
		{
			if (audioSources[i].isPlaying == false)
			{
				audioSources[i].clip = newAudio;
				if (doRandomPitch)
				{
					audioSources[i].pitch = Random.Range(0.95f, 1.05f);
				}
				else
				{
					audioSources[i].pitch = 1.0f;
				}
				audioSources[i].volume = volume;
				audioSources[i].Play();
				return;
			}
		}
	}

    public void PlaySound(AudioClip newAudio, float volume, float pitch)
    {
        for (int i = 0; i < audioSources.Length; i++)
        {
            if (audioSources[i].isPlaying == false)
            {
                audioSources[i].clip = newAudio;
                audioSources[i].pitch = pitch;
                audioSources[i].volume = volume;
                audioSources[i].Play();
                return;
            }
        }
    }
}
