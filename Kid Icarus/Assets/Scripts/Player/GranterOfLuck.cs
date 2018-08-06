using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GranterOfLuck : MonoBehaviour
{
	[Header("Chance")]
	public int oneInThis;
	public int startingM;
	public int increaseMBy;
	public int targetM;
	public float destroyAfterTime;

   [Header("Komayto")]
   public GameObject komaytoPrefab;
   public Sound komaytoSound;

	private bool alreadyDecided = false;
	private bool fadeOut = false;

	[Header("Message")]
	public Text goddessText;

	private PlayerCollision refPlayerCollision;
   private UtilityAudioManager refAudioManager;

	void Start ()
	{
		refPlayerCollision = transform.parent.GetComponent<PlayerCollision>();
      refAudioManager = GameObject.FindObjectOfType<UtilityAudioManager>();
		startingM = refPlayerCollision.getCurrentMeters();
		targetM = startingM + increaseMBy;

		goddessText = GameObject.Find("textGoddess").GetComponent<Text>();;
	}

	void Update ()
	{
		if (refPlayerCollision.getCurrentMeters() >= targetM && alreadyDecided == false)
		{
			if (Random.Range(0, oneInThis) == 0)
			{
				goddessText.canvasRenderer.SetAlpha(0);//= new Color(goddessText.color.r, goddessText.color.g, goddessText.color.b, 0);
				alreadyDecided = true;
				DoRandomEffect();
			}
		}

		if (alreadyDecided == true && fadeOut == false)
		{
			goddessText.CrossFadeAlpha(1, 1.0f, false);
		}

		if (fadeOut == true)
		{
			goddessText.CrossFadeAlpha(0, 1.0f, false);
		}
	}

	private void DoRandomEffect()
	{
		int tmp = Random.Range(0, 4);

		switch(tmp)
		{
			case 0:
			{
				goddessText.text = "The next item you purchase in the\nshop will be half off...";
				refPlayerCollision.sale = 2;
				break;
			}
			case 1:
			{
				goddessText.text = "Health restoration for 10 seconds...";
				refPlayerCollision.SetConstantHealthRegen(true, 10);
				break;
			}
			case 2:
			{
				goddessText.text = "I've sent the Orne down 50m\nfor you, Pit...";
				EnemyOrne tmpOrne = GameObject.Find("Orne").GetComponent<EnemyOrne>();
            tmpOrne.transform.position = new Vector2(tmpOrne.transform.position.x, tmpOrne.transform.position.y - 50.0f);
				tmpOrne.SetDefaultMovSpeed(tmpOrne.GetDefaultMovSpeed() - tmpOrne.increaseMovSpeedBy * 2);
				break;
			}
         case 3:
         {
            goddessText.text = "Komaytos are attacking!\nOnly your hammer can defeat them!";
            Instantiate(komaytoPrefab, Vector2.zero, transform.rotation);
            refAudioManager.PlaySound(komaytoSound.clip, komaytoSound.volume);
            break;
         }
         default:
			{
				goddessText.text = "Sorry Pit, the goddess screwed up her switch statement somehow.";
				break;
			}
		}

		Invoke("FadeOut", destroyAfterTime - 1.5f);
		Invoke("Dest", destroyAfterTime);
	}

	private void FadeOut()
	{
		fadeOut = true;
	}

	private void Dest()
	{
		goddessText.text = "";
		Destroy(gameObject);
	}
}