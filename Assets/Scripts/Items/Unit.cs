using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

    bool isInit;
    public bool move;
    public bool onGround;
    bool previouslyGrounded;
    bool movingLeft;
    bool initLerp;


    float MoveSpeed;
    float lerpSpeed = 0.3f;
    public float fallSpeed = 3f;
    float t;
    int t_x;
    int t_y;
    int airLimit;

    Node curNode;
    Node targetNode;
    Vector3 targetPos;
    Vector3 startPos;

    GameControl gameControl;
    public SpriteRenderer ren;
    public Animator unitAnimator;
    
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
	
	public void FrameTick (float delta) {
		
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

            float distance = Vector3.Distance(targetPos, startPos);

            if (onGround)
            {
                MoveSpeed = lerpSpeed / distance;
            } else
            {
                MoveSpeed = fallSpeed / distance;
            }
        } else
        {
            t += delta * MoveSpeed;
            if(t > 1)
            {
                t = 1;
                initLerp = false;
                curNode = targetNode;
            }

            Vector3 tp = Vector3.Lerp(startPos, targetPos, t);
            transform.position = tp;
        }

        ren.flipX = movingLeft;

	}
    void Pathfind()
    {
        t_x = curNode.x;
        t_y = curNode.y;

        bool belowIsAir = IsAir(t_x, t_y - 1);
        bool nextIsAir = IsAir(t_x, t_y);

        //Falling down.
        if(belowIsAir)
        {
            t_x = curNode.x;
            t_y -= 1;

            if(onGround)
            {
                airLimit++;

                if (airLimit > 4)
                {
                    onGround = false;
                    unitAnimator.Play("Falling");
                }
            }
        }
        //On the ground.
        else
        {
            onGround = true;
            airLimit = 0;
            if(onGround && !previouslyGrounded)
            {
                unitAnimator.Play("Walking");
            }

            if (nextIsAir)
            {
                t_x = (movingLeft) ? t_x - 1 : t_x + 1;
                t_y = curNode.y;
            }
            else
            {
                int height = 0;
                bool isValid = false;
                while (height < 4)
                {
                    height++;
                    bool f_isAir = IsAir(t_x, t_y + height);
                    if (f_isAir)
                    {
                        isValid = true;
                        break;
                    }
                }
                if (isValid)
                {
                    t_y += height;
                }
                else
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
