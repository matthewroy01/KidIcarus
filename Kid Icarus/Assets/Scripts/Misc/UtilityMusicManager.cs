using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("Main theme options")]
    public Text musicText;
    private bool fadeOut = false;
    public MusicOption[] options;
    private int choice;

    private MiscSoundWobble refSoundWobble;

    private bool playedDeath = false;
    private PlayerMovement refPlayerMovement;

	void Start ()
	{
        choice = Random.Range(0, options.Length);
        main.clip = options[choice].clip;

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

        refSoundWobble = GetComponent<MiscSoundWobble>();
        refPlayerMovement = GameObject.FindObjectOfType<PlayerMovement>();

        Invoke("SetFade", 6.0f);
	}

	void Update ()
	{
        SelectMusic();
		UpdateMusic();

        if (fadeOut == true)
        {
            musicText.color = Vector4.Lerp(musicText.color, new Vector4(musicText.color.r, musicText.color.g, musicText.color.b, 0.0f), 0.1f);
        }
        else
        {
            musicText.color = Vector4.Lerp(musicText.color, new Vector4(musicText.color.r, musicText.color.g, musicText.color.b, 0.5f), 0.5f);
        }
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
            if (death.isPlaying == false && playedDeath == false)
				{
					death.Play();
				}
                playedDeath = true;
				orne.volume = Mathf.Lerp(orne.volume, 0.0f, 0.1f);
				main.volume = Mathf.Lerp(main.volume, 0.0f, 0.1f);
				shop.volume = Mathf.Lerp(shop.volume, 0.0f, 0.1f);
				reap.volume = Mathf.Lerp(reap.volume, 0.0f, 0.1f);
				break;
			}
		}
	}

    private void SelectMusic()
    {
        musicText.text = options[choice].description;
        if (refPlayerMovement.gameStarted == false)
        {
            // scroll left
            if (Input.GetKeyDown(KeyCode.A))
            {
                // if we're at the end, circle around
                if (choice == 0)
                {
                    choice = options.Length - 1;
                }
                else
                {
                    choice--;
                }
                main.clip = options[choice].clip;
                main.Play();

                fadeOut = false;
                CancelInvoke("SetFade");
                Invoke("SetFade", 3.0f);
            }
            // scroll right
            else if (Input.GetKeyDown(KeyCode.D))
            {
                // if we're at the end, circle around
                if (choice == options.Length - 1)
                {
                    choice = 0;
                }
                else
                {
                    choice++;
                }
                main.clip = options[choice].clip;
                main.Play();

                fadeOut = false;
                CancelInvoke("SetFade");
                Invoke("SetFade", 3.0f);
            }
        }
    }

    private void SetFade()
    {
        fadeOut = true;
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

    public void SetWobble(bool set)
    {
        refSoundWobble.shouldWobble = set;
    }
}

public enum MusicStatus { orneTheme, mainTheme, shopTheme, reaperTheme, death};

[System.Serializable]
public struct MusicOption
{
    public AudioClip clip;
    [TextArea(15, 20)]
    public string description;
}