using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Reference : MonoBehaviour
{
    public static Reference Ref { get; private set; }
    public TextMeshProUGUI playerName, lobbyName;

    public Button Create, JoinRoom, Leave;
    public GameObject lobby;
    public GameObject user;

    private void Awake()
    {
        if (Ref == null)
            Ref = this;
        else
            Destroy(Ref);
    }
}
