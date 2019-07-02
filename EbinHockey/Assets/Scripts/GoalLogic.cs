using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalLogic : MonoBehaviour
{
    private bool team1;
    private GameController gameController;
    // Start is called before the first frame update
    void Start()
    {
        gameController = FindObjectOfType<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Puck")
        {
            gameController.Scored(!team1);
        }
    }
    public void SetTeam(bool isteam1)
    {
        team1 = isteam1;
    }
    public void SwapTeam()
    {
        team1 = !team1;
    }
}
