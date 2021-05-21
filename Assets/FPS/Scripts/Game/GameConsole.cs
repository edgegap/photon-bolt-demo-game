using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.FPS.Game;

public class GameConsole : MonoBehaviour
{
    [SerializeField]
    private int maxLength = 5000;

    [SerializeField]
    private Text consoleOutput;

    [SerializeField]
    private Scrollbar consoleScrollBar;

    private CursorState previousCursorState;

    private static GameConsole _instance = null;

    public class CursorState{
        public CursorLockMode LockState;
        public bool Visible;
        public CursorState(CursorLockMode lockState, bool visible){
            this.LockState = lockState;
            this.Visible = visible;
        }
    }

    private void Awake(){
        if(_instance == null){
            _instance = this;
        }
        
        if(this != _instance){
            Destroy(gameObject);
        }
    }

    private void OnDestroy(){
        if(this == _instance){
            _instance = null;
        }
    }

    private void Start(){
        DontDestroyOnLoad(gameObject);
        Application.logMessageReceived += UpdateConsoleOutput;

        consoleOutput.supportRichText = true;
        consoleOutput.text += "Console start";
    }

    private void Update(){
        if(Input.GetKeyDown(KeyCode.F1)){
            Canvas canvas = GetComponent<Canvas>();
            canvas.enabled = !canvas.enabled;
            consoleScrollBar.value = 0;

            if(canvas.enabled){
                GUIState.GUIVisible = false;
                previousCursorState = new CursorState(Cursor.lockState, Cursor.visible);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }else{
                GUIState.GUIVisible = true;
                Cursor.lockState = previousCursorState.LockState;
                Cursor.visible = previousCursorState.Visible;
            }
        }
    }

    private void UpdateConsoleOutput(string logString, string stackTrace, LogType type){
        consoleOutput.text += "\n" + GetColoredConsoleText(logString, type);

        if(type == LogType.Error || type == LogType.Exception || type == LogType.Error){
            consoleOutput.text += "\n" + GetColoredConsoleText(stackTrace, type);
        }

        if (consoleOutput.text.Length > maxLength)
        {
            consoleOutput.text = consoleOutput.text.Substring(consoleOutput.text.Length - maxLength, consoleOutput.text.Length - (consoleOutput.text.Length - maxLength));
        }
    }

    private string GetColoredConsoleText(string logString, LogType type){
        if(type == LogType.Error || type == LogType.Exception || type == LogType.Error){
            return string.Format("<color=#FF0000FF>{0}</color>", logString);
        }else if(type == LogType.Warning){
            return string.Format("<color=#FFFF00FF>{0}</color>", logString);
        }
        else return logString;
    }
}
