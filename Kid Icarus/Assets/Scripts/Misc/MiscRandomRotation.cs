using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiscRandomRotation : MonoBehaviour
{
    public Vector3 rotationAxes;

    void Start()
    {
        transform.eulerAngles = rotationAxes * Random.Range(0, 360);
    }

    void Update()
    {
        
    }
}
