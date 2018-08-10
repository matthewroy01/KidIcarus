using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFallAndRotate : MonoBehaviour
{
   [Header("Falling")]
   public float targetY;
   public float waitTime;
   public float gravityScale;

   [Header("Rotating")]
   public float rotationInterval;
   private float rotateBy = 0.0f;

   private Rigidbody2D rb;
   private Enemy refEnemy;
   private Animator refAnimator;

   private GameObject refCamera;

	void Start ()
   {
      rb = GetComponent<Rigidbody2D>();
      refEnemy = GetComponent<Enemy>();
      refAnimator = GetComponent<Animator>();
      refCamera = GameObject.FindObjectOfType<Camera>().gameObject;
      transform.parent = refCamera.transform;

      transform.position = (Vector2)refCamera.transform.position + new Vector2(Random.Range(-7.0f, 7.0f), targetY);

      Invoke("StartFalling", waitTime);
	}

   private void StartFalling()
   {
      transform.parent = null;
      rb.gravityScale = gravityScale;
      refAnimator.SetTrigger("falling");
      StartCoroutine("Rotation");
   }

   private IEnumerator Rotation()
   {
      while (refEnemy.isDead == false)
      {
         transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, transform.rotation.z + rotateBy);
         rotateBy += 90.0f;
         yield return new WaitForSeconds(rotationInterval);
      }

      rb.velocity = Vector2.zero;
      rb.gravityScale = 0.0f;
   }
}
