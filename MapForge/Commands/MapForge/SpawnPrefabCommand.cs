﻿using CommandSystem;
using MapForge.API;
using MapForge.API.Models;
using RemoteAdmin;
using System;
using UnityEngine;

namespace MapForge.Commands.MapForge
{
    public class SpawnPrefabCommand : ICommand
    {
        public string Command { get; } = "spawnprefab";

        public string[] Aliases { get; } = new string[] { "sp" };

        public string Description { get; } = "Spawns prefab";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!(sender is PlayerCommandSender playerSender))
            {
                response = "This command can be only executed ingame!";
                return false;
            }

            if (arguments.Count < 2)
            {
                response = "Syntax: mapforge spawnprefab (bundleName) (prefabName) (dimensionId OPTIONAL)";
                return false;
            }

            string bundleName = arguments.At(0);
            string prefabName = arguments.At(1);

            if (!MapForgeAPI.TryGetBundle(bundleName, out BundleInfo bundle))
            {
                response = $"Bundle \"{bundleName}\" is not loaded!";
                return false;
            }

            if (!bundle.ContainsPrefab(prefabName))
            {
                response = $"Prefab \"{prefabName}\" not exists in bundle \"{bundleName}\"";
                return false;
            }

            int dimensionId = -1;
            if (arguments.Count == 3)
            {
                if (int.TryParse(arguments.At(2), out int dimId))
                    dimensionId = dimId;
            }

            PrefabInfo prefab = bundle.CreatePrefab(prefabName);
            prefab.Spawn(playerSender.ReferenceHub.transform.position, Vector3.zero, Vector3.one, dimensionId: dimensionId);
            response = $"Spawned prefab [{prefab.Id}] {prefabName}";
            return true;
        }
    }
}
