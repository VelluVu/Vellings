using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public bool enemyIsMovingLeft;
    public bool enemyIsOnGround;
    bool attacked;

    public float enemyMoveSpeed;
    public float distance;

    public SpriteRenderer enemySpriteRend;
    public Animator enemyAnim;

    public Transform groundDetector;

    public void Update()
    {

        transform.Translate((Vector2.right * enemyMoveSpeed) * Time.deltaTime);
        RaycastHit2D groundRay = Physics2D.Raycast(groundDetector.position, Vector2.down, distance);
        //Debug.Log(groundRay.distance);

        if (groundRay.distance <= 0.15)
        {
            enemyIsOnGround = true;
        } else
        {
            enemyIsOnGround = false;
        }

        if (groundRay.collider == false || groundRay.distance <= 0.05f)
        {            

            if (enemyIsMovingLeft == false)
            {
                transform.eulerAngles = new Vector3(0, -180, 0);
                enemyIsMovingLeft = true;
            }
            else
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
                enemyIsMovingLeft = false;
            }
        } 
    } 

    void EnemyDie()
    {

        enemyAnim.SetBool("enemydie", true);
       

    }

    public void Attack()
    {        

        enemyAnim.SetBool("enemyattack", true);
        attacked = true;

    }


    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.collider.tag == ("unit") && enemyIsOnGround && !attacked)
        {
            
            Attack();
            
        }
        else
        {
            enemyAnim.SetBool("enemyattack", false);
            attacked = false;
        }

        /*if (collision.collider.tag == ("ground"))
        {
            enemyIsOnGround = true;
            
        }
        else
        {
            enemyIsOnGround = false;
                     
        }
        */
    }

    public float EnemyXPos ()
    {
        return this.transform.position.x;
    }

    public float EnemyYPos()
    {
        return this.transform.position.y;
    }
       
}
