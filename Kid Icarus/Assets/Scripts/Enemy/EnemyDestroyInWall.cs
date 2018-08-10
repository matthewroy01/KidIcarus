using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDestroyInWall : MonoBehaviour
{
   [Header("Checking for ground")]
   public LayerMask mask;
   public uint numberOfTries;
   public int yAccuracy;

   private bool foundASpot;

	void Start ()
   {
      // check three times before destroying
      for (int i = 0; i < numberOfTries; ++i)
      {
         if (Physics2D.OverlapCircle((Vector2)transform.position, 0.5f, mask) == true)
         {
            transform.position = new Vector2(Random.Range(0, 16), transform.position.y + (Random.Range(yAccuracy * -1, yAccuracy)));
         }
         else
         {
            foundASpot = true;
         }
      }

      // if we spawned inside a wall, destroy
      if (foundASpot == false)
      {
         Destroy(gameObject);
      }
   }
}
