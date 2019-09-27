using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleController : MonoBehaviour
{
    private GameController gameController;
    private PlayerController[] players;
    private GameObject startPositions;
    private GameObject titleScreen;
    private TeamController teamController1;
    private TeamController teamController2;
    private bool gameOn = false;
    private int current = 0;
    private GameObject puck;
    
    private bool[] playerPressed;
    private float[] playerHoldTime;
    private float holdTreshold = 0.5f;
    private Transform valueObject;
    private Transform cursor;
    
    [SerializeField] string[] valueTexts;
    [SerializeField] float[] values;
    private float[] valueAdditions;
    private float[] valueFactors;
    private JoyController joyController;

    // Start is called before the first frame update
    void Awake()
    {
        gameController = FindObjectOfType<GameController>();
        players = FindObjectsOfType<PlayerController>();
        startPositions = GameObject.Find("TitlePositions");
        titleScreen = GameObject.Find("TitleScreen");
        teamController1 = GameObject.Find("TeamController1").GetComponent<TeamController>();
        teamController2 = GameObject.Find("TeamController2").GetComponent<TeamController>();
        puck = GameObject.Find("puck");
        valueObject = GameObject.Find("Values").transform;
        cursor = GameObject.Find("Cursor").transform;
        playerPressed = new bool[] { false, false, false, false };
        playerHoldTime = new float[] { 0, 0, 0, 0 };
        valueAdditions = new float[] { 1, 10, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f };
        valueFactors = new float[] { 3, 60, 0.5f, 2000, 1, 500, 1.1f, 1, 0.5f, 0.3f, 1 };
        joyController = FindObjectOfType<JoyController>();
        SetDefaults();
        UpdateValues(1);

    }
    private void Start()
    {
        SetTeams();
        teamController1.RandomColor();
        teamController2.RandomColor();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameOn)
        {
            for (int i = 1; i < 5; i++)
            {
                if (Input.GetButtonDown("p" + i + "_button_x"))
                {
                    ShuffleTeams();
                }
            }
            for (int i = 1; i < 5; i++)
            {
                if (joyController.GetTackle(i))
                {
                    StartGame();
                }
            }
            for (int i = 1; i < 5; i++)
            {
                if (Input.GetButtonDown("p" + i + "_button_b"))
                {
                    RandomColor(i);
                }
            }
            for (int i = 1; i < 5; i++)
            {
                if (Mathf.Abs(joyController.GetLHorizontal(i)) < 0.1 && Mathf.Abs(joyController.GetLVertical(i)) < 0.1)
                {
                    playerHoldTime[i-1] = 0;
                    playerPressed[i-1] = false;
                }
                else
                { 
                    
                    playerHoldTime[i-1] += Time.deltaTime;
                    if (playerHoldTime[i-1] > holdTreshold)
                    {
                        UpdateValues(i);
                    }
                    else if(!playerPressed[i-1])
                    {
                        playerPressed[i-1] = true;
                        UpdateValues(i);
                    }

                }
            }
        }
        
    }


    private void UpdateValues(int i)
    {
        float val = joyController.GetLVertical(i);
        if(Mathf.Abs(val) >= 0.1f)
        {
            val = val < 0 ? -1 : 1;
        }
        current -= (int)val;
        if (current > values.Length - 1) current = 0;
        if (current < 0) current = values.Length - 1;
        float hor = joyController.GetLHorizontal(i);
        if (Mathf.Abs(hor) >= 0.1f)
        {
            hor = hor < 0 ? -1 : 1;
        }
        values[current] += (int)hor * valueAdditions[current];
        if(current<2) valueObject.GetChild(current).GetComponent<Text>().text = ((int)values[current]).ToString();
        else valueObject.GetChild(current).GetComponent<Text>().text = values[current].ToString("F2");
        Vector3 pos = cursor.position;
        cursor.position = new Vector3(pos.x, valueObject.GetChild(current).transform.position.y, pos.z);
    }
    private void RandomColor(int num)
    {

        foreach(PlayerController player in players)
        {
            if(player.playerNum == num)
            {
                if(teamController1.FindPlayer(player))
                {
                    teamController1.RandomColor();
                }
                else
                {
                    
                    teamController2.RandomColor();
                }
            }
        }
    }
    private void ShuffleTeams()
    {
        RandomizeArray(players);
        for(int i = 0; i < startPositions.transform.childCount; i++)
        {
            Transform position = startPositions.transform.GetChild(i);
            players[i].transform.position = position.position;
        }
        SetTeams();
    }

    private void SetTeams()
    {
        foreach(PlayerController player in players)
        {
            if (player.transform.position.x < 0)
            {
                teamController1.AddPlayer(player);
                teamController2.RemovePlayer(player);
                
            }
            else
            {
                teamController2.AddPlayer(player);
                teamController1.RemovePlayer(player);
                
            }
            teamController1.UpdateColor();
            teamController2.UpdateColor();
        }
        
    }
    private void StartGame()
    {
        gameOn = true;
        gameController.StartGame();
        gameController.PeriodAmount = (int)values[0];
        gameController.SetPeriodTime((int)values[1]);
        foreach (PlayerController player in players)
        { 
            player.SetMaxSpeed(values[2]*valueFactors[2]);
            player.SetAcceleration(values[3] * valueFactors[3]);
            player.SetMass(values[4] * valueFactors[4]);
            player.SetStickMaxSpeed(values[5] * valueFactors[5]);
            player.SetStickAcceleration(values[6] * valueFactors[6]);
            player.SetStickMass(values[7] * valueFactors[7]);
            
        }
        puck.GetComponent<Rigidbody2D>().sharedMaterial.bounciness = values[8] * valueFactors[8];
        puck.GetComponent<Rigidbody2D>().drag = values[9] * valueFactors[9];
        puck.GetComponent<Rigidbody2D>().mass = values[10] * valueFactors[10];
        titleScreen.SetActive(false);
    }
    private void RandomizeArray(PlayerController[] arr)
    {
        for (var i = arr.Length - 1; i > 0; i--)
        {
            var r = Random.Range(0, i);
            var tmp = arr[i];
            arr[i] = arr[r];
            arr[r] = tmp;
        }
    }
    public void TitleScreen()
    {
        gameOn = false;
        titleScreen.SetActive(true);
        SetTeams();
    }
    private void SetDefaults()
    {
        
        //values = new float[] { periodAmount, periodTime, playerSpeed, stickSpeed, stickAcceleration, puckFriction, puckBounciness, stickWeight, playerWeight};
        //valueAdditions = new float[] { 1f, 10f, 10f, 10f, 1f, 0.01f, 0.01f, 0.1f, 0.1f};
        //valueTexts = new string[] { "Period amount", "Period Time", "Player speed", "Stick Max Speed", "Stick acceleration", "Puck friction", "Puck bouncyness", "Stick Weight", "Player Weight"};

        float height = valueObject.GetComponent<RectTransform>().rect.size.y;
        float zeroPos = height * 0.5f;
        float spacePerValue = height / values.Length;
        for (int i = 0; i < values.Length; i++)
        {
            GameObject obj = Instantiate(Resources.Load<GameObject>("Value"), valueObject);
            obj.transform.GetChild(0).GetComponent<Text>().text = valueTexts[i];
            Vector3 pos = obj.transform.localPosition;
            float y = zeroPos - spacePerValue * i - spacePerValue * 0.5f;
            valueObject.GetChild(i).transform.localPosition = new Vector3(pos.x, y, pos.z);
        }
        for(int i = 0; i < values.Length; i++)
        {
            if (i < 2) valueObject.GetChild(i).GetComponent<Text>().text = ((int)values[i]).ToString();
            else valueObject.GetChild(i).GetComponent<Text>().text = values[i].ToString("F2");
        }
    }
}
