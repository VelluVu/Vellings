using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minigun : MonoBehaviour {

    public Transform shootPoint;
    public GameObject bulletPrefab;
    float timer;
    float rateOfFire;
    float burst;
    
   
    private void Start()
    {
        rateOfFire = 0.4f;
        burst = 10;    
    }
    
    public void Fire(float delta)
    {
        
        timer -= delta;
        for (int i = 0; i < burst; i++)
        {
            if (timer < 0)
            {
                timer = rateOfFire;
                Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);

            }
        }
    }
    

}
