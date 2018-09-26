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
	
	public void Tick (float d, float timeScale)
    {
        
        StartCoroutine(WaitStart(momentBeforeSpawning, timeScale));
        StartCoroutine(Enrage(timeToEnrage, timeScale));

        delta = d;


        if(changeSpeed)
        {
            changeSpeed = false;
            ChangeSpeedOfUnits(timeScale);
        }

        if (units.Count == maxUnits)
        {
            unitsSpawned = true;
        }

        if (outCount <= maxUnits/2 && escapeCount >= maxUnits/2)
        {
            wonGame = true;
            UiControl.singleton.winPopUp.SetActive(true);
            
            if (escapeCount == maxUnits)
            {
                UiControl.singleton.wintext.text = "PERFECT !";
                //perfect
            }
        }

        if (unitsSpawned)
        {
            Ulose();
        }
        
        if (units.Count < maxUnits)
        {
                   
            timer -= delta;
            if (timer < 0)
            {              
                timer = interval;
                SpawnUnit();
            }
        }

		for (int i = 0; i < units.Count; i++)
        {           
            units[i].Tick(delta);
        }
	}

    void SpawnUnit()
    {
        if (!unitsSpawned)
        {
            GameObject spawn = Instantiate(unitPrefab);
            spawn.transform.parent = unitsP.transform;
            Unit u = spawn.GetComponent<Unit>();
            u.Init(FindObjectOfType<GameControl>());
            units.Add(u);
            SetCount(1);
            u.move = true;
        }
    }
    
    void Ulose()
    {
        if (unitsSpawned && !wonGame && outCount == 0)
        {
            FindObjectOfType<UiControl>().losePopUp.SetActive(true);
        }
    }

    public void DeleteUnit(Unit curUnit)
    {
        units.Remove(curUnit);
    }
    
    void ChangeSpeedOfUnits(float t)
    {
        for (int i = 0; i < units.Count; i++)
        {
            units[i].unitAnimator.speed = t;
        }

    }

    public void ClearAll()
    {
        for (int i = 0; i < units.Count; i++)
        {
            Destroy(units[i].gameObject);
        }
        units.Clear();
    }

    public void SetCount(int count)
    {
        this.outCount += count;
       
        FindObjectOfType<UiControl>().ResetVellingCounter(outCount);
    }

    public void ResetCount()
    {
        this.outCount = 0;

        FindObjectOfType<UiControl>().ResetVellingCounter(outCount);
    }

    public int GetCount()
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

    public void ResetEscapeCount()
    {
        this.escapeCount = 0;
        FindObjectOfType<UiControl>().ResetVellingEscape(escapeCount);
    }
    
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
public enum Ability
{
    walker, bouncer, umbrella, dig_forward, dig_down, explode, builder, filler, die, minigun, speedy
}
