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

    bool isMaster => PhotonNetwork.IsMasterClient;

    public const string MAP_ATTR = "map";
    public const string ACCESS_ATTR = "access";
    public const string PING_ATTR = "ping";

    public readonly TypedLobby lobbyFilter = new("Game", LobbyType.Default);
    RoomOptions roomOptions = new RoomOptions { IsOpen = true, IsVisible = true, EmptyRoomTtl = 0, PlayerTtl = 0 };
    bool isRoomFull => PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers;
    public List<Games> games = new();
    public List<RoomInfo> cachedRoomsList = new();

    byte code_length = 6;

    [SerializeField]
    List<User> players = new();    

    string Generate()
    {
        string alphanum = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new(Enumerable.Repeat(alphanum, code_length).Select(s => s[Random.Range(0, s.Length)]).ToArray());
    }

    public void UpdateRoomList(List<RoomInfo> roomList)
    {
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
                    if (room.RemovedFromList)
                    {
                        cachedRoomsList.Remove(room);
                        print(room.Name + " yeeted ");
                    }
                    else
                    {
                        for (int i = 0; i < cachedRoomsList.Count; i++)
                        {
                            if (cachedRoomsList[i].Name == room.Name)
                                cachedRoomsList[i] = room;
                        }
                    }
                }
                else
                    cachedRoomsList.Add(room);
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
        roomOptions.CustomRoomPropertiesForLobby = new[] { MAP_ATTR, ACCESS_ATTR, PING_ATTR };
        roomOptions.CustomRoomProperties = new() { { MAP_ATTR, 0 }, { ACCESS_ATTR, 1 }, { PING_ATTR, 2 } };
        roomOptions.MaxPlayers = playerCount;
        PhotonNetwork.NickName = Ref.playerName.text;
        PhotonNetwork.CreateRoom(name, roomOptions, lobbyFilter, null);
        log.L("created w/ pc " + playerCount);
    }

    public void JoinRoom(string lobby)
    {
        PhotonNetwork.NickName = Ref.playerName.text;        
        if (PhotonNetwork.IsConnectedAndReady)
            PhotonNetwork.JoinRoom(lobby);
    }

    public override void OnJoinedRoom()
    {
        Room room = PhotonNetwork.CurrentRoom;
        log.L(PhotonNetwork.NickName + " Joined " + room.PlayerCount);
        foreach (var p in PhotonNetwork.CurrentRoom.Players)
            print(p.Key + " " + p.Value);
        for (int i = 0; i < room.PlayerCount; i++)
        {
            Player pl = room.Players[i + 1];
            User usr = Instantiate(Ref.user, Ref.PlayerList).GetComponent<User>();
            usr.name = pl.NickName;
            usr.playerName = pl.NickName;
            //usr.ping = pl.
            players.Add(usr);
        }

    }


    public void LeaveRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            log.L(PhotonNetwork.NickName + " Left Room");
            for (int i = 0; i < players.Count; i++)
                Destroy(players[i].gameObject);
            players.Clear();
        }
    }

    public override void OnLeftRoom()
    {

    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        log.L(newPlayer.NickName + " Joined");
        string npName = newPlayer.NickName;
        User usr = Instantiate(Ref.user, Ref.PlayerList).GetComponent<User>();
        usr.name = newPlayer.NickName;
        usr.playerName = npName;
        usr.ping = PhotonNetwork.GetPing();
        players.Add(usr);
        PhotonNetwork.CurrentRoom.IsOpen = !isRoomFull;
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Room room = PhotonNetwork.CurrentRoom;
        room.IsOpen = !isRoomFull;
        foreach(var p in room.Players)
        {
            if (p.Value == otherPlayer)
                room.Players.Remove(p.Key);
        }
        log.L(otherPlayer.NickName + " Left Room");
        User usr = players.Find(p => p.name == otherPlayer.NickName);
        players.Remove(usr);
        Destroy(usr.gameObject);
        //Destroy(go);
    }
}
