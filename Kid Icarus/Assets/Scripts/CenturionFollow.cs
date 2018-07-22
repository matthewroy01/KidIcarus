using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenturionFollow : MonoBehaviour
{
	[Header("Position relative to the player")]
	public Vector2 targetPosition;

	[Header("Movement")]
	[Range(0, 1)]
	public float followSpeed;

	[Header("Shooting")]
	public float arrowProjectileSpeed;
	public GameObject arrowObject;

	[Header("Health")]
	public int maxHealth;
	private int currentHealth;
	public float invincibilityTime;
	public bool isDead;
	private bool canGetHit = true;
	public Sound death;

	private PlayerMovement refPlayerMovement;
	private PlayerCollision refPlayerCollision;
   private PlayerShoot refPlayerShoot;
	private Transform refPlayer;
	private SpriteRenderer sr;
	private Rigidbody2D rb;
	private UtilityAudioManager refAudioManager;

	void Start ()
	{
		refPlayerMovement = GameObject.FindObjectOfType<PlayerMovement>();
		refPlayerCollision = GameObject.FindObjectOfType<PlayerCollision>();
      refPlayerShoot = GameObject.FindObjectOfType<PlayerShoot>();
		refPlayer = GameObject.FindGameObjectWithTag("Player").transform;
		refAudioManager = GameObject.FindObjectOfType<UtilityAudioManager>();
		sr = GetComponent<SpriteRenderer>();
		rb = GetComponent<Rigidbody2D>();

		transform.parent = refPlayer;

		currentHealth = maxHealth;
	}

	void Update ()
	{
		if (currentHealth <= 0 && isDead == false)
		{
			isDead = true;
			refAudioManager.PlaySound(death.clip, death.volume, 1.5f);
			refPlayerShoot.hasCenturion = false;
		}

		if (isDead == false)
		{
			FlipSprite();
			UpdatePosition();
			Shooting();
		}
		else
		{
			sr.flipY = true;
			rb.velocity = Vector2.down * arrowProjectileSpeed;
			Invoke("Dest", 2.0f);
		}

	}

	private void FlipSprite()
	{
		sr.flipX = !refPlayerMovement.facingRight;
	}

	private void UpdatePosition()
	{
		if (refPlayerMovement.facingRight)
		{
			transform.position = Vector2.Lerp(transform.position, (Vector2)refPlayer.position + targetPosition, followSpeed);
		}
		else
		{
			transform.position = Vector2.Lerp(transform.position, (Vector2)refPlayer.position + new Vector2(targetPosition.x * -1, targetPosition.y), followSpeed);
		}
	}

	private void Shooting()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Vector2 shootDir;
			float zRotation;
			GameObject tmp;

			// facing right
			if (refPlayerMovement.facingRight)
			{
				shootDir = Vector2.right * arrowProjectileSpeed;
				zRotation = -90;
			}
			// facing left
			else
			{
				shootDir = Vector2.right * arrowProjectileSpeed * -1;
				zRotation = 90;
			}

			// instantiate the arrow
			tmp = Instantiate(arrowObject, transform.position, Quaternion.Euler(0, 0, zRotation));
			tmp.GetComponent<Rigidbody2D>().velocity = shootDir;
		}
	}

	private void OnTriggerStay2D(Collider2D other)
	{
		if (other.CompareTag("Enemy") && canGetHit == true)
		{
			currentHealth--;
			canGetHit = false;
			Invoke("StopInvincibility", invincibilityTime);
		}
	}

	private void StopInvincibility()
	{
		canGetHit = true;
	}

	private void Dest()
	{
		Destroy(gameObject);
	}
}