using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiscFollowMouse : MonoBehaviour
{
	void Update ()
   {
      Vector3 mouseVector = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane + 9.0f);
      transform.position = Camera.main.ScreenToWorldPoint(mouseVector);
	}
}