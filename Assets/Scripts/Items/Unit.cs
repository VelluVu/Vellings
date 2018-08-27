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

    public bool isUmbrella;

    public Ability curAbility;

    float moveSpeed;
    float lerpSpeed = 0.3f;
    public float fallSpeed = 3f;
    public float umbrellaSpeed = 0.3f;
    float t;
    int t_x;
    int t_y;
    int airLimit;

    Node curNode;
    Node targetNode;
    Vector3 targetPos;
    Vector3 startPos;

    GameControl gameControl;
    List<Node> stoppedNodes = new List<Node>();

    public SpriteRenderer ren;
    public Animator unitAnimator;
    
	public void Init(GameControl gc)
    {
        gameControl = gc;
        PlaceOnNode();
        isInit = true;
        curAbility = Ability.walker;
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

        if (!move)
        {
            return;
        }

        ren.flipX = movingLeft;

        unitAnimator.SetBool("isUmbrella", isUmbrella);

        switch (curAbility)
        {
            case Ability.walker:
            case Ability.umbrella:
            case Ability.dig_forward:
                Walker(delta);
                break;
            case Ability.bouncer:
                Bouncer();
                break;
            case Ability.dig_down:
                break;
            case Ability.explode:
                break;
            case Ability.die:
                break;
            default:
                break;
        }

	}

    bool Pathfind()
    {
        if ( curNode == null)
        {
            targetPos = transform.position;
            targetPos.y = -50;
            previouslyGrounded = onGround;
            return false;
        }

        t_x = curNode.x;
        t_y = curNode.y;

        bool belowIsAir = IsAir(t_x, t_y - 1);
        bool nextIsAir = IsAir(t_x, t_y);

        //Falling down.
        if(belowIsAir)
        {
            t_x = curNode.x;
            t_y -= 1;
            airLimit++;

            if (onGround)
            {
                
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
            //land on ground
            if(onGround && !previouslyGrounded)
            {
                if (airLimit > 60 && !isUmbrella)
                {
                    targetNode = curNode;
                    ChangeAbility(Ability.die);
                    unitAnimator.Play("Explode");
                    previouslyGrounded = onGround;
                    return true;
                }
                else
                {
                    unitAnimator.Play("Land");
                    targetNode = curNode;
                    previouslyGrounded = onGround;
                    airLimit = 0;
                    return true;
                }
            }
            airLimit = 0;

            int s_x = (movingLeft) ? t_x - 1 : t_x + 1;
            bool stopped = IsStopped(s_x, t_y);

            if (stopped)
            {
                movingLeft = !movingLeft;
                t_x = (movingLeft) ? curNode.x - 1 : curNode.x + 1;
                t_y = curNode.y;
            }
            else
            {

                if (nextIsAir)
                {
                    t_x = (movingLeft) ? t_x - 1 : t_x + 1;
                    t_y = curNode.y;
                }
                else
                {
                    int height = 0;
                    bool isValid = false;
                    while (height < 3)
                    {
                        height++;
                        bool n_isAir = IsAir(t_x, t_y + height);
                        if (n_isAir)
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
        }

        targetNode = gameControl.GetNode(t_x,t_y);
        previouslyGrounded = onGround;
        return true;

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

    bool IsStopped(int x, int y)
    {
        Node n = gameControl.GetNode(x, y);
        if (n == null)
        {
            return false;
        }
        return n.isStopped;
    }

    void Walker(float delta)
    {
        if (!initLerp)
        {
            initLerp = true;
            startPos = transform.position;

            t = 0;

            bool onPath = Pathfind();

            if (onPath)
            {

                Vector3 tp = gameControl.GetWorldPosFromNode(targetNode);
                targetPos = tp;

            }

            float distance = Vector3.Distance(targetPos, startPos);

            if (onGround)
            {
                moveSpeed = lerpSpeed / distance;
            }
            else
            {
                if (isUmbrella)
                {
                    moveSpeed = umbrellaSpeed / distance;
                }
                else
                {

                    moveSpeed = fallSpeed / distance;
                }
            }
        }
        else
        {
            t += delta * moveSpeed;
            if (t > 1)
            {
                t = 1;
                initLerp = false;
                curNode = targetNode;
            }

            Vector3 tp = Vector3.Lerp(startPos, targetPos, t);
            transform.position = tp;
        }
    }

    void Bouncer()
    {

    }
    
    public bool ChangeAbility(Ability a)
    {
        isUmbrella = false;

        switch(a)
        {
            case Ability.walker:
                curAbility = a;
                unitAnimator.Play("Walking");
                break;
            case Ability.bouncer:

                if (onGround)
                {
                    FindStopNodes();
                    curAbility = a;
                    unitAnimator.Play("Bouncer");
                    return true;
                } else
                {
                    return false;
                }
         
            case Ability.umbrella:
                curAbility = a;
                isUmbrella = true;
                unitAnimator.Play("Slowfall");
                break;
            case Ability.dig_forward:
                curAbility = a;
                unitAnimator.Play("Digfront");
                break;
            case Ability.dig_down:
                curAbility = a;
                unitAnimator.Play("Digbelow");
                break;
            case Ability.explode:
                curAbility = a;
                unitAnimator.Play("Explode");
                break;
            case Ability.die:
                curAbility = a;
                unitAnimator.Play("Explode");
                break;
            default:
                break;
        }

        return true;

    }

    void FindStopNodes()
    {
        for (int x = -2; x < 2; x++)
        {
            for(int y = 0; y < 4; y++)
            {
                Node n = gameControl.GetNode(curNode.x + x, curNode.y + y);
                if (n == null)
                {
                    continue;
                }
                n.isStopped = true;
                stoppedNodes.Add(n);
            }
        }
    }

    void ClearStopNodes()
    {
        for (int i = 0; i < stoppedNodes.Count; i++)
        {
            stoppedNodes[i].isStopped = false;
        }
        stoppedNodes.Clear();
    }
}
