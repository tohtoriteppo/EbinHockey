using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuckLogic : MonoBehaviour
{
    private GameController controller;
    private List<int> stack;
    // Start is called before the first frame update
    void Start()
    {
        stack = new List<int>();
        controller = FindObjectOfType<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Out")
        {
            controller.Out();
        }
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Stick")
        {
            int toucher = collision.collider.GetComponent<StickLogic>().playerNum;

            stack.Remove(toucher);
            stack.Add(toucher);

        }
    }
    public List<int> GetGoalMaker()
    {
        return stack;
    }
    public void EmptyStack()
    {
        stack = new List<int>();
    }
}
