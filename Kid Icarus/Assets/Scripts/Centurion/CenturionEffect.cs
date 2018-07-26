using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenturionEffect : MonoBehaviour
{
   public float lerpSpeed;
   private GameObject goTo;
   private PlayerShoot refPlayerShoot;

	void Start ()
   {
      goTo = GameObject.FindGameObjectWithTag("UITarget");
      refPlayerShoot = GameObject.FindObjectOfType<PlayerShoot>();
      transform.parent = Camera.main.gameObject.transform;
	}
	
	void Update ()
   {
      transform.position = Vector2.Lerp(transform.position, goTo.transform.position, lerpSpeed);
      transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(0.5f, 0.5f, 0.5f), lerpSpeed);
	}

   private void OnTriggerEnter2D(Collider2D other)
   {
      if (other.CompareTag("UITarget"))
      {
         // increase centurions stored
         refPlayerShoot.centurionsStored++;

         // destroy self
         Destroy(gameObject);
      }
   }
}
