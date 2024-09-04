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
    public List<RoomInfo> rooms = new();
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
        if (games.Count > 0)
        {
            foreach (var game in games)
                Destroy(game.gameObject);
            games.Clear();
        }
        //print("count " + roomList.Count);
        for (int i = 0; i < roomList.Count; i++)
        {
            RoomInfo rInfo = roomList[i];
            print(rInfo.Name);

            if (rInfo != null && rInfo.RemovedFromList)
            {
                print(rInfo.Name + " yeeted");
                rooms.Remove(rInfo);
                Destroy(games[i].gameObject);
                games.Remove(games[i]);
            }

            rooms.Add(rInfo);
            Games game = Instantiate(Ref.lobby, Ref.LobbyList).GetComponent<Games>();
            game.name = rInfo.Name;
            games.Add(game);


            /*else if (rooms[i] != roomList[i])
            {
                if (rInfo.RemovedFromList)
                {
                    print(rInfo.Name + " yeeted");
                    rooms.Remove(rInfo);
                    Destroy(games[i].gameObject);
                    games.Remove(games[i]);
                }
                else
                {
                    rooms.Add(rInfo);
                    Games game = Instantiate(Ref.lobby, Ref.LobbyList).GetComponent<Games>();
                    game.name = rInfo.Name;
                    games.Add(game);
                }
            }*/
            //print(i + "below");
            //Games ga = games[i];
            game.LobbyName = rInfo.Name;
            //print(game.name + " renamed" + game.LobbyName);
            game.playerCount = rInfo.MaxPlayers;
        }
    }

    public void RefreshLobby()
    {
        /*if (games.Count > 0)
        {
            foreach (var game in games)
                Destroy(game.gameObject);
            games.Clear();
        }*/
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
                Destroy(user);
        }
    }
}
