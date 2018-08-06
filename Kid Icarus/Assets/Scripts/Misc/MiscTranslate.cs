using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiscTranslate : MonoBehaviour
{
   public Vector2 translateEachFrameBy;

	void FixedUpdate ()
   {
      transform.Translate(translateEachFrameBy);
	}
}
