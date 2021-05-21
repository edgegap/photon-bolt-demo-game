using UnityEngine;
using Bolt;
using System;
using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using UnityEngine.SceneManagement;
using UdpKit;

namespace Unity.FPS.Network
{
    // Script is added automatically to scene
    [BoltGlobalBehaviour]
    public class NetworkCallbacks : GlobalEventListener
    {
        public override void ConnectFailed(UdpEndPoint endpoint, IProtocolToken token)
        {
            ReturnToMenu();
        }

        public override void ConnectRefused(UdpEndPoint endpoint, IProtocolToken token)
        {
            ReturnToMenu();
        }

        public override void BoltShutdownBegin(AddCallback registerDoneCallback, UdpConnectionDisconnectReason disconnectReason)
        {
            ReturnToMenu();
        }

        public override void Disconnected(BoltConnection connection)
        {
            ReturnToMenu();
        }

        public void ReturnToMenu()
        {
            // Clear game events
            PlayerEvents.OnLocalPlayerReady = null;

            // Ignore for non-clients
            if (!BoltNetwork.IsClient) return;

            // Prevent infinite loop
            if(SceneManager.GetActiveScene().name == "StartScene") return;

            // Reload starting scene
            // if (BoltNetwork.IsRunning) BoltNetwork.Shutdown();
            SceneManager.LoadScene("StartScene");
        }
    }

}
