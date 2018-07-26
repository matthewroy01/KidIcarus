using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCenturionStatue : MonoBehaviour
{
   [Header("If we're stuck in a wall")]
   public LayerMask wallLayer;

   Enemy refEnemy;
   private bool alreadyDead; // nani!?

   private void Start()
   {
      // if the object spawns in a wall, destroy it
      if (Physics2D.OverlapCircle(transform.position, 0.2f, wallLayer))
      {
         Destroy(gameObject);
      }

      refEnemy = GetComponent<Enemy>();
   }

   void Update()
   {
      // when the centurion statue "dies", increase the number of centurions stored
      if (refEnemy.isDead && !alreadyDead)
      {
         // GameObject.FindObjectOfType<PlayerShoot>().centurionsStored++;
         alreadyDead = true;
      }
   }
}
