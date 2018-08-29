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
    public bool isDigForward;

    public Ability curAbility;

    float moveSpeed;
    float lerpSpeed = 0.3f;
    float fallSpeed = 3f;
    float umbrellaSpeed = 0.3f;
    float dig_d_speed = 0.030f;

    float t;
    int t_x;
    int t_y;

    int airLimit;
    public int digFrontLimit = 40;
    int digFCount;
    public int digBelowLimit = 30;
    int digBCount;
    public int deathFallLimit = 70;

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
	
    //Replaces Update, this is updated on GameControl class.
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
                Walker(delta);
                break;
            case Ability.bouncer:
                Bouncer();
                break;
            case Ability.dig_forward:
                DigForward(delta);
                break;
            case Ability.dig_down:
                DigBelow(delta);
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
            //land on ground
            if(onGround && !previouslyGrounded)
            {
                Debug.Log(airLimit);
                if (airLimit > deathFallLimit && !isUmbrella)
                {
                    targetNode = curNode;
                    ChangeAbility(Ability.die);
                    unitAnimator.Play("Death_land");
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
                    bool startDig = false;

                    while (height < 4)
                    {
                        
                        bool n_isAir = IsAir(t_x, t_y + height);

                        if (isDigForward)
                        {
                            if (height > 0)
                            {
                                if (!n_isAir)
                                {
                                    startDig = true;
                                    break;
                                }
                            }
                        }

                        if (n_isAir)

                        {
                            isValid = true;
                            break;
                        }
                        height++;
                    }

                    if (isValid && !startDig)
                    {
                        t_y += height;
                    }
                    else
                    {
                    
                        if (startDig)
                        {
                            curAbility = Ability.dig_forward;
                            unitAnimator.Play("Digfront");
                            return false;
                        }
                        else
                        { 
                        
                            movingLeft = !movingLeft;
                            t_x = (movingLeft) ? curNode.x - 1 : curNode.x + 1;
                        
                        }
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
            else
            {

                

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
            LerpIntoPosition(delta);
        }
    }

    void LerpIntoPosition(float delta)
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

                if (previouslyGrounded)
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
                isUmbrella = true;
                unitAnimator.Play("Slowfall");
                break;

            case Ability.dig_forward:
                isDigForward = true;
                digFCount = 0;
                break;

            case Ability.dig_down:
                if (previouslyGrounded)
                {
                    unitAnimator.Play("Digbelow");
                    curAbility = a;
                    digBCount = 0;
                    return true;
                }
                else
                {
                    return false;
                }
                      
            case Ability.explode:
                curAbility = a;
                unitAnimator.Play("Explode");
                break;

            case Ability.die:
                curAbility = a;
                unitAnimator.Play("Die");
                break;

            default:
                break;
        }

        return true;

    }

    void DigBelow(float delta)
    {
        if (!initLerp)
        {
            initLerp = true;
            startPos = transform.position;

            t = 0;

            int t_x = (movingLeft) ? curNode.x + 1 : curNode.x - 1;

            Node originNode = gameControl.GetNode(t_x, curNode.y + 3);

            List<Node> nodes = CheckNode(originNode, 6);

            if (nodes.Count == 0 || digBCount > digBelowLimit)
            {
                ChangeAbility(Ability.walker);
                return;
            }
            digBCount++;
   
            gameControl.AddNodePossibilities(nodes);
            Node n = gameControl.GetNode(curNode.x, curNode.y - 1);
            if (n == null)
            {
                ChangeAbility(Ability.walker);
                return;
            }

            targetNode = n;
            targetPos = gameControl.GetWorldPosFromNode(targetNode);

            float distance = Vector3.Distance(targetPos, startPos);
            moveSpeed = dig_d_speed / distance;

        }
        else
        {
            LerpIntoPosition(delta);
        }
    }

    void DigForward(float delta)
    {
        if (!initLerp)
        {
            initLerp = true;
            startPos = transform.position;

            t = 0;

            int t_x = (movingLeft) ? curNode.x -2 : curNode.x + 2;

            Node originNode = gameControl.GetNode(t_x -4, t_y + 8);

            List<Node> nodes = CheckNode(originNode, 9);

            if (nodes.Count == 0 || digFCount > digFrontLimit)
            {
                ChangeAbility(Ability.walker);
                isDigForward = false;
                return;
            }
            digFCount++;

            gameControl.AddNodePossibilities(nodes);
 
            Node n = gameControl.GetNode(t_x, curNode.y);

            if (n == null)
            {
                ChangeAbility(Ability.walker);
                return;
            }

            targetNode = n;
            targetPos = gameControl.GetWorldPosFromNode(targetNode);

            float distance = Vector3.Distance(targetPos, startPos);
            moveSpeed = dig_d_speed / distance;

        }
        else
        {
            LerpIntoPosition(delta);
        }
    }

    List<Node> CheckNode(Node origin, float rad)
    {
        List<Node> r = new List<Node>();
        Vector3 center = gameControl.GetWorldPosFromNode(origin);
        float radius = rad * gameControl.posOffset;

        for (int x = -16; x < 16; x++)
        {
            for (int y = -16; y < 16; y++)
            {
                int t_x = x + curNode.x;
                int t_y = y + curNode.y;

                float d = Vector3.Distance(center, gameControl.GetWorldPosFromNode(t_x, t_y));
                if (d > radius)
                {
                    continue;
                }

                Node n = gameControl.GetNode(t_x, t_y);
                if (n == null)
                {
                    continue;
                }
                if (!n.isEmpty)
                {
                    r.Add(n);
                }
            }
        }
        return r;
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
