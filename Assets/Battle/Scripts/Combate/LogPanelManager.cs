using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogPanelManager : MonoBehaviour
{
    public Text battleLogText;
    public GameObject LogPanel;

    private Queue<string> logMessages = new Queue<string>();
    private int maxMessages = 5;

    void Start()
    {
        LogPanel.SetActive(true);
    }

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        // Capturar todos los mensajes, incluyendo daños infligidos
        if (type == LogType.Log)
        {
            AddMessage(logString);
        }
    }

    private void AddMessage(string message)
    {
        LogPanel.SetActive(true); // Asegurar que el panel sea visible

        logMessages.Enqueue(message);

        if (logMessages.Count > maxMessages)
        {
            logMessages.Dequeue();
        }

        battleLogText.text = string.Join("\n", logMessages);
    }
}