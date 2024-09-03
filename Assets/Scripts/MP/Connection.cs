using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Logger;
using static RoomManager;

public class Connection : MonoBehaviourPunCallbacks
{

    private void Start()
    {
        PhotonNetwork.SendRate = 40;
        PhotonNetwork.GameVersion = Application.version;
        PhotonNetwork.KeepAliveInBackground = 60;
        PhotonNetwork.MaxResendsBeforeDisconnect = 3;
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {        
        PhotonNetwork.JoinLobby(rm.lobbyFilter);
        log.L("Online");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        InvokeRepeating(nameof(Reconnect), 1, 3);
    }

    public void Reconnect()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable || PhotonNetwork.IsConnected == false)
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.AutomaticallySyncScene = true;
        }
        else
        {
            CancelInvoke(nameof(Reconnect));
        }
    }
}