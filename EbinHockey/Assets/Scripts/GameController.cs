using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public float startDelay = 2f;
    public float goalDelay = 3f;
    public int PeriodTime = 30;
    public int PeriodAmount = 3;
    private float minutes = 2f;
    private float seconds = 0f;
    private int score1 = 0;
    private int score2 = 0;
    private PlayerController[] players;
    private bool countTime = false;
    private bool scored = false;
    private Text minutesText;
    private Text secondsText;
    private Text score1Text;
    private Text score2Text;
    private Text periodText;
    private GoalLogic goal1;
    private GoalLogic goal2;
    private GameObject startPositions;
    private int currentPeriod = 1;

    private GameObject team1BG;
    private GameObject team2BG;
    private GameObject puck;
    private TeamController[] teamControllers;
    private FadingText goText;
    private FadingText readyText;
    private FadingText goalText;
    private FadingText endText;
    private FadingText timeText;
    private FadingText overTimeText;
    private FadingText outText;
    private int[] goals;
    // Start is called before the first frame update
    void Start()
    {
        teamControllers = FindObjectsOfType<TeamController>();
        goText = GameObject.Find("GoText").GetComponent<FadingText>();
        readyText = GameObject.Find("ReadyText").GetComponent<FadingText>();
        goalText = GameObject.Find("GoalText").GetComponent<FadingText>();
        endText = GameObject.Find("EndText").GetComponent<FadingText>();
        timeText = GameObject.Find("TimeText").GetComponent<FadingText>();
        overTimeText = GameObject.Find("OverTimeText").GetComponent<FadingText>();
        outText = GameObject.Find("OutText").GetComponent<FadingText>();
        team1BG = GameObject.Find("ColorBG1");
        team2BG = GameObject.Find("ColorBG2");
        puck = GameObject.FindGameObjectWithTag("Puck");
        players = FindObjectsOfType<PlayerController>();
        minutesText = GameObject.Find("Minutes").GetComponent<Text>();
        secondsText = GameObject.Find("Seconds").GetComponent<Text>();
        score1Text = GameObject.Find("Score1").GetComponent<Text>();
        score2Text = GameObject.Find("Score2").GetComponent<Text>();
        periodText = GameObject.Find("Period").GetComponent<Text>();
        goal1 = GameObject.Find("Goal1").transform.GetChild(0).GetComponent<GoalLogic>();
        goal2 = GameObject.Find("Goal2").transform.GetChild(0).GetComponent<GoalLogic>();
        startPositions = GameObject.Find("StartPositions");
        Physics2D.IgnoreLayerCollision(8, 9);
        Physics2D.IgnoreLayerCollision(10, 11);
        Physics2D.IgnoreLayerCollision(8, 11);
        Physics2D.IgnoreLayerCollision(8, 8);
        SetPeriodTime((int)PeriodTime);
    }

    // Update is called once per frame
    void Update()
    {
        if(countTime)
        {
            CountTime();     
        }
        
    }

    private void CountTime()
    {
        seconds -= Time.deltaTime;
        if(seconds <= 0)
        {
            seconds = seconds + 60;
            minutes--;
            if(minutes < 0)
            {
                EndPeriod();
            }
            minutesText.text = ((int)minutes).ToString();
        }
        secondsText.text = ((int)seconds).ToString();
    }

    private void EndGame()
    {
        if(score1 == score2)
        {
            OverTime();
        }
        else
        {
            endText.Reset();
            foreach (TeamController team in teamControllers)
            {
                team.SetPositions();
            }
            
            FindObjectOfType<TitleController>().TitleScreen();
        }
    }

    public void SetPeriodTime(int time)
    {
        minutes = time / 60;
        seconds = time % 60;
        minutesText.text = minutes.ToString();
        secondsText.text = seconds.ToString();
    }
    private void OverTime()
    {
        overTimeText.Reset();
        periodText.text = "OT";
        Invoke("SwapSides", 2f);
        StartCoroutine("SetPuck", 2f);
    }

    private void EndPeriod()
    {
        timeText.Reset();
        countTime = false;
        scored = true;
        SetPeriodTime((int)PeriodTime);
        if (currentPeriod >= PeriodAmount)
        {
            Invoke("EndGame", 3);
        }
        else
        {
            Invoke("SwapSides", 2f);
            StartCoroutine("SetPuck", 2f);
            currentPeriod++;
            periodText.text = currentPeriod.ToString();
        }
    }

    public void Scored(bool team1)
    {
        
        if(!scored)
        {
            Debug.Log("Scored");
            goalText.Reset();
            scored = true;
            if (team1)
            {
                score1++;
                score1Text.text = score1.ToString();
                
            }
            else
            {
                score2++;
                score2Text.text = score2.ToString();
                
            }
            int side = team1 ? 1 : 2;
            Goal(side);
            countTime = false;
            StartCoroutine("SetPuck", goalDelay);
        }
        
    }
    private void Goal(int side)
    {
        List<int> goalMaker = puck.GetComponent<PuckLogic>().GetGoalMaker();
        int it = goalMaker.Count - 1;
        bool found = false;
        while (it >= 0)
        {
            
            foreach(PlayerController player in players)
            {
                if (player.playerNum == goalMaker[it] && player.side == side)
                {
                    goals[goalMaker[it] - 1]++;
                    if (goals[goalMaker[it] - 1] >= 3)
                    {
                        player.Morko();
                    }
                    found = true;
                }
            }
            it--;
            if (found) break;
        }
        for(int i = 0; i < goals.Length; i++)
        {
            Debug.Log("goal " + i + " " + goals[i]);
        }
        for (int i = 0; i < goalMaker.Count; i++)
        {
            Debug.Log("touchers " + goalMaker[i]);
        }
        
    }

    public void StartGame()
    {
        goals = new int[] { 0, 0, 0, 0 };
        SetGoals();
        CountTime();
        StartCoroutine("SetPuck", 1f);
        currentPeriod = 1;
        score1 = 0;
        score2 = 0;
        SetUI();
    }
    public void Out()
    {
        outText.Reset();
        countTime = false;
        StartCoroutine("SetPuck", goalDelay);
    }

    private void SetUI()
    {
        periodText.text = currentPeriod.ToString();
        score1Text.text = score1.ToString();
        score2Text.text = score2.ToString();
        minutesText.text = ((int) minutes).ToString();
        secondsText.text =((int) seconds).ToString();
    }
    private IEnumerator SetPuck(float delay)
    {
        yield return new WaitForSeconds(delay);
        Debug.Log("SetPuck");
        scored = false;
        puck.transform.position = Vector3.zero;
        puck.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        foreach (TeamController team in teamControllers)
        {
            team.SetPositions();
        }
        readyText.Reset();
        Invoke("Begin", startDelay);
    }

    private void Begin()
    {
        Debug.Log("Begin");
        foreach (TeamController team in teamControllers)
        {
            team.ActivatePlayers();
        }
        goText.Reset();
        countTime = true;
    }
    private void SetGoals()
    {
        goal1.SetTeam(true);
        goal2.SetTeam(false);
    }
    private void SwapSides()
    {
        goal1.SwapTeam();
        goal2.SwapTeam();
        Vector3 pos1 = team1BG.transform.position;
        team1BG.transform.position = team2BG.transform.position;
        team2BG.transform.position = pos1;
        int side = teamControllers[0].side;
        teamControllers[0].SetSide(teamControllers[1].GetSide());
        teamControllers[1].SetSide(side);
    }

}
