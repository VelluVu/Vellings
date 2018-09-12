using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    bool enemyIsMovingLeft;
    bool enemyIsOnGround;
    bool enemyInitLerp;
    bool enemyIsInit;
    bool enemyMove;
    bool enemyPreviouslyGrounded;

    float enemyMoveSpeed;
    float enemyLerpSpeed;
    float enemyFallSpeed;
    float t;

    int t_x;
    int t_y;

    Node curNode;
    Node targetNode;
    
    Vector3 enemyTargetPos;
    Vector3 enemyStartPos;
    public SpriteRenderer enemySpriteRend;
    public Animator enemyAnim;

    

    private void start()
    {
        
        PlaceOnNode();
        enemyLerpSpeed = 0.2f;
        enemyMoveSpeed = 1f;
        enemyFallSpeed = 1.5f;
        
    }

    void PlaceOnNode()
    {
        curNode = GameControl.FindObjectOfType<GameControl>().enemySpawnNode;
        transform.position = GameControl.FindObjectOfType<GameControl>().enemySpawnPosition;
    }

    public void Update()
    {
        
        EnemyPathFind();


        enemySpriteRend.flipX = enemyIsMovingLeft;

    }

    

    public bool EnemyPathFind()
    {
        if (curNode == null)
        {
            enemyTargetPos = transform.position;
            enemyTargetPos.y = -50;
            enemyPreviouslyGrounded = enemyIsOnGround;
            return false;
        }

        t_x = curNode.x;
        t_y = curNode.y;

        bool belowIsAir = EnemyIsAir(t_x, t_y - 1);
        bool nextIsEmpty = EnemyIsAir(t_x, t_y);

        if (belowIsAir)
        {

            t_x = curNode.x;
            t_y -= 1;
            
        }
        else
        {

            enemyIsOnGround = true;

        }

        int s_x = (enemyIsMovingLeft) ? t_x - 1 : t_x + 1;
        bool stopped = IsStopped(s_x, t_y);

        if (stopped)
        {
            enemyIsMovingLeft = !enemyIsMovingLeft;
            t_x = (enemyIsMovingLeft) ? curNode.x - 1 : curNode.x + 1;
            t_y = curNode.y;
        }
        else if (nextIsEmpty)

        {
            t_x = (enemyIsMovingLeft) ? t_x - 1 : t_x + 1;
            t_y = curNode.y;
        }
        
        
        targetNode = GameControl.FindObjectOfType<GameControl>().GetNode(t_x, t_y);
        return true;

    }    

    void EnemyDie()
    {

    }


    bool IsStopped(int x, int y)
    {
        Node n = GameControl.FindObjectOfType<GameControl>().GetNode(x, y);
        if (n == null)
        {
            return false;
        }
        return n.isStopped;
    }

    bool EnemyIsAir(int x, int y)
    {
        Node n = GameControl.FindObjectOfType<GameControl>().GetNode(x, y);

        
        if (n == null)
        {
            return true;
        }

        return n.isEmpty;

        /*if (n.isFiller)
        {
            isAir = true;
        }*/      
    }
}
