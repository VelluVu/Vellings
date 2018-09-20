﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitControl : MonoBehaviour {

    public bool changeSpeed;
    public bool unitsSpawned;
    
    float delta;
    float timer;
    float timeScale;
    float Interval;
    float momentBeforeSpawning;
    float timeToEnrage;

    int maxUnits;
    int count;

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
        unitsSpawned = false;
        count = 0;
        Interval = 2f;
        timeToEnrage = 500f;
        maxUnits = 10;    
        momentBeforeSpawning = 1.5f;

        unitsP = new GameObject();
        unitsP.name = "units parent";

        StartCoroutine(WaitStart(momentBeforeSpawning));
        StartCoroutine(Enrage(timeToEnrage));

    }
	
	public void Tick ()
    {
        
        delta = Time.deltaTime;
        delta *= timeScale;

        if(changeSpeed)
        {
            changeSpeed = false;
            ChangeSpeedOfUnits(timeScale);
        }
        
        if (units.Count == maxUnits)
        {           
            count = 0;
            unitsSpawned = true;
        }
        
        if (units.Count < maxUnits)
        {
            
            
            timer -= delta;
            if (timer < 0)
            {              
                timer = Interval;
                SpawnUnit();
                count++;
                UiControl.FindObjectOfType<UiControl>().IncrementVellingCounter(count);
            }
        }

		for (int i = 0; i < units.Count; i++)
        {           
            units[i].Tick(delta);
        }
	}

    void SpawnUnit()
    {
        
        GameObject spawn = Instantiate(unitPrefab);
        spawn.transform.parent = unitsP.transform;
        Unit u = spawn.GetComponent<Unit>();
        u.Init(FindObjectOfType<GameControl>());
        units.Add(u);
        u.move = true;
    }

    public void DeleteUnit(Unit curUnit)
    {
        units.Remove(curUnit);
        maxUnits--;
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
        this.count = count;
        UiControl.FindObjectOfType<UiControl>().ResetVellingCounter(this.count);
    }
    
    public int GetCount()
    {
        return this.count;
    }

    IEnumerator WaitStart(float momentBeforeSpawning)
    {
        timeScale = 0;
        yield return new WaitForSeconds(momentBeforeSpawning);
        //Debug.Log("YOU WAITED" + momentBeforeSpawning);
        timeScale = 1f;
    }

    IEnumerator Enrage(float timeToEnrage)
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
    walker, bouncer, umbrella, dig_forward, dig_down, explode, builder, filler, die, minigun
}
