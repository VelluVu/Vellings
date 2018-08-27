using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitControl : MonoBehaviour {

    public bool changeSpeed;

    float delta;
    float timer;
    public float maxUnits = 10;
    public float timeScale = 1;
    public float Interval = 1;
    public GameObject unitPrefab;

    GameObject unitsP;
    List<Unit> units = new List<Unit>();

	
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
                SpawnLenny();
            }
        }

		for (int i = 0; i < units.Count; i++)
        {
            units[i].FrameTick(delta);
        }
	}
    void SpawnLenny()
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
}
