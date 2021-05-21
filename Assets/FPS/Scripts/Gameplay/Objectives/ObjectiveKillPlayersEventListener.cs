using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Unity.FPS.Gameplay
{
    public class ObjectiveKillPlayersEventListener : Bolt.GlobalEventListener
    {
        public UnityAction<ScoreUpdateEvent> OnScoreUpdate;

        public override void OnEvent(ScoreUpdateEvent evt)
        {
            OnScoreUpdate.Invoke(evt);
        }
    }
}