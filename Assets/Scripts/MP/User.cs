using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public class User : MonoBehaviour
{
    public string playerName;
    public int ping;
    [SerializeField] TextMeshProUGUI pname, pingtxt;

    private void Start()
    {
        pname.text = playerName;
        pingtxt.text = ping.ToString();
    }
}
