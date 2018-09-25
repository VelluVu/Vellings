using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public bool enemyIsMovingLeft;
    public bool enemyMove;
    bool enemyPreviouslyGrounded;
    public bool enemyOnGround;
    bool attacked;

    public int enemyHealth;
    int enemyAirLimit;
    int et_x;
    int et_y;

    public float distance;
    float enemyMoveSpeed;
    float enemyFallSpeed;
    float lerpSpeed;
    float edieT;
    float t;
    float enemyLerpSpeed;

    public SpriteRenderer enemySpriteRend;
    public Animator enemyAnim;
    public Transform groundDetector;
    
    
    GameControl gameControl;
    Node enemyCurNode;
    Node enemyTargetNode;
    Vector3 enemyTargetPos;
    Vector3 enemyStartPos;
    public bool enemyIsInit;
    private bool enemyInitLerp;

   
    private void Start()
    {     
        gameControl = GetComponent<GameControl>();
        enemyFallSpeed = 0.6f;
        lerpSpeed = 0.5f;
        enemyHealth = 100;
        edieT = 0.55f;
        enemyInitLerp = false;
    }

    public void EnemyPlaceOnNode()
    {
        enemyCurNode = gameControl.GetNodeFromWorldPos(transform.position);
        enemyStartPos = gameControl.GetWorldPosFromNode(enemyCurNode);
        
    }

    
    public void Update()
    {
     
        enemySpriteRend.flipX = enemyIsMovingLeft;

        if (!enemyInitLerp)
        {
            enemyInitLerp = true;
            enemyStartPos = transform.position;
            t = 0;
            EnemyPathFind();
            Vector3 tp = gameControl.GetWorldPosFromNode(enemyTargetNode);
            enemyTargetPos = tp;
            float d = Vector3.Distance(enemyTargetPos, enemyStartPos);

            if (enemyOnGround)
            {
                enemyMoveSpeed = enemyLerpSpeed / d;

            }
            else
            {
                enemyMoveSpeed = enemyFallSpeed / d;
            }

        }
        else
        {
            t += Time.deltaTime * enemyMoveSpeed;

            if (t > 1)
            {
                t = 1;
                enemyInitLerp = false;
                enemyCurNode = enemyTargetNode;
            }
            Vector3 tp = Vector3.Lerp(enemyStartPos, enemyTargetPos, t);
            transform.position = tp;
        }

    }


    void EnemyPathFind()
    {
        
        et_x = enemyCurNode.x;
        et_y = enemyCurNode.y;

        bool enemyDownIsAir = EnemyIsAir(et_x, et_y - 1);
        bool enemyNextIsAir = EnemyIsAir(et_x, et_y);

        //Falling down.
        if (enemyDownIsAir)
        {
            et_x = enemyCurNode.x;
            et_y -= 1;

            if (enemyOnGround)
            {
                enemyAirLimit++;

                if (enemyAirLimit > 2)
                {
                    enemyOnGround = false;
                }
            }

        }
        //On the ground.
        else
        {
            enemyOnGround = true;
            enemyAirLimit = 0;

            if (enemyNextIsAir)
            {
                et_x = (enemyIsMovingLeft) ? et_x - 1 : et_x + 1;
                et_y = enemyCurNode.y;
            }
            else
            {
                int height = 0;
                bool isValid = false;


                while (height < 4)
                {

                    bool n_isAir = EnemyIsAir(et_x, et_y + height);

                    if (n_isAir)

                    {
                        isValid = true;
                        break;
                    }
                    height++;
                }

                if (isValid)
                {
                    et_y += height;
                }
                else
                {

                    enemyIsMovingLeft = !enemyIsMovingLeft;
                    et_x = (enemyIsMovingLeft) ? enemyCurNode.x - 1 : enemyCurNode.x + 1;


                }
            }
        }

    enemyTargetNode = gameControl.GetNode(et_x, et_y);
    enemyPreviouslyGrounded = enemyOnGround;
        
    }   

    bool EnemyIsAir(int x, int y)
    {
        Node n = gameControl.GetNode(x, y);
        if (n == null)
        {
            return true;
        }     
        return n.isEmpty;
    }

    /*public void Update()
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
    } */

    public void EnemyTakeDamage(int damage)
    {
        enemyHealth -= damage;

        if (enemyHealth <= damage)
        {          
            StartCoroutine(EnemyDeath());
        }

    }
    
        IEnumerator EnemyDeath()
    {
        enemyMoveSpeed = 0;
        enemyAnim.SetBool("enemydie", true);
        yield return new WaitForSecondsRealtime(edieT);
        Destroy(gameObject);
    }

    public void Attack()
    {        

        enemyAnim.SetBool("enemyattack", true);
        attacked = true;

    }


    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.collider.tag == ("unit") && enemyOnGround && !attacked)
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
