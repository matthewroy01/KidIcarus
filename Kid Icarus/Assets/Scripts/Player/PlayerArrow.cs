using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArrow : MonoBehaviour
{
    public Rigidbody2D rb;
    public bool useChargeSpeed;

    private PlayerShoot refPlayerShoot;
    private GameObject closestEnemy;

    private void Start()
    {
        refPlayerShoot = FindObjectOfType<PlayerShoot>();

        FindClosestEnemy();
    }

    void Update()
	{
        Align();
        HomeIn();

        // if the enemy died, look for another one
        if (closestEnemy == null)
        {
            FindClosestEnemy();
        }
	}

    private void FindClosestEnemy()
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        float distance = float.MaxValue;

        // find the enemy that is the shortest distance away
        for (int i = 0; i < enemies.Length; ++i)
        {
            if (enemies[i].immuneToArrows == false)
            {
                float check = Vector2.Distance(transform.position, enemies[i].transform.position);
                if (check < distance)
                {
                    distance = check;
                    closestEnemy = enemies[i].gameObject;
                }
            }
        }
    }

    private void Align()
    {
        // point in the direction we're moving
        transform.up = rb.velocity;
    }

    private void HomeIn()
    {
        if (closestEnemy != null)
        {
            // calculate direction to closest enemy
            Vector2 direction = (Vector2)closestEnemy.transform.position - (Vector2)transform.position;
            direction = direction.normalized;

            // change the arrow's direction to home in on a target
            rb.AddForce(direction * (refPlayerShoot.arrowHomingBase + (refPlayerShoot.arrowHomingLevel * refPlayerShoot.arrowHomingIncrement)));

            // normalize speed
            if (useChargeSpeed)
            {
                rb.velocity = rb.velocity.normalized * refPlayerShoot.arrowSpeedCharged;
            }
            else
            {
                rb.velocity = rb.velocity.normalized * refPlayerShoot.arrowSpeedNormal;
            }
        }
    }
}
