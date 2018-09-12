using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    bool enemyIsMovingLeft;
    bool enemyIsOnGround;
    float enemyMoveSpeed;

    int t_x;
    int t_y;

    Node curNode;
    Node targetNode;

    private void Update()
    {
        EnemyPathFind();
    }

    public void EnemyPathFind()
    {
        t_x = curNode.x;
        t_y = curNode.y;

        bool belowIsAir = EnemyIsAir(t_x, t_y - 1);
        bool nextIsEmpty = EnemyIsAir(t_x, t_y);

        if (belowIsAir)
        {
            t_x = curNode.x;
            t_y -= 1;
            enemyIsOnGround = false;
            
        }
        else
        {
            enemyIsOnGround = true;
        }
        

    }
    bool EnemyIsAir(int x, int y)
    {
        Node n = GameControl.FindObjectOfType<GameControl>().GetNode(x, y);
        if (n == null)
        {
            return true;
        }
        bool isAir = n.isEmpty;
        if (n.isFiller)
        {
            isAir = true;
        }
        return isAir;
    }
}
