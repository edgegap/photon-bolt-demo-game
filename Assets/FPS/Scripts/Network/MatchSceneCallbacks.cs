using UnityEngine;
using Bolt;
using System;
using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Unity.FPS.Network
{
    // Script is added automatically to scene
    [BoltGlobalBehaviour("MatchScene")]
    public class MatchSceneCallbacks : GlobalEventListener
    {
        [Obsolete]
        public override void SceneLoadLocalDone(string scene)
        {
            // Spawn player when scene is done loading locally.
            GameObject newPlayer = BoltNetwork.Instantiate(BoltPrefabs.PlayerCharacter);

            // Reset match score
            ObjectiveKillPlayers.playerScores = new List<int>();

            if (HeadlessServerManager.IsHeadlessMode() && BoltNetwork.IsServer)
            {
                // Destroy newPlayer if the (headless) server is its owner.
                if (newPlayer.GetComponent<PlayerCharacterController>().entity.IsOwner)
                {
                    BoltNetwork.Destroy(newPlayer);
                    Debug.Log("Server ready");
                }
            }
        }

        public override void OnEvent(CreateProjectileEvent evt)
        {
            if (PlayerCharacterController.LocalPlayer == null) return;
            // Create projectile only for distant clients
            if (evt.OwnerEntity == PlayerCharacterController.LocalPlayer.entity) return;

            GameObject newProjectile = BoltNetwork.Instantiate(evt.ProjectilePrefab, evt.MuzzlePosition,
                        Quaternion.LookRotation(evt.ShotDirection));
            newProjectile.GetComponent<ProjectileBase>().IsServerInstance = true;

            newProjectile.GetComponent<ProjectileBase>().Shoot(evt.OwnerEntity.gameObject.GetComponentInChildren<WeaponController>());
        }
    }

}
