﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

    public bool move;
    public bool hasLeft;
    public bool isGunning;

    bool isInit;
    bool previouslyGrounded;
    bool movingLeft;
    bool initLerp;
    bool isUmbrella;
    bool isDigForward;
    bool onGround;
    bool startFilling;
    bool imDead;
    bool hasTurn;

    float moveSpeed;
    float lerpSpeed;
    float fallSpeed;
    float umbrellaSpeed;
    float dig_d_speed;
    float build_Time;
    float build_Speed;
    float buildAmount;
    float startFill;
    float startfill_t;
    float fill_t;
    float pix_t;
    float build_t;
    float t;
    float explodeTimer;
    float explodeRadius;
    float explode_t;
    float react;
    float dieT;
    float fireTime;  
    float prepareToFire;

    int t_x;
    int t_y;
    int airLimit;
    int digBCount;
    int build_Count;
    int digFCount;
    int deathFallLimit;
    int digBelowLimit;
    int digFrontLimit;
    int maxPixels;

    public int pixelAmount;

    public Ability curAbility;

    Node curNode;
    Node targetNode;
    Vector3 targetPos;
    Vector3 startPos;

    Minigun wep;
    ParticleSystem bulletRain;
    GameControl gameControl;
    List<Node> stoppedNodes = new List<Node>();

    public SpriteRenderer ren;
    public Animator unitAnimator;
    
    //Custom initialize

    public void Init(GameControl gc)
    {
        
        gameControl = gc;
        PlaceOnNode();
        isInit = true;
        curAbility = Ability.walker;
        lerpSpeed = 0.2f;
        fallSpeed = 3f;
        umbrellaSpeed = 0.3f;
        dig_d_speed = 0.03f;
        build_Time = 0.9f;
        build_Speed = 0.03f;
        buildAmount = 25f;
        startFill = 3.1f;
        explodeTimer = 1.10f;
        explodeRadius = 12f;
        deathFallLimit = 70;
        digBelowLimit = 30;
        digFrontLimit = 40;
        maxPixels = 80;
        react = 0.1f;
        dieT = 0.8f;
        imDead = false;
        isGunning = false;
        
    }

    //Spawn Position of this unit

    void PlaceOnNode()
    {
        curNode = gameControl.spawnNode;
        transform.position = gameControl.spawnPosition;
    }
	
	public void Tick (float delta) {


        if (!isInit)
        {
            return;
        }

        if (!move)
        {
            return;
        }
     

        FlipUnit();

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
            case Ability.builder:
                Builder(delta);
                break;
            case Ability.filler:     
                Filler(delta);                
                break;
            case Ability.explode:
                Explodes(delta);
                break;
            case Ability.minigun:
                UseMinigun(delta);              
                break;
            case Ability.die:
                break;
            default:
                break;
        }

	}

    void FlipUnit()
    {
        
        ren.flipX = movingLeft;
        
    }
    
   
    public bool ChangeAbility(Ability a)
    {
        isUmbrella = false;

        switch (a)
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
                    FindObjectOfType<UnitControl>().SetOutCount(-1);
                    return true;
                }
                else
                {
                    return false;
                }

            case Ability.umbrella:
                isUmbrella = true;
                unitAnimator.Play("Slowfall");
                break;

            case Ability.dig_forward:
                //isDigForward = true;
                unitAnimator.Play("Digfront");
                curAbility = a;
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

            case Ability.filler:
                curAbility = a;
                unitAnimator.Play("Begin_fill");
                startFilling = false;
                startfill_t = 0;
                fill_t = 0;
                pix_t = 0;
                pixelAmount = 0;
                break;

            case Ability.builder:
                unitAnimator.Play("Bridge");
                curAbility = a;
                build_Count = 0;
                break;

            case Ability.explode:
                curAbility = a;
                unitAnimator.Play("Explode");
                explode_t = 0;
                break;

            case Ability.minigun:              
                if (previouslyGrounded)
                {
                    curAbility = a;
                    unitAnimator.Play("Miniguns_Minigun");
                    return true;
                } else
                {
                    return false;
                }

            case Ability.die:
                curAbility = a;
                unitAnimator.Play("Die");
                break;

            default:
                break;
        }

        return true;

    }

    IEnumerator Finish()
    {
        yield return new WaitForSecondsRealtime(0.45f);
        ReachEnd();
    }

    bool Pathfind()
    {
        if ( curNode == null)
        {
            targetPos = transform.position;
            targetPos.y = -50;
            previouslyGrounded = onGround;
            FindObjectOfType<UnitControl>().DeleteUnit(this);
            FindObjectOfType<UnitControl>().SetOutCount(-1);
            FindObjectOfType<UnitControl>().SetDeadCount(1);
            return false;
        }
        if ( curNode.isExit)
        {
            unitAnimator.Play("Finish");          
            hasLeft = true;           
            StartCoroutine(Finish());
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
                //Debug.Log(airLimit);
                if (airLimit > deathFallLimit && !isUmbrella)
                {
                    targetNode = curNode;
                    ChangeAbility(Ability.die);
                    unitAnimator.Play("Death_land");                                  
                    FindObjectOfType<UnitControl>().SetOutCount(-1);
                    FindObjectOfType<UnitControl>().SetDeadCount(1);
                    previouslyGrounded = onGround;
                    FindObjectOfType<UnitControl>().DeleteUnit(this);
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
        bool isAir = n.isEmpty;
        if (n.isFiller)
        {
            isAir = true;
        }
        return isAir;
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

    void Bouncer()
    {
        if (CheckNodeBelow() || CheckCurrentNode())
        {
            ClearStopNodes();
            FindObjectOfType<UnitControl>().SetOutCount(1);
        }
    }

    void Walker(float delta)
    {
        isGunning = false;

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

    bool CheckNodeBelow()
    {
        Node below = gameControl.GetNode(curNode.x, curNode.y - 2);

        if (below != null)
        {

            if (below.isEmpty)
            {
                ChangeAbility(Ability.walker);
                return true;
            }

        }
        else
        {
            ChangeAbility(Ability.walker);
            return true;
        }
        return false;
    }

    bool CheckCurrentNode()
    {
        

        if (curNode.isEmpty == false)
        {
            ChangeAbility(Ability.walker);
            return true;
        }

        return false;

    }

    void UseMinigun(float delta)
    {
        if (wep == null)
        {
            wep = this.GetComponent<Minigun>();
        }
        
        isGunning = false;
        fireTime = 5f;
        prepareToFire = 1f;      
        StartCoroutine(BeforeShoot(prepareToFire, fireTime, delta));
        
    }

    void Builder(float delta)
    {
        if (!initLerp)
        {
            build_t += delta;
           
            if ( build_t > build_Time)
            {
                
                build_t = 0;
                initLerp = true;
                bool interrupt = false;
                build_Count++;
              
                if (build_Count > buildAmount)
                {
                    interrupt = true;
                }
                
                int _tx = curNode.x;
                int _ty = curNode.y;
                _tx = (movingLeft) ? _tx - 1 : _tx + 1;
                _ty++;   

                startPos = transform.position;
                targetNode = gameControl.GetNode(_tx, _ty);     
                
                if ( targetNode.isEmpty == false || interrupt)
                {
                    ChangeAbility(Ability.walker);
                    return;
                }
                
                targetPos = gameControl.GetWorldPosFromNode(targetNode.x, targetNode.y);
                float dist = Vector3.Distance(startPos, targetPos);
                
                moveSpeed = build_Speed / dist;
                //Debug.Log(moveSpeed);
                
                

                List<Node> possibleNodes = new List<Node>();

                for (int i = -1; i < 6; i++)
                {
                    int size = _tx + i;
                    Node n = gameControl.GetNode(size, curNode.y);

                    if (n.isEmpty == true)
                    {
                        possibleNodes.Add(n);
                    } 
                }
                gameControl.AddPossibleNodesToSelection(possibleNodes);
            }
        }
        else
        {
            LerpIntoPosition(delta);
        }
    }

    void Filler(float delta)
    {
        if (!startFilling)
        {
            startfill_t += delta;

            if (startfill_t < startFill)
            {
                startFilling = true;
            }
        }

        if (pixelAmount > maxPixels)
        {
            ChangeAbility(Ability.walker);
            return;
        }

        pix_t += delta;

        if (pix_t > 0.05f)
        {
            pixelAmount++;
            pix_t = 0;
        }
        else
        {
            return;
        }

        int _x = (movingLeft) ? curNode.x - 3 : curNode.x + 3;
        int _y = curNode.y + 4;

        Node n = gameControl.GetNode(_x,_y);
        FillNode f = new FillNode();
        f.x = n.x;
        f.y = n.y;
        gameControl.AddFillNode(f);

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
   
            gameControl.AddNodePossibilitiesForRemoval(nodes);
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

            int t_x = (movingLeft) ? curNode.x - 2 : curNode.x + 2;

            Node originNode = gameControl.GetNode(t_x , t_y + 8);

            List<Node> nodes = CheckNode(originNode, 9);

            if (digFCount > 0 && (nodes.Count < 2 || digFCount > digFrontLimit))
            {
                ChangeAbility(Ability.walker);
                isDigForward = false;
                return;
            }
            digFCount++;

            gameControl.AddNodePossibilitiesForRemoval(nodes);
 
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

    void Explodes(float delta)
    {
        explode_t += delta;

        if (explode_t > explodeTimer)
        {
            ChangeAbility(Ability.explode);

            float radius = explodeRadius * 0.01f;
            int steps = Mathf.RoundToInt(explodeRadius);
            Vector3 center = transform.position;
            List<Node> nodes = new List<Node>();

            for (int x = -steps; x < steps; x++)
            {
                for (int y = -steps; y < steps; y++)
                {
                    int t_x = x + curNode.x;
                    int t_y = y + curNode.y;

                    float d = Vector3.Distance(center, gameControl.GetWorldPosFromNode(t_x, t_y));
                    if (d > radius)
                    {
                        continue;
                    }

                    Node n = gameControl.GetNode(t_x, t_y + 6);
                    if (n == null)
                    {
                        continue;
                    }

                    nodes.Add(n);
                }
            }
            gameControl.AddNodePossibilitiesForRemoval(nodes);
            FindObjectOfType<UnitControl>().DeleteUnit(this);
            FindObjectOfType<UnitControl>().SetOutCount(-1);
            FindObjectOfType<UnitControl>().SetDeadCount(1);
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
    
    private void OnCollisionEnter2D(Collision2D collision)
    {

        /*if (collision.collider.tag == ("endPoint"))
        {        
            unitAnimator.Play("Finish");
            EndPoint.FindObjectOfType<EndPoint>().Score();
            ReachEnd();
        }*/
        
        if (collision.collider.tag == ("enemy"))
        {
            StartCoroutine(Reaction(react));                 
            FindObjectOfType<UnitControl>().SetOutCount(-1);
            FindObjectOfType<UnitControl>().SetDeadCount(1);
        }

    }

    public void ReachEnd()
    {
        
        this.gameObject.SetActive(false);
        FindObjectOfType<UnitControl>().SetEscapeCount(1);
        FindObjectOfType<UnitControl>().DeleteUnit(this);
        
    }

    public float myXPos()
    {
        return this.transform.position.x;
    }

    public float myYPos()
    {
        return this.transform.position.y;
    }

    IEnumerator Reaction(float react)
    {
        yield return new WaitForSeconds(react);
        ChangeAbility(Ability.die);
        FindObjectOfType<UnitControl>().DeleteUnit(this);
        FindObjectOfType<UnitControl>().SetOutCount(-1);
        FindObjectOfType<UnitControl>().SetDeadCount(1);
        StartCoroutine(Dying(dieT));

    }

    IEnumerator Dying(float dieT)
    {
        yield return new WaitForSecondsRealtime(dieT);
        ImDead();
    }

    IEnumerator ShootTime(float firetime, float delta)
    {   
        
        unitAnimator.SetTrigger("isFiring");
        isGunning = true;
        if (isGunning)
        {
            wep.Fire(delta);
        }
        yield return new WaitForSeconds(fireTime);
        ChangeAbility(Ability.walker);

    }

    IEnumerator BeforeShoot(float prepToFire, float firetime ,float delta)
    {    
        
        yield return new WaitForSeconds(prepToFire);
        StartCoroutine(ShootTime(firetime, delta));
        
    }  


    public void ImDead()
    {
        this.imDead = true;
        this.gameObject.SetActive(false);
    }

    public bool AmDead()
    {
        return imDead;
    }
}
