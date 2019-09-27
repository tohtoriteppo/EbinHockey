using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoyController : MonoBehaviour
{

    private List<bool> psModes;
    // Start is called before the first frame update
    void Awake()
    {
        psModes = new List<bool>(new bool[]{ false, false, false, false });
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < 4; i++)
        {
            if(Input.GetButton("ps"+(i+1)+"_join"))
            {
                psModes[i] = true;
            }
        }
    }

    public bool IsPsMode(int playerNum)
    {
        return psModes[playerNum-1];
    }

    public float GetLHorizontal(int playerNum)
    {
        if(psModes[playerNum-1])
        {
            return Input.GetAxis("ps" + playerNum + "_left_horizontal");
        }
        return Input.GetAxis("p" + playerNum + "_joystick_horizontal");
    }

    public float GetLVertical(int playerNum)
    {
        if(psModes[playerNum-1])
        {
            return Input.GetAxis("ps" + playerNum + "_left_vertical");
        }
        return Input.GetAxis("p" + playerNum + "_joystick_vertical");
    }

    public float GetRHorizontal(int playerNum)
    {
        return Input.GetAxis("ps" + playerNum  + "_right_horizontal");
    }

    public float GetRVertical(int playerNum)
    {
        return Input.GetAxis("ps" + playerNum  + "_right_vertical");
    }

    public bool GetTackle(int playerNum)
    {
        if(psModes[(playerNum - 1)])
        {
            return Input.GetButton("ps" + playerNum + "_R1");
        }
        return Input.GetButton("p" + playerNum  + "_button_x");
    }
}
