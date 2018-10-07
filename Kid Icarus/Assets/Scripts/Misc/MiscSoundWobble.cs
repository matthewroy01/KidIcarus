using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiscSoundWobble : MonoBehaviour
{
    [Header("Pitch wobble values")]
    public bool shouldWobble;
    [Range(-3.0f, 3.0f)]
    public float high;
    [Range(-3.0f, 3.0f)]
    public float low;
    [Range(0.0f, 1.0f)]
    public float lerpSpeed;

    private float switchThreshold = 0.1f;
    private bool goingUp = false;

    [Header("Affect this audio source")]
    public AudioSource source;
	
    void Update ()
    {
        if (shouldWobble == true)
        {
            if (goingUp == true)
            {
                if (source.pitch < high - switchThreshold)
                {
                    source.pitch = Mathf.Lerp(source.pitch, high, lerpSpeed);
                }
                else
                {
                    goingUp = false;
                }
            }

            if (goingUp == false)
            {
                if (source.pitch > low + switchThreshold)
                {
                    source.pitch = Mathf.Lerp(source.pitch, low, lerpSpeed);
                }
                else
                {
                    goingUp = true;
                }
            }
        }
        else
        {
            source.pitch = Mathf.Lerp(source.pitch, 1.0f, lerpSpeed);
            goingUp = false;
        }
    }
}
