
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;


public class PlayerController : MonoBehaviour
{
    private float maxSpeed = 500f;
    private float accelerationSpeed = 1f;
    private bool canMove = false;
    public int playerNum = 1;
    public Sprite morkoColor;
    public Sprite morkoNonColor;

    private Sprite originalSpriteColor;
    private Sprite originalSpriteNonColor;
    private float offSet = 60f;
    private GameObject pText;
    private Vector3 startPosition;
    private StickLogic stickLogic;
    private Rigidbody2D rb;
    private float tackleLength = 0.1f;
    private float tackleCD = 1f;
    private float tackleSpeed;
    private bool canTackle = true;
    public bool tackling = false;
    private float slowSpeed;
    private float tackleMass;
    private float originalMass;
    private float originalBouncyness;
    private float originalDrag;
    private Color originalColor;
    private Color tackleColor;
    public int side;

    // Start is called before the first frame update
    void Awake()
    {
        originalSpriteColor = GetComponent<SpriteRenderer>().sprite;
        originalSpriteNonColor = transform.GetChild(1).GetComponent<SpriteRenderer>().sprite;
        pText = Instantiate(Resources.Load<GameObject>("PText"), GameObject.Find("GameScreen").transform);
        pText.GetComponent<Text>().text = "P" + playerNum.ToString();
        startPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
        stickLogic = transform.GetChild(0).GetComponent<StickLogic>();
        stickLogic.playerNum = playerNum;
        originalDrag = rb.drag;
        
    }



    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
        pText.transform.position = new Vector3(pos.x, pos.y + offSet, pos.z);
        if (canMove)
        {
            float horizontal = Input.GetAxis("p" + playerNum + "_joystick_horizontal");
            float vertical = Input.GetAxis("p" + playerNum + "_joystick_vertical");
            horizontal = horizontal * Time.deltaTime;
            vertical = vertical * Time.deltaTime;
            Vector2 velocity = new Vector2(horizontal * accelerationSpeed, vertical * accelerationSpeed);
            if(rb.velocity.magnitude>5)
            {
                Debug.Log("player " + playerNum + " velo " + rb.velocity.magnitude);
            }
            Move(velocity);
            //Debug.Log("after " + rb.velocity.magnitude);

            if (canTackle && (Input.GetButtonDown("p" + playerNum + "_button_a") || Input.GetButtonDown("p" + playerNum + "_button_x")))
            {
                StartCoroutine(Tackle());
            }
            if (rb.velocity.magnitude > maxSpeed)
            {
                //Debug.Log("velo " + playerNum +" " + rb.velocity);
                //Vector2 slow = new Vector2(slowSpeed, slowSpeed) * -rb.velocity;
                //Debug.Log("Slow " + playerNum+ " " +slow);
                //rb.AddForce(slow * (float)rb.mass);
                rb.drag = originalDrag + Mathf.Min(rb.velocity.magnitude, originalDrag);
            }
            else
            {
                rb.drag = Mathf.Max(rb.velocity.magnitude, originalDrag);
            }
        }

    }
    private IEnumerator Tackle()
    {
        rb.mass = tackleMass;
        rb.sharedMaterial.bounciness = 100f;
        canTackle = false;
        tackling = true;
        GetComponent<SpriteRenderer>().color = tackleColor;
        yield return new WaitForSeconds(tackleLength);
        GetComponent<SpriteRenderer>().color = originalColor;
        rb.mass = originalMass;
        rb.sharedMaterial.bounciness = originalBouncyness;
        tackling = false;
        yield return new WaitForSeconds(tackleCD);
        canTackle = true;
    }
    public void Move(Vector2 force)
    {
        
        if (tackling)
        {
            
            rb.velocity = rb.velocity.normalized * tackleSpeed;
        }
        else
        {

            rb.AddForce(force * rb.mass);

        }

    }
    public void ResetPosition()
    {
        transform.position = startPosition;
    }

    public void ResetPlayer()
    {
        //ResetPosition();
        canMove = false;
        rb.velocity = Vector2.zero;
        stickLogic.DisableStick();
    }
    public void SetColor(Color color)
    {
        GetComponent<SpriteRenderer>().color = color;
        pText.GetComponent<Text>().color = new Color(color.r, color.g, color.b, 0.7f);
        originalColor = color;
        //tackleColor = new Color(color.r-0.2f, color.g - 0.2f, color.b - 0.2f, 1f);
        tackleColor = new Color(color.r+0.3f, color.g + 0.3f, color.b + 0.3f, 1f);
        //Morko();
        //tackleColor = new Color(1, 1, 1, 1f);
    }
    public void ActivatePlayer()
    {
        canMove = true;
        stickLogic.ActivateStick();
    }

    public void UpdateColor(bool team1)
    {
        if (team1)
        {
            GetComponent<SpriteRenderer>().color = new Color(0, 0, 1);
            pText.GetComponent<Text>().color = new Color(0, 0, 1, 0.7f);
        }
        else
        {
            GetComponent<SpriteRenderer>().color = new Color(1, 0, 0);
            pText.GetComponent<Text>().color = new Color(1, 0, 0, 0.7f);
        }
    }
    public void SetMaxSpeed(float value)
    {
        maxSpeed = value;
        tackleSpeed = maxSpeed * 25f;
        Debug.Log("tackle speed " + tackleSpeed);
        slowSpeed = maxSpeed * 1f;
    }
    public void SetAcceleration(float value)
    {
        accelerationSpeed = value;
    }
    public void SetStickMaxSpeed(float value)
    {
        stickLogic.SetMaxSpeed(value);
    }
    public void SetStickAcceleration(float value)
    {
        stickLogic.SetAcceleration(value);
    }
    public void SetStickMass(float value)
    {
        stickLogic.GetComponent<Rigidbody2D>().mass = value;
    }
    public void SetMass(float value)
    {
        originalMass = value;
        rb.mass = originalMass;
        tackleMass = originalMass * 1f;
        originalBouncyness = rb.sharedMaterial.bounciness;
    }
    public void Morko()
    {
        Color color = GetComponent<SpriteRenderer>().color;
        GetComponent<SpriteRenderer>().sprite = morkoColor;
        GetComponent<SpriteRenderer>().color = color;
        transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = morkoNonColor;
    }
    public void ResetMorko()
    {
        Color color = GetComponent<SpriteRenderer>().color;
        GetComponent<SpriteRenderer>().sprite = originalSpriteColor;
        GetComponent<SpriteRenderer>().color = color;
        transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = originalSpriteNonColor;
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        float factor = 260f;
        if(collision.collider.tag == "Player" && tackling)
        {
            Rigidbody2D other = collision.collider.GetComponent<Rigidbody2D>();
            other.AddForce((other.position - rb.position).normalized * tackleMass * factor);
        }
        else if(collision.collider.tag == "Player" && collision.collider.GetComponent<PlayerController>().tackling)
        {
            Rigidbody2D other = collision.collider.GetComponent<Rigidbody2D>();
            rb.AddForce((rb.position - other.position).normalized * tackleMass * factor);
        }
        
    }
}

