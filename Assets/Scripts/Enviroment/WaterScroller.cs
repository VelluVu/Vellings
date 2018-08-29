using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterScroller : MonoBehaviour {

    public float min;
    public float max;
    public float speed = 0.01f;

	void Update () {
        Vector3 tp = transform.localPosition;
        tp.x += speed;
        
        if (tp.x > max)
        {
            tp.x = min;
        }
        if (tp.x < min)
        {
            tp.x = max;
        }
        transform.localPosition = tp;
	}
}
