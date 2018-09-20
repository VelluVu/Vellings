using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawn : MonoBehaviour {

    public Animator spawnAnim;
    // Use this for initialization

    
    void Start () {
        spawnAnim = GetComponent<Animator>();
        spawnAnim.SetBool("spawnNow", true);
	}
	
	// Update is called once per frame
	void Update () {
	    if (UnitControl.FindObjectOfType<UnitControl>().unitsSpawned)
        {
            spawnAnim.SetBool("spawnNow", false);
        }	
	}

}
