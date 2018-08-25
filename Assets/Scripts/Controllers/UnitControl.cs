using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitControl : MonoBehaviour {

    bool isInit;
    public bool move;
    public bool onGround;
    bool previouslyGrounded;
    bool movingLeft;
    bool initLerp;


    float MoveSpeed;
    public float lerpSpeed = 1;
    float t;
    int t_x;
    int t_y;
    int airFrames;

    Node curNode;
    Node targetNode;
    Vector3 targetPos;
    Vector3 startPos;

    GameControl gameControl;
    public SpriteRenderer renderer;

    
    
    

	public void Init(GameControl gc)
    {
        gameControl = gc;
        PlaceOnNode();
        isInit = true;
	}

    void PlaceOnNode()
    {
        curNode = gameControl.spawnNode;
        transform.position = gameControl.spawnPosition;
    }
	
	void Update () {
		
        if (!isInit)
        {
            return;
        }
        if (curNode == null)
        {
            return;
        }
        if (!move)
        {
            return;
        }
        if (!initLerp)
        {
            initLerp = true;
            startPos = transform.position;
            t = 0;
            Pathfind();
            Vector3 tp = gameControl.GetWorldPosFromNode(targetNode);
            targetPos = tp;
            float d = Vector3.Distance(targetPos, startPos);
            MoveSpeed = lerpSpeed / d;

        } else
        {
            t += Time.deltaTime * MoveSpeed;
            if(t > 1)
            {
                t = 1;
                initLerp = false;
                curNode = targetNode;
            }

            Vector3 tp = Vector3.Lerp(startPos, targetPos, t);
            transform.position = tp;
        }

        renderer.flipX = movingLeft;

	}
    void Pathfind()
    {
        t_x = curNode.x;
        t_y = curNode.y;

        bool downIsAir = IsAir(t_x, t_y - 1);
        bool forwardIsAir = IsAir(t_x, t_y);

        //Falling down.
        if(downIsAir)
        {
            t_x = curNode.x;
            t_y -= 1;

            onGround = false;
            if(!onGround && previouslyGrounded)
            {

            }

        }
        //On the ground.
        else
        {
            onGround = true;
            if(onGround && !previouslyGrounded)
            {

            }
            if (forwardIsAir)
            {
                t_x = (movingLeft) ? t_x - 1 : t_x + 1;
                t_y = curNode.y;
            }
            else
            {
                int s = 0;
                bool isValid = false;
                while ( s < 3)
                {
                    s++;
                    bool f_isAir = IsAir(t_x, t_y + s);
                    if (f_isAir)
                    {
                        isValid = true;
                        break;
                    }
                }
                if (isValid)
                {
                    t_y += s;
                } else
                {
                    movingLeft = !movingLeft;
                    t_x = (movingLeft) ? curNode.x - 1 : curNode.y + 1;
                }
            }
        }

        targetNode = gameControl.GetNode(t_x,t_y);
        previouslyGrounded = onGround;

    }

    bool IsAir(int x, int y)
    {
        Node n = gameControl.GetNode(x, y);
        if (n == null)
        {
            return true;
        }
        return n.isEmpty;
    }
}
