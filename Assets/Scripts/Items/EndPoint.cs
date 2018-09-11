using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPoint : MonoBehaviour {

    int scoreCount;

    private void Start()
    {
        scoreCount = 0;
    }

    private void Score()
    {
            Debug.Log("Wohoo");
            Unit.FindObjectOfType<Unit>().ReachEnd();
            scoreCount++;    
    }
}
