using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static Reference;
using static Logger;

public class RoomManager : MonoBehaviourPunCallbacks
{
    RoomOptions roomOptions = new RoomOptions { IsOpen = true, EmptyRoomTtl = 1024, MaxPlayers = 10, PlayerTtl = 2048, IsVisible = true, CleanupCacheOnLeave = true };
    readonly TypedLobby lobbyFilter = new("Game", LobbyType.Default);
    bool isRoomFull => PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers;
    RoomInfo roomInfo;
    byte code_length = 6;

    [SerializeField]
    User[] players;

    string Generate()
    {
        string alphanum = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new(Enumerable.Repeat(alphanum, code_length).Select(s => s[Random.Range(0, s.Length)]).ToArray());
    }

    public void ListRooms()
    {
        PhotonNetwork.GetCustomRoomList(lobbyFilter, )
    }

    public void CreateRoom(string name = "Unnamed", bool locked = false, int playerCount = 10)
    {
        PhotonNetwork.NickName = Ref.playerName.text;
        roomOptions.MaxPlayers = playerCount;
        players = new User[playerCount];
        PhotonNetwork.CreateRoom(name, roomOptions, lobbyFilter, null);
    }

    public void JoinRoom(string lobby)
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.JoinRoom(lobby);
            log.L(PhotonNetwork.NickName + " Joined");
        }
    }

    public override void OnJoinedRoom()
    {
        User usr = Instantiate(Ref.user, Ref.PlayerList).GetComponent<User>();
        usr.playerName = Ref.playerName.text;
        usr.ping = PhotonNetwork.GetPing();
        players[0] = usr;
    }


    public void LeaveRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            log.L(PhotonNetwork.NickName + " Left Room");
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        log.L(newPlayer.NickName + " Joined");
        User usr = Instantiate(Ref.user, Ref.PlayerList).GetComponent<User>();
        usr.playerName = newPlayer.NickName;
        usr.ping = PhotonNetwork.GetPing();
        players[PhotonNetwork.CurrentRoom.PlayerCount] = usr;
        if (isRoomFull)
        {

        }
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        log.L(otherPlayer.NickName + " Left Room");
        foreach(User user in players)
        {
            if (user.playerName == otherPlayer.NickName)
                Destroy(user);
        }
    }
}
