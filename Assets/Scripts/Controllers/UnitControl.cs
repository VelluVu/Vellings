using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitControl : MonoBehaviour {

    public bool changeSpeed;

    float delta;
    float timer;
    float maxUnits = 10f;
    float timeScale = 1.2f;
    float Interval = 3f;
    public GameObject unitPrefab;

    GameObject unitsP;
    List<Unit> units = new List<Unit>();

    public static UnitControl singleton;

    private void Awake()
    {
        singleton = this;
    }

    void Start () {
        unitsP = new GameObject();
        unitsP.name = "units parent";
	}
	
	void Update () {

        delta = Time.deltaTime;
        delta *= timeScale;

        if(changeSpeed)
        {
            changeSpeed = false;
            ChangeSpeedOfUnits(timeScale);
        }

        if (units.Count < maxUnits)
        {


            timer -= delta;
            if (timer < 0)
            {
                timer = Interval;
                SpawnUnit();
            }
        }

		for (int i = 0; i < units.Count; i++)
        {
            units[i].FrameTick(delta);
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

    void ChangeSpeedOfUnits(float t)
    {
        for (int i = 0; i < units.Count; i++)
        {
            units[i].unitAnimator.speed = t;
        }

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
    walker, bouncer, umbrella, dig_forward, dig_down, explode, builder, filler, die
}
