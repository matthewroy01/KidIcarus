using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCenturionStatue : MonoBehaviour
{
   [Header("If we're stuck in a wall")]
   public LayerMask wallLayer;

   private void Start()
   {
      // if the object spawns in a wall, destroy it
      if (Physics2D.OverlapCircle(transform.position, 0.2f, wallLayer))
      {
         Destroy(gameObject);
      }
   }
   private void OnDestroy()
   {
      GameObject.FindObjectOfType<PlayerShoot>().centurionsStored++;
   }
}
