using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPoint : MonoBehaviour {

    int scoreCount;

    private void Start()
    {
        scoreCount = 0;
    }

    public void Score()
    {
                
            scoreCount++;
            Debug.Log("Wohoo " + scoreCount);

    }

    public void ReduceScore()
    {

        scoreCount--;
        Debug.Log("crap" + scoreCount);

    }
}
