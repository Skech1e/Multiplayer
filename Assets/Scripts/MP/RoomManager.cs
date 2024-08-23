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

    readonly TypedLobby lobbyFilter = new("Game", LobbyType.Default);
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
        for (int i = 0; i < roomList.Count; i++)
        {
            RoomInfo rInfo = roomList[i];
            if (rooms.Count == 0)
            {
                rooms.Add(rInfo);
                Games game = Instantiate(Ref.lobby, Ref.LobbyList).GetComponent<Games>();
                games.Add(game);
            }
            else if (rooms[i] != roomList[i])
            {
                if (rInfo.RemovedFromList)
                {
                    rooms.Remove(rInfo);
                    Destroy(games[i].gameObject);
                    games.Remove(games[i]);
                }
                else
                {
                    print("ok");
                    rooms.Add(rInfo);
                    Games game = Instantiate(Ref.lobby, Ref.LobbyList).GetComponent<Games>();
                    games.Add(game);
                }
            }
            Games ga = games[i];
            ga.LobbyName = rInfo.Name;
            ga.playerCount = rInfo.MaxPlayers;
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        UpdateRoomList(roomList);
    }

    public void CreateRoom(string name = "Unnamed", bool locked = false, int playerCount = 10)
    {
        RoomOptions roomOptions = new RoomOptions { IsOpen = true, EmptyRoomTtl = 512, MaxPlayers = 10, PlayerTtl = 2048, IsVisible = true, CleanupCacheOnLeave = true };
        roomOptions.CustomRoomPropertiesForLobby = new[] { MAP_ATTR, ACCESS_ATTR, PING_ATTR };
        roomOptions.CustomRoomProperties = new() { { MAP_ATTR, 0 }, { ACCESS_ATTR, 1 }, { PING_ATTR, 2 } };
        PhotonNetwork.NickName = Ref.playerName.text;
        roomOptions.MaxPlayers = playerCount;
        players = new User[playerCount];
        PhotonNetwork.CreateRoom(name, roomOptions, lobbyFilter, null);
        log.L("created");
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
