using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    private Transform puck;
    public float smoothTime = 0.3f;
    public float lockDistance = 6f;
    private Vector3 velocity = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        puck = GameObject.Find("puck").transform;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = new Vector3(puck.position.x, transform.position.y, transform.position.z);
        float target = puck.position.x > lockDistance ? lockDistance : puck.position.x;
        target = target < -lockDistance ? -lockDistance : target;
        Vector3 targetPosition = new Vector3(target, transform.position.y, transform.position.z);

        // Smoothly move the camera towards that target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
