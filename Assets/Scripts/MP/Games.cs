using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public class Games : MonoBehaviour
{
    public string LobbyName;
    public int playerCount;
    public string Level;
    public int ping;

    [SerializeField] TextMeshProUGUI pname, pingtxt, players;

    private void Start()
    {
        pname.text = LobbyName;
        pingtxt.text = ping.ToString();
        players.text = players.ToString();
    }
}
