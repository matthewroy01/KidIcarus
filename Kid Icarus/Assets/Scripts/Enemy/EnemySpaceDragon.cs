using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpaceDragon : MonoBehaviour
{
    private Enemy refEnemy;
    private GameObject cam;
    private GameObject refPlayer;
    private SpriteRenderer refSpriteRenderer;

    [Header("Positioning")]
    public float startingY;
    public float restingY;
    [Range(0.0f, 1.0f)]
    public float spdEntry;

    [Header("Movement")]
    public float horizontalRange;

    [Header("Attacks")]
    public float attackCooldown;
    public GameObject fireball;
    public float spdFireball;
    public int fireballCount;

    void Start()
    {
        refEnemy = GetComponent<Enemy>();
        refPlayer = GameObject.FindGameObjectWithTag("Player");
        refSpriteRenderer = GetComponent<SpriteRenderer>();

        cam = Camera.main.gameObject;
        transform.parent = cam.transform;

        // set default position
        transform.position = new Vector2(cam.transform.position.x, cam.transform.position.y + startingY);

        StartCoroutine(SpaceDragonMovement());
    }

    private void Update()
    {
        refSpriteRenderer.flipX = refPlayer.transform.position.x < transform.position.x;
    }

    private IEnumerator SpaceDragonMovement()
    {
        // enter screen
        while (Vector2.Distance(transform.position, new Vector2(cam.transform.position.x, cam.transform.position.y + restingY)) > 0.2f)
        {
            transform.position = Vector2.Lerp(transform.position, new Vector2(cam.transform.position.x, cam.transform.position.y + restingY), spdEntry);
            yield return new WaitForEndOfFrame();
        }
        
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(SpaceDragonAttacks());

        while (Mathf.Abs(Mathf.Cos(Time.time)) > 0.01f)
        {
            yield return new WaitForEndOfFrame();
        }

        while (true)
        {
            transform.position = new Vector2(cam.transform.position.x + (Mathf.Cos(Time.time) * horizontalRange), cam.transform.position.y + restingY);
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator SpaceDragonAttacks()
    {
        while (!refEnemy.isDead)
        {
            int rand = Random.Range(0, 1);
            switch(rand)
            {
                case 0:
                {
                    for (int i = 0; i < fireballCount; ++i)
                    {
                        Rigidbody2D tmp = Instantiate(fireball, transform.position, Quaternion.identity).GetComponent<Rigidbody2D>();
                        tmp.velocity = (refPlayer.transform.position - transform.position).normalized * spdFireball;

                        yield return new WaitForSeconds(0.05f);
                    }
                    break;
                }
                case 1:
                {
                    break;
                }
                default:
                {
                    break;
                }
            }

            yield return new WaitForSeconds(attackCooldown);
        }
    }
}
