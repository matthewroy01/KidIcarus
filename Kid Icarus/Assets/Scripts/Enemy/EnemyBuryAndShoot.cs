using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBuryAndShoot : MonoBehaviour
{
   [Header("Checking for ground")]
   public uint checkDist;
   private Vector2 finalPos;
   public LayerMask groundLayer;
   public bool allSet = false;
   public Vector2 finalPosOffset;

   [Header("Shooting")]
   public GameObject projPrefab;
   public Vector2 projOrigin;
   public float projSpeed;

   [Header("Getting stunned")]
   public float stunDist;
   public float stunDuration;

	void Start ()
   {
      CheckBelow();
	}

   private void CheckBelow()
   {
      for (uint i = 0; i < checkDist; ++i)
      {
         Vector2 checkPos = new Vector2(transform.position.x, transform.position.y + i);
         // check if we're already on top of something and if the space above us is air 
         if (Physics2D.OverlapCircle(checkPos, 0.2f, groundLayer) && !Physics2D.OverlapCircle(new Vector2(checkPos.x, checkPos.y + 1.0f), 0.2f, groundLayer))
         {
            // save the final position
            finalPos = checkPos + finalPosOffset;
            transform.position = finalPos;
            allSet = true;
            Debug.Log("Girin found a good spot!");
            return;
         }
      }

      if (allSet == false)
      {
         // if we didn't find an appropriate position, destroy
         Debug.Log("Girin was not able to find a good spot...");
         Destroy(gameObject);
      }
   }
	
	void Update ()
   {
		
	}
}