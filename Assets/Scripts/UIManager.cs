using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using WebSocketSharp;
using static Reference;
using static RoomManager;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    int playerCount;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance);
    }

    private void OnEnable()
    {
        Ref.Create.onClick.AddListener(() => rm.CreateRoom(Ref.lobbyName.text, false, 4));
        Ref.Leave.onClick.AddListener(() => rm.LeaveRoom());
        Ref.Exit.onClick.AddListener(() => Application.Quit());
        Ref.Refresh.onClick.AddListener(() => rm.RefreshLobby());
        Ref.dd_playerNos.onValueChanged.AddListener(delegate
            {
                PlayerCountSelected(Ref.dd_playerNos);
            });
    }
    private void OnDisable()
    {
        Ref.Create.onClick.RemoveAllListeners();
        Ref.Leave.onClick.RemoveAllListeners();
        Ref.Exit.onClick.RemoveAllListeners();
        Ref.Refresh.onClick.RemoveAllListeners();
        Ref.dd_playerNos.onValueChanged.RemoveAllListeners();
    }

    void PlayerCountSelected(TMP_Dropdown dd) => playerCount = int.Parse(dd.options[dd.value].text);
}
