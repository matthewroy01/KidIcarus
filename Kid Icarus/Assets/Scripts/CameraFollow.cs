using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// script from https://github.com/Telden/GlobalGameJam2018
// taken from Evan Janssen

public class CameraFollow : MonoBehaviour
{

    public GameObject cameraTarget;
    [Range(0, 1)]
    public float followSpeed;
    [Range(0, 1)]
    public float turnSpeed;

    //exists so that we can switch between the follow speed and the boost speed
    private float currentFollowSpeed = 0;

    [Header("Camera Shake")]
    public bool shouldShake;
    public Vector3 shakePrevRotation;
    [Range(0, 2)]
    public float shakeIntensity;
    public float shakeDuration;

    [Header("WallClip")]
    public Transform player;
    public LayerMask clip;

   private void Awake()
   {
      currentFollowSpeed = followSpeed;
   }

   // Update is called once per frame
   void FixedUpdate()
    {
		transform.position = Vector3.Lerp(new Vector3(7.5f, transform.position.y, transform.position.z), new Vector3(7.5f, cameraTarget.transform.position.y + 1, transform.position.z), currentFollowSpeed);
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

    // if you want, call this function to stop screen shake prematurely
    public void StopShake()
    {
        shouldShake = false;
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

    Vector3 CalcMidpoint(Vector3 one, Vector3 two)
    {
        float tmpX, tmpY, tmpZ;
        tmpX = (one.x + two.x) / 2;
        tmpY = (one.y + two.y) / 2;
        tmpZ = (one.z + two.z) / 2;

        return new Vector3(tmpX, tmpY, tmpZ);
    }

   public void EndBoosting()
   {
      currentFollowSpeed = followSpeed;
   }
}