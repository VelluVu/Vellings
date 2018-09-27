using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitControl : MonoBehaviour {

    public bool changeSpeed;
    public bool unitsSpawned;
    public bool wonGame;

    float delta;
    float timer;
    float interval;
    float momentBeforeSpawning;
    float timeToEnrage;

    int spawnCount;
    int escapeCount;
    int maxUnits;
    int outCount;
    int deadCount;

    public GameObject unitPrefab;

    GameObject unitsP;
    List<Unit> units = new List<Unit>();
  
    public static UnitControl singleton;

    private void Awake()
    {

        singleton = this;
        
    }
    
    void Start ()
    {
        wonGame = false;
        unitsSpawned = false;
        outCount = 0;
        escapeCount = 0;
        interval = 2f;
        timeToEnrage = 500f;
        maxUnits = 10;    
        momentBeforeSpawning = 1.5f;

        unitsP = new GameObject();
        unitsP.name = "units parent";

    }

    //Custom update for unitcontrol and each unit, passing same deltatime for each unit bottom of the function
	
	public void Tick (float d, float timeScale)
    {

        StartCoroutine(WaitStart(momentBeforeSpawning, timeScale));
        StartCoroutine(Enrage(timeToEnrage, timeScale));

        delta = d;

        if (changeSpeed)
        {
            changeSpeed = false;
            ChangeSpeedOfUnits(timeScale);
        }

        if (spawnCount == maxUnits)
        {
            unitsSpawned = true;
        }

        WinConditions();

        Ulose();

        SpawnTime();

        for (int i = 0; i < units.Count; i++)
        {
            units[i].Tick(delta);
        }
    }

    private void SpawnTime()
    {
        if (units.Count < maxUnits && deadCount <= maxUnits)
        {

            timer -= delta;
            if (timer < 0)
            {
                timer = interval;
                SpawnUnit();
            }
        }
    }

    private void WinConditions()
    {
        if (unitsSpawned && outCount == 0 && escapeCount >= maxUnits * 0.5f)
        {
            wonGame = true;
            UiControl.singleton.winPopUp.SetActive(true);

            if (escapeCount == maxUnits)
            {

                UiControl.singleton.wintext.text = "100% PERFECT !";
                //perfect

            }
            else if (escapeCount >= maxUnits * 0.75f && deadCount <= maxUnits * 0.25f)
            {

                UiControl.singleton.wintext.text = "75% Escaped!";
            }
        }
    }

    //Creates one Velling

    void SpawnUnit()
    {
        if (!unitsSpawned)
        {
            GameObject spawn = Instantiate(unitPrefab);
            spawn.transform.parent = unitsP.transform;
            Unit u = spawn.GetComponent<Unit>();
            u.Init(FindObjectOfType<GameControl>());
            units.Add(u);
            SetOutCount(1);
            spawnCount++;
            u.move = true;
        }
    }
    
    //Lose Window pops up

    void Ulose()
    {
        if (deadCount == maxUnits || unitsSpawned && outCount == 0)
        {
            FindObjectOfType<UiControl>().losePopUp.SetActive(true);
        }
    }

    //Delete current unit

    public void DeleteUnit(Unit curUnit)
    {
        units.Remove(curUnit);
    }
    
    //Change speed for all units

    void ChangeSpeedOfUnits(float t)
    {
        for (int i = 0; i < units.Count; i++)
        {
            units[i].unitAnimator.speed = t;
        }

    }

    //Clear all units

    public void ClearAll()
    {
        for (int i = 0; i < units.Count; i++)
        {
            Destroy(units[i].gameObject);
        }
        units.Clear();
    }

    //These functions Manipulate various counter values

    public void SetOutCount(int count)
    {
        this.outCount += count;
       
        FindObjectOfType<UiControl>().ResetVellingCounter(outCount);
    }

    public void ResetÖutCount()
    {
        this.outCount = 0;

        FindObjectOfType<UiControl>().ResetVellingCounter(outCount);
    }

    public int GetOutCount()
    {
        return this.outCount;
    }

    public void SetDeadCount(int count)
    {
        this.deadCount += count;

        FindObjectOfType<UiControl>().ResetVellingDead(deadCount);
    }

    public void ResetDeadCount()
    {
        this.deadCount = 0;

        FindObjectOfType<UiControl>().ResetVellingDead(deadCount);
    }

    public int GetDeadCount()
    {
        return this.deadCount;
    }

    public void SetMaxUnits(int units)
    {
        this.maxUnits = units;
    }

    public void SetEscapeCount(int ec)
    {
        this.escapeCount += ec;
        this.outCount -= 1;
    
        FindObjectOfType<UiControl>().ResetVellingEscape(escapeCount);
        FindObjectOfType<UiControl>().ResetVellingCounter(outCount);
    }

    public void ResetSpawnCount()
    {
        this.spawnCount = 0;
    }

    public void ResetEscapeCount()
    {
        this.escapeCount = 0;
        FindObjectOfType<UiControl>().ResetVellingEscape(escapeCount);
    }

    //Function to reset all counter values

    public void ResetMapCounters()
    {
        unitsSpawned = false;
        this.escapeCount = 0;
        this.spawnCount = 0;
        this.deadCount = 0;
        this.outCount = 0;

        FindObjectOfType<UiControl>().ResetVellingCounter(outCount);
        FindObjectOfType<UiControl>().ResetVellingDead(deadCount);
        FindObjectOfType<UiControl>().ResetVellingEscape(escapeCount);
    }

    //Time functions "cooldowns"

    IEnumerator WaitStart(float momentBeforeSpawning,float timeScale)
    {
        timeScale = 0;
        yield return new WaitForSeconds(momentBeforeSpawning);
        //Debug.Log("YOU WAITED" + momentBeforeSpawning);
        timeScale = 1f;
    }

    IEnumerator Enrage(float timeToEnrage, float timeScale)
    {
        yield return new WaitForSecondsRealtime(timeToEnrage);
        Debug.Log("Enrage");
        timeScale = 5f;
    }

    //Function to find closest unit to select point

    public Unit GetClosest(Vector3 origin)
    {
        float minDis = 0.1f;
        Unit r = null;

        for (int i = 0; i < units.Count; i++)
        {
            float tempDistance = Vector3.Distance(origin, units[i].transform.position);
            if(tempDistance < minDis)
            {
                minDis = tempDistance;
                r = units[i];
            }
        }
        return r;
    }   
}

//Unit abilities

public enum Ability
{
    walker, bouncer, umbrella, dig_forward, dig_down, explode, builder, filler, die, minigun, speedy
}
