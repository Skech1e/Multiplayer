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
    public static RoomManager rm { get; private set; }

    private void Awake()
    {
        if (rm == null)
            rm = this;
        else
            Destroy(rm);
    }

    public const string MAP_ATTR = "map";
    public const string ACCESS_ATTR = "access";
    public const string PING_ATTR = "ping";

    public readonly TypedLobby lobbyFilter = new("Game", LobbyType.Default);
    bool isRoomFull => PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers;
    public List<Games> games = new();
    public List<RoomInfo> cachedRoomsList = new();
    public List<string> lavduPun2 = new();

    byte code_length = 6;

    [SerializeField]
    User[] players;

    string Generate()
    {
        string alphanum = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new(Enumerable.Repeat(alphanum, code_length).Select(s => s[Random.Range(0, s.Length)]).ToArray());
    }

    public void UpdateRoomList(List<RoomInfo> roomList)
    {
        //print("chck "+cachedRoomsList.Count);
        lavduPun2.Clear();
        for (int i = 0; i < roomList.Count; i++)
        {
            lavduPun2.Add(roomList[i].Name);
        }

        if (cachedRoomsList.Count <= 0)
        {
            cachedRoomsList.Clear();
            cachedRoomsList = roomList;
        }
        else
        {
            foreach (RoomInfo room in roomList)
            {
                if (cachedRoomsList.Contains(room))
                {
                    print("A");
                    //List<RoomInfo> newList = cachedRoomsList;
                    if (room.RemovedFromList)
                    {
                        print("A1");
                        //newList.Remove(newList[i]);
                        cachedRoomsList.Remove(room);
                        print(room.Name + " yeeted ");
                    }
                    else
                    {
                        print("A2");
                        //newList[i] = room;
                    }
                    //cachedRoomsList = newList;
                }
                else
                {
                    print("B");
                    cachedRoomsList.Add(room);
                }
            }
        }
        if (games.Count > 0)
        {
            foreach (var game in games)
                Destroy(game.gameObject);
            games.Clear();
        }

        if (cachedRoomsList.Count > 0)
        {
            print(cachedRoomsList.Count);
            foreach (var room in cachedRoomsList)
            {
                Games game = Instantiate(Ref.lobby, Ref.LobbyList).GetComponent<Games>();
                game.name = room.Name;
                game.LobbyName = room.Name;
                game.playerCount = room.PlayerCount;
                games.Add(game);
            }
        }
    }

    public void RefreshLobby()
    {
        if (games.Count > 0)
        {
            foreach (var game in games)
                Destroy(game.gameObject);
            games.Clear();
        }
        PhotonNetwork.JoinLobby(lobbyFilter);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        UpdateRoomList(roomList);
    }

    public void CreateRoom(string name = "Unnamed", bool locked = false, int playerCount = 10)
    {
        RoomOptions roomOptions = new RoomOptions { IsOpen = true, EmptyRoomTtl = 0, MaxPlayers = playerCount, PlayerTtl = 2048, IsVisible = true };
        //roomOptions.CustomRoomPropertiesForLobby = new[] { MAP_ATTR, ACCESS_ATTR, PING_ATTR };
        //roomOptions.CustomRoomProperties = new() { { MAP_ATTR, 0 }, { ACCESS_ATTR, 1 }, { PING_ATTR, 2 } };
        PhotonNetwork.NickName = Ref.playerName.text;
        players = new User[playerCount];
        PhotonNetwork.CreateRoom(name, roomOptions, lobbyFilter, null);
        log.L("created w/ pc " + playerCount);
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
        foreach (User user in players)
        {
            if (user.playerName == otherPlayer.NickName)
                Destroy(user.gameObject);
        }
    }
}
