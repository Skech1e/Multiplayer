using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Reference : MonoBehaviour
{
    public static Reference Ref { get; private set; }
    public TextMeshProUGUI playerName, lobbyName;
    public TMP_Dropdown dd_playerNos;

    public Button Create, Refresh, Leave, Exit;
    public GameObject lobby;
    public GameObject user;
    public Transform LobbyList, PlayerList;

    private void Awake()
    {
        if (Ref == null)
            Ref = this;
        else
            Destroy(Ref);        
    }  
}
