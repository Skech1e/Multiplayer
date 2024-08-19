using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logger : MonoBehaviour
{
    public static Logger log { get; private set; }
    public bool showLog;

    private void Awake()
    {
        if (log == null)
            log = this;
        else
            Destroy(log);
    }

    public void L(object msg)
    {
        if(showLog)
            Debug.Log(msg);
    }

}
