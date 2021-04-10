﻿using HarmonyLib;
using NebulaWorld.MonoBehaviours.Remote;
using UnityEngine;

namespace NebulaWorld
{
    public class RemotePlayerModel
    {
        const int PLAYER_PROTO_ID = 1;

        public Transform PlayerTransform { get; set; }
        public Transform PlayerModelTransform { get; set; }
        public RemotePlayerMovement Movement { get; set; }
        public RemotePlayerAnimation Animator { get; set; }
        public RemotePlayerEffects Effects { get; set; }

        public Player PlayerInstance { get; set; }
        public Mecha MechaInstance { get; set; }

        public RemotePlayerModel(ushort playerId)
        {
            // Spawn remote player model by cloning the player prefab and replacing local player script by remote player ones.
            string playerPrefabPath = LDB.players.Select(PLAYER_PROTO_ID).PrefabPath;
            if (playerPrefabPath != null)
            {
                PlayerTransform = Object.Instantiate(Resources.Load<Transform>(playerPrefabPath));
                PlayerModelTransform = PlayerTransform.Find("Model");

                // Remove local player components
                Object.Destroy(PlayerTransform.GetComponent<PlayerFootsteps>());
                Object.Destroy(PlayerTransform.GetComponent<PlayerEffect>());
                Object.Destroy(PlayerTransform.GetComponent<PlayerAudio>());
                Object.Destroy(PlayerTransform.GetComponent<PlayerAnimator>());
                Object.Destroy(PlayerTransform.GetComponent<PlayerController>());
                PlayerTransform.GetComponent<Rigidbody>().isKinematic = true;

                // Add remote player components
                Movement = PlayerTransform.gameObject.AddComponent<RemotePlayerMovement>();
                Animator = PlayerTransform.gameObject.AddComponent<RemotePlayerAnimation>();
                Effects = PlayerTransform.gameObject.AddComponent<RemotePlayerEffects>();
            }

            PlayerTransform.gameObject.name = $"Remote Player ({playerId})";

            PlayerInstance = new Player();
            MechaInstance = new Mecha();
            AccessTools.Property(typeof(Player), "mecha").SetValue(PlayerInstance, MechaInstance, null);
            MechaInstance.Init(PlayerInstance);
        }

        public void Destroy()
        {
            Object.Destroy(PlayerTransform.gameObject);
            PlayerTransform = null;
            PlayerModelTransform = null;
            Movement = null;
            Animator = null;
            Effects = null;
            PlayerInstance.Free();
            PlayerInstance = null;
        }
    }
}
