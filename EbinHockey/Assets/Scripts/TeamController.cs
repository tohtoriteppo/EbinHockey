using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamController : MonoBehaviour
{
    public List<PlayerController> players;
    private GameObject[] startPositions;
    public int side = 1;
    private Color color;
    private SpriteRenderer background;
    // Start is called before the first frame update
    void Awake()
    {
        players = new List<PlayerController>();
        background = transform.GetChild(0).GetComponent<SpriteRenderer>();
        SetSide(side);
    }

    public void SetSide(int sideToPut)
    {
        side = sideToPut;
        startPositions = GameObject.FindGameObjectsWithTag("Team" + side.ToString());
        foreach(PlayerController player in players)
        {
            player.side = sideToPut;
        }
    }
    public void AddPlayer(PlayerController player)
    {
        if (!players.Contains(player))
        {
            players.Add(player);
            player.side = side;
        }

    }
    public void RemovePlayer(PlayerController player)
    {
        players.Remove(player);
    }
    public bool FindPlayer(PlayerController player)
    {
        return players.Contains(player);
    }
    public int GetSide()
    {
        return side;
    }
    public void RandomColor()
    {
        color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
        foreach (PlayerController player in players)
        {
            player.SetColor(color);
        }
        background.color = color;
    }

    public void SetPositions()
    {
        //RandomizeArray(startPositions);
        for(int i = 0; i < players.Count; i++)
        {
            players[i].transform.position = startPositions[i].transform.position;
            players[i].ResetPlayer();
        }
    }
    public void ActivatePlayers()
    {
        for (int i = 0; i < players.Count; i++)
        {
            players[i].ActivatePlayer();
        }
    }
    private void RandomizeArray(GameObject[] arr)
    {
        for (var i = arr.Length - 1; i > 0; i--)
        {
            var r = Random.Range(0, i);
            var tmp = arr[i];
            arr[i] = arr[r];
            arr[r] = tmp;
        }
    }
    public void UpdateColor()
    {
        foreach(PlayerController player in players)
        {
            player.SetColor(color);
        }
        background.color = color;
    }
}
