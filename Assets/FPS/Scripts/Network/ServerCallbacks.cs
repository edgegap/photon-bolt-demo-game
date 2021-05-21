using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;
using Unity.FPS.Gameplay;

// Only create instance of class on server
[BoltGlobalBehaviour(BoltNetworkModes.Server)]
public class ServerCallbacks : GlobalEventListener
{
    public override void Connected(BoltConnection connection)
    {
        Debug.LogFormat("{0} connected", connection.RemoteEndPoint);
    }

    public override void Disconnected(BoltConnection connection)
    {
        Debug.LogFormat("{0} disonnected", connection.RemoteEndPoint);
    }

    public override void OnEvent(GameFinishedEvent evt)
    {
        BoltNetwork.LoadScene(evt.SceneToLoad);
    }
}