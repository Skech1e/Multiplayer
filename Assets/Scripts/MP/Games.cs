using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static RoomManager;
using static Reference;

[DisallowMultipleComponent]
public class Games : MonoBehaviour
{
    public string LobbyName;
    string LobbyID;
    public int playerCount;
    public string Level;
    public int ping;

    [SerializeField] TextMeshProUGUI pname, pingtxt, players;
    Button button;

    private void Start()
    {
        pname.text = LobbyName;
        pingtxt.text = ping.ToString();
        players.text = players.ToString();
    }
    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            Ref.Menu.gameObject.SetActive(false);
            Ref.LobbyPanel.gameObject.SetActive(true);
            rm.JoinRoom(LobbyName);
        }); 
    }

    private void OnDestroy()
    {
        button.onClick.RemoveAllListeners();
    }
    private void OnDisable()
    {
        button.onClick.RemoveAllListeners();
    }
}
