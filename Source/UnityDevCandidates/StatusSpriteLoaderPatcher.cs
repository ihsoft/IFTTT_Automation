// Timberborn Utils
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

using HarmonyLib;
using TimberApi.ConsoleSystem;
using TimberApi.ModSystem;
using Timberborn.AssetSystem;
using Timberborn.StatusSystem;
using UnityEngine;

namespace Automation.UnityDevCandidates {

// ReSharper disable once UnusedType.Global
sealed class StatusSpriteLoaderPatcher : IModEntrypoint {
  public void Entry(IMod mod, IConsoleWriter consoleWriter) {
    new Harmony("IgorZ.Automation").PatchAll();
  }
}

/// <summary>Patch that allows using custom icons in status subject in teh stock logic.</summary>
/// <remarks>
/// The status specification gets name of teh icon and behind the hood translates it to a path to the stock game folder.
/// In order to re-write the path to the plugin's asset folder, add prefix "?>" to the sprite name and provide full path
/// to the custom icon. E.g. if icon is stored in "my.mod/icons/icon0", then provide sprite name "?>my.mod/icons/icon0". 
/// </remarks>
[HarmonyPatch(typeof(StatusSpriteLoader), nameof(StatusSpriteLoader.LoadSprite), typeof(string))]
static class StatusSpriteLoaderPatch {
  static bool Prefix(string spriteName, IResourceAssetLoader ____resourceAssetLoader, ref Sprite __result) {
    if (!spriteName.StartsWith("?>")) {
      return true;
    }
    __result = ____resourceAssetLoader.Load<Sprite>(spriteName.Substring(2));
    return false;
  }
}

}
