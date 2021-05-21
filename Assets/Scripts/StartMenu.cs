using System;
using UnityEngine;
using Bolt;
using Bolt.Matchmaking;
using UdpKit;
using System.Collections;
using UnityEngine.Events;

public class StartMenu : GlobalEventListener
{
    [SerializeField] string matchScene;
    private bool timerStarted = false;
    private int timerSeconds = 0;
    private int timerMinutes = 0;
    private int timerHours = 0;
    private string connectionLabel = "Connect to a random server";
    private string serverCountLabel = "Bolt shutdown";
    private bool serverRequestAvailable = true;
    private int serverRequestTimer = 0;
    private Map<Guid, UdpSession> serverSessionList;
    private Coroutine autoStartClient = null;
    private float launchTimer = 0;

    StagingController staging;
    GUIStyle testStyle;
    private static string oldEdgegapToken = String.Empty;

    private void Update(){
        if(autoStartClient == null){
            if(launchTimer <= 0){
                if(BoltNetwork.IsRunning) BoltNetwork.ShutdownImmediate();
                staging = GetComponent<StagingController>();
                autoStartClient = StartCoroutine("AutoStartClient");
            }
        }

        if(launchTimer > 0) launchTimer -= Time.deltaTime;
    }

    private void RestartClient(){
        serverCountLabel = "Restarting Bolt...";
        if(autoStartClient != null) StopCoroutine(autoStartClient);
        autoStartClient = null;
        launchTimer = 1f;
    }

    private IEnumerator AutoStartClient(){
        while(!BoltNetwork.IsRunning){
            BoltLauncher.StartClient();
            yield return new WaitForSeconds(1f);
        }
    }

    void OnGUI()
    {
        if(Unity.FPS.Game.GUIState.GUIVisible == false) return;

        GUILayout.BeginArea(new Rect(50, 50, Screen.width - 100, Screen.height - 100));

        // "Start Server" button
        if (serverRequestAvailable && GUILayout.Button("Request Server", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
        {
            StartCoroutine("DisableServerRequest");
            staging.RequestServer();
        } else if(!serverRequestAvailable){
            GUILayout.Button(staging.StatusString + String.Format("\n Next request available in {0} seconds.", serverRequestTimer + 1), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        }

        // "Start client" button
        if (!timerStarted && GUILayout.Button(connectionLabel, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
        {
            ConnectToRandomServer();
        }

        GUILayout.Label(serverCountLabel);

        GUILayout.EndArea();
    }

    private void ConnectToRandomServer(){
        try{
            connectionLabel = "Connecting...";

            if(serverSessionList == null || serverSessionList.Count == 0) {
                Debug.LogWarning("No server found, aborting connection.");
                connectionLabel = "No server found\nTry again when a server is available";
                return;
            }

            // Connect to first session found
            foreach (var session in serverSessionList)
            {
                UdpSession photonSession = session.Value as UdpSession;

                if (photonSession.Source == UdpSessionSource.Photon)
                {
                    BoltMatchmaking.JoinSession(photonSession);
                    return;
                }
            }
            connectionLabel = "Couldn't connect to server, please try again";
        }catch(Exception e){
            Debug.Log(e.ToString());
            connectionLabel = "An error happened, please try again";
        }
    }

    private IEnumerator DisableServerRequest(){
        serverRequestAvailable = false;

        // Wait for finished status
        while(Math.Abs(staging.Status) < 100){
            yield return new WaitForSecondsRealtime(0.2f);
        }

        if(staging.Status < 0) serverRequestTimer = 3;
        else serverRequestTimer = 15;

        while(serverRequestTimer > 0){
            serverRequestTimer--;
            yield return new WaitForSecondsRealtime(1f);
        }

        serverRequestAvailable = true;
        staging.ResetStatus();
    }

    private IEnumerator TickTimer()
    {
        while (timerStarted)
        {
            // Wait 1 second
            yield return new WaitForSeconds(1);

            // Tick seconds
            timerSeconds++;

            // Tick minutes
            if(timerSeconds >= 60)
            {
                timerSeconds -= 60;
                timerMinutes += 1;
            }

            // Tick hours
            if (timerMinutes >= 60)
            {
                timerMinutes -= 60;
                timerHours += 1;
            }
        }
    }

    private string GetFormatedTime()
    {
        //return ("HH:MM:SS");
        if (timerHours > 0)
        {
            return String.Format("{0}:{1}:{2}", timerHours, (timerMinutes < 10 ? "0" + timerMinutes.ToString() : timerMinutes.ToString()), (timerSeconds < 10 ? "0" + timerSeconds.ToString() : timerSeconds.ToString()));
        } else if(timerMinutes > 0)
        {
            return String.Format("{0}:{1}", timerMinutes, (timerSeconds < 10 ? "0" + timerSeconds.ToString() : timerSeconds.ToString()));
        }else
        {
            return timerSeconds.ToString();
        }
    }

    public override void BoltStartFailed(UdpConnectionDisconnectReason disconnectReason)
    {
        Debug.LogWarning("BoltStartFailed : " + disconnectReason);

        serverCountLabel = "Bolt start failed, retrying...";

        StopCoroutine(autoStartClient);
        autoStartClient = StartCoroutine("AutoStartClient");
    }

    public override void BoltStartBegin()
    {
        serverCountLabel = "Starting Bolt...";
    }

    // Create match session once server is ready
    public override void BoltStartDone()
    {
        serverCountLabel = "Searching for servers...";

        if (BoltNetwork.IsServer)
        {
            string matchName = Guid.NewGuid().ToString();

            BoltMatchmaking.CreateSession(
                sessionID: matchName,
                sceneToLoad: matchScene
            );
        }
    }

    public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
    {
        serverSessionList = sessionList;
        serverCountLabel = String.Format("Available servers: {0}", sessionList.Count);
        connectionLabel = "Connect to a random server";
    }
}