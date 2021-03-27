﻿using HarmonyLib;
using NebulaModel.Packets.Universe;
using NebulaWorld;
using NebulaWorld.Universe;
using UnityEngine;

namespace NebulaPatcher.Patches.Dynamic
{
    [HarmonyPatch(typeof(DysonSwarm))]
    class DysonSwarm_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("NewOrbit")]
        public static bool Prefix1(DysonSwarm __instance, int __result, float radius, Quaternion rotation)
        {
            //Notify others that orbit for Dyson Swarm was created
            if (!DysonSphere_Manager.IncomingDysonSwarmPacket)
            {
                LocalPlayer.SendPacket(new DysonSwarmAddOrbitPacket(__instance.starData.index, radius, rotation));
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("RemoveOrbit")]
        public static bool Prefix2(DysonSwarm __instance, int orbitId)
        {
            //Notify others that orbit for Dyson Swarm was deleted
            if (!DysonSphere_Manager.IncomingDysonSwarmPacket)
            {
                LocalPlayer.SendPacket(new DysonSwarmRemoveOrbitPacket(__instance.starData.index, orbitId));
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("AddBullet")]
        public static void Postfix(DysonSwarm __instance, SailBullet bullet, int orbitId)
        {
            //Host is sending correction / authorization packet to correct constants of the generated bullet
            Debug.Log($"Sending sync packet to the other players {SimulatedWorld.Initialized} - {LocalPlayer.IsMasterClient}");
            if (SimulatedWorld.Initialized && LocalPlayer.IsMasterClient)
            {
                LocalPlayer.SendPacket(new DysonSphereBulletCorrectionPacket(__instance.starData.index, bullet.id, bullet.uEndVel, bullet.uEnd));
            }
        }
    }
}
