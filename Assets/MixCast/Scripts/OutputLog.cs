using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class OutputLog : MonoBehaviour {

    public string logFileName = "MixCastLog.txt";
    private StreamWriter sw;

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(gameObject);
        if (!File.Exists(Path.Combine(Application.persistentDataPath, logFileName)))
        {
            File.Create(Path.Combine(Application.persistentDataPath, logFileName));
        }
        
	}

    private void OnEnable()
    {
        /*
         * There is also a Application.logMessageReceivedThreaded event
         * */

        Application.logMessageReceived += HandleLog;
        sw = new StreamWriter(Path.Combine(Application.persistentDataPath, logFileName));
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
        sw.Close();
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {

        sw.WriteLine(logString + " " + stackTrace);

    }
}
