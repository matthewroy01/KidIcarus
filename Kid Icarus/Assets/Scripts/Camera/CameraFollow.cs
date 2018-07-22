using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// script from https://github.com/Telden/GlobalGameJam2018
// taken from Evan Janssen

public class CameraFollow : MonoBehaviour
{

    public Transform cameraTarget;
    [Range(0, 1)]
    public float followSpeed;
    [Range(0, 1)]
    public float turnSpeed;

    [Header("Camera Shake")]
    public bool shouldShake;
    public Vector3 shakePrevRotation;
    [Range(0, 2)]
    public float shakeIntensity;
    public float shakeDuration;

    void Start()
    {
    	transform.position = new Vector3(7.5f, 7.5f, transform.position.z);
    }

    void FixedUpdate()
    {
    	if (transform.position.y < cameraTarget.transform.position.y)
    	{
			transform.position = Vector3.Lerp(new Vector3(7.5f, transform.position.y, transform.position.z), new Vector3(7.5f, cameraTarget.position.y + 1, transform.position.z), followSpeed);
		}

		transform.rotation = Quaternion.Lerp(transform.rotation, cameraTarget.transform.rotation, turnSpeed);

		if (shouldShake)
		{
			Shake();
		}
    }

    // call this function to begin screen shake
    public void StartShake()
    {
        shouldShake = true;
        Invoke("StopShake", shakeDuration);
    }

    // call this function to begin screen shake if you want a custom duration
    public void StartShake(float tmpDur)
    {
        shouldShake = true;
        Invoke("StopShake", tmpDur);
    }

   // call this function to begin screen shake if you want a custom duration and intensity
   public void StartShake(float tmpDur, float intensity)
   {
      shouldShake = true;
      shakeIntensity = intensity;
      Invoke("StopShake", tmpDur);
   }

   // if you want, call this function to stop screen shake prematurely
   public void StopShake()
    {
        shouldShake = false;
      shakeIntensity = 0.0f;
    }

    private void Shake()
    {
        // reset rotation
        transform.rotation = Quaternion.Euler(shakePrevRotation);
        // generate random numbers
        float tmpx = Random.Range(-shakeIntensity, shakeIntensity);
        float tmpy = Random.Range(-shakeIntensity, shakeIntensity);
        float tmpz = Random.Range(-shakeIntensity, shakeIntensity);
        // set new rotation
        transform.rotation = Quaternion.Euler(shakePrevRotation.x + tmpx, shakePrevRotation.y + tmpy, shakePrevRotation.z + tmpz);
    }
}