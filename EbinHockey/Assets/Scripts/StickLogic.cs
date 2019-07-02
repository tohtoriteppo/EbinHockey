using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickLogic : MonoBehaviour
{

    private float spinSpeed = 0f;
    private Rigidbody2D rb;
    private float accelerationSpeed = 0.3f;
    private float maxSpeed = 20f;
    private float collisionFactor = 50f;
    private bool canMove = false;
    public int playerNum = 1;
    // Start is called before the first frame update
    void Start()
    {
        Physics2D.IgnoreCollision(transform.parent.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 targetPos = transform.parent.position;
        Vector3 offset = targetPos - transform.position;
        rb.velocity = offset * 40;
        if (canMove)
        {
            if (Input.GetButton("p"+playerNum+"_trigger_left"))
            {
                //spinSpeed = spinSpeed + accelerationSpeed > maxSpeed ? maxSpeed : spinSpeed + accelerationSpeed;
                rb.AddTorque(accelerationSpeed * rb.mass);
            }
            if (Input.GetButton("p" + playerNum + "_trigger_right"))
            {
                //spinSpeed = spinSpeed - accelerationSpeed < -maxSpeed ? -maxSpeed : spinSpeed - accelerationSpeed;
                rb.AddTorque(-accelerationSpeed * rb.mass);
            }
            //transform.RotateAround(transform.parent.position, new Vector3(0, 0, 1), spinSpeed);
            
        }
        if (Mathf.Abs(rb.angularVelocity) > maxSpeed) rb.angularVelocity = rb.angularVelocity > 0 ? maxSpeed : -maxSpeed;

        //Debug.Log(rb.angularVelocity);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == "Puck")
        {
            //collision.collider.GetComponent<Rigidbody2D>().AddForce((collision.collider.transform.position - transform.position) * Mathf.Abs(spinSpeed * collisionFactor));
        }
    }

    public void DisableStick()
    {
        canMove = false;
        transform.rotation = Quaternion.identity;
        rb.angularVelocity = 0;
    }

    public void ActivateStick()
    {
        canMove = true;
    }
    public void SetMaxSpeed(float value)
    {
        maxSpeed = value;
    }
    public void SetAcceleration(float value)
    {
        accelerationSpeed = value;
    }
}
