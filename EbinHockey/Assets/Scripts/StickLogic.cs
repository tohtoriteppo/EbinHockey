using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickLogic : MonoBehaviour
{

    private float spinSpeed = 0f;
    private Rigidbody2D rb;
    private float accelerationSpeed = 15f;
    private float maxSpeed = 20f;
    private float collisionFactor = 50f;
    private bool canMove = false;
    public int playerNum = 1;
    private JoyController joyController;
    private PIDController pidController;
    // Start is called before the first frame update
    void Start()
    {
        Physics2D.IgnoreCollision(transform.parent.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        rb = GetComponent<Rigidbody2D>();
        joyController = FindObjectOfType<JoyController>();
        pidController = new PIDController();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 targetPos = transform.parent.position;
        Vector3 offset = targetPos - transform.position;
        rb.velocity = offset * 40;
        if (canMove)
        {
            if(joyController.IsPsMode(playerNum))
            {
                float horizontal = Input.GetAxis("ps" + playerNum + "_right_horizontal");
                float vertical = Input.GetAxis("ps" + playerNum + "_right_vertical");
                if (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f)
                {
                    float target = Mathf.Atan2(horizontal, vertical) * Mathf.Rad2Deg + 180;
                    float current = transform.eulerAngles.z - 90;
                    if (current < 0) current += 360;
                    //angle between
                    float angle = target - current;
                    if (angle < -180) angle += 360;
                    //shortest angle, i.e. abs(angle) < 180
                    float minAngle = (angle + 180) % 360 - 180;
                    //get direction to rotate
                    float dir = pidController.Update(minAngle);
                    if(dir == 0)
                    {
                        rb.AddTorque(-accelerationSpeed * rb.mass);
                    }
                    else
                    {
                        rb.AddTorque(accelerationSpeed * rb.mass);
                    }
                }
            }
            else
            {
                if (Input.GetButton("p" + playerNum + "_trigger_left"))
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
        }
        if (Mathf.Abs(rb.angularVelocity) > maxSpeed)
        {
            rb.angularVelocity = rb.angularVelocity > 0 ? maxSpeed : -maxSpeed;
            
        }
        Debug.Log("accelerationSpeed "+accelerationSpeed);
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
