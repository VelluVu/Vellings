using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour {
/* 
    int enemyMaxUnits;
    float enemyTimer;
    float enemyDelta;
    float enemyInterval;
    float enemyTimeScale;
    public bool enemyUnitsSpawned;
    public bool enemyChangeSpeed;

    public GameObject enemyUnitPrefab;
    GameObject enemyUnitsP;
    List<Enemy> enemyUnits = new List<Enemy>();

    public static EnemyControl singleton;

    private void Awake()
    {

        singleton = this;
        
    }

    void Start ()
    {
        enemyUnitsSpawned = false;
        enemyInterval = 2f;
        enemyMaxUnits = 2;
        enemyUnitsP = new GameObject();
        enemyUnitsP.name = "enemyUnits parent";
    }

    public void Tick()
    {

        enemyDelta = Time.deltaTime;
        enemyDelta *= enemyTimeScale;

        if (enemyChangeSpeed)
        {
            enemyChangeSpeed = false;
            ChangeSpeedOfEnemyUnits(enemyTimeScale);
        }

        if (enemyUnits.Count == enemyMaxUnits)
        {          
            enemyUnitsSpawned = true;
        }

        if (enemyUnits.Count < enemyMaxUnits)
        {

            StartCoroutine(EnemySpawnCD());
        }

        for (int i = 0; i < enemyUnits.Count; i++)
        {
            enemyUnits[i].Tick(enemyDelta);
        }
    }

    IEnumerator EnemySpawnCD()
    {

        EnemySpawnUnit();
        yield return new WaitForSeconds(enemyInterval);
        

    }

    void EnemySpawnUnit()
    {
        GameObject enemySpawn = Instantiate(enemyUnitPrefab);
        enemySpawn.transform.parent = enemyUnitsP.transform;
        Enemy e = enemySpawn.GetComponent<Enemy>();
        e.Init(FindObjectOfType<GameControl>());
        enemyUnits.Add(e);
        e.enemyMove = true;
    }

    void ChangeSpeedOfEnemyUnits(float t)
    {
        for (int i = 0; i < enemyUnits.Count; i++)
        {
            enemyUnits[i].enemyAnim.speed = t;
        }

    }

    public void DeleteEnemyUnit(Enemy curEnemy)
    {
        enemyUnits.Remove(curEnemy);
        enemyMaxUnits--;
    }

    public void ClearAllEnemy()
    {     
        for (int i = 0; i < enemyUnits.Count; i++)
        {
            Destroy(enemyUnits[i].gameObject);
        }
        enemyUnits.Clear();
    }*/
}
