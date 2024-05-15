using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debug : MonoBehaviour {

    public static Debug Log;

    [SerializeField]
    bool ScreenDebug = true;

    string debugStr = "";

    private void Awake()
    {
        Log = this;
    }

    bool isAndroid = false;
    bool androidDebug = false;

    private void Start()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            isAndroid = true;
        }
        else if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            isAndroid = false;
        }
        try
        {
            networkDebug = TestClient.Instance.NetworkDebug;
        }
        catch { }
    }

    bool networkDebug;

    public void Add(string contenet)
    {
        UnityEngine.Debug.Log(DateTime.Now.ToString("[hh:mm:ss] ") + contenet);
        if (isAndroid)
        {
            if (ScreenDebug && androidDebug)
            {
                debugStr = DateTime.Now.ToString("[hh:mm:ss] ") + contenet + "\n" + debugStr;
            }
        }
        if(networkDebug)
        {
            try
            {
                TestClient.Instance.sendMessage(contenet);
            }
            catch { }
        }
    }

    GUIStyle guiStyle = new GUIStyle();

    private void OnGUI()
    {
        guiStyle.fontSize = 40;
        guiStyle.normal.textColor = Color.green;
        GUI.Label(new Rect(10, 130, 150, 1000), debugStr, guiStyle);
    }
}
