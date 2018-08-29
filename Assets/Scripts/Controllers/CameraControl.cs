using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

    public float camMoveSpeed = 0.01f;
    public Transform camTrans;

    public float minX;
    public float minY;
    
    public float maxX;
    public float maxY;
    
    Camera cam;
    Vector3 moveCam;

	void Start () {
		
	}
	
	
	void Update () {

        float xAxis = Input.GetAxis("Horizontal");
        moveCam = Vector3.zero;
        float yAxis = Input.GetAxis("Vertical");
        moveCam.x = xAxis * camMoveSpeed;
        moveCam.y = yAxis * camMoveSpeed;

        Vector3 targetPosition = camTrans.position;
        targetPosition += moveCam;

        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX + 0.01f);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY + 0.01f);

        camTrans.position = targetPosition;
    }
}
