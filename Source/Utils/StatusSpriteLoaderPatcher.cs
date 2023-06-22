// Timberborn Utils
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

using System;
using System.Reflection;
using HarmonyLib;
using TimberApi.ConsoleSystem;
using TimberApi.ModSystem;
using Timberborn.AssetSystem;
using Timberborn.StatusSystem;
using UnityEngine;

namespace Automation.Utils {

// ReSharper disable once UnusedType.Global
sealed class StatusSpriteLoaderPatcher : IModEntrypoint {
  public void Entry(IMod mod, IConsoleWriter consoleWriter) {
    Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
  }
}

/// <summary>Patch that allows using custom icons in status subject in teh stock logic.</summary>
/// <remarks>
/// The status specification gets name of teh icon and behind the hood translates it to a path to the stock game folder.
/// In order to re-write the path to the plugin's asset folder, add prefix <see cref="ResetPathDelimiter"/> to the
/// sprite name and provide full path to the custom icon. E.g. if icon is stored in "my.mod/icons/icon0", then provide
/// sprite name <c>ResetPathDelimiter</c> + "my.mod/icons/icon0". 
/// </remarks>
[HarmonyPatch(typeof(StatusSpriteLoader), nameof(StatusSpriteLoader.LoadSprite))]
static class StatusSpriteLoaderPatch {
  /// <summary>Resource path delimiter that can be used to rewrite the stock game paths.</summary>
  public const string ResetPathDelimiter = "_TAPI_RESOURCE_/";

  [HarmonyPriority(Priority.First)]
  static bool Prefix(string spriteName, IResourceAssetLoader ____resourceAssetLoader,
                     ref Sprite __result, out Sprite __state) {
    if (!spriteName.StartsWith(ResetPathDelimiter)) {
      __state = null;
      return true;
    }
    __state = ____resourceAssetLoader.Load<Sprite>(spriteName.Substring(ResetPathDelimiter.Length));

    // Use this weird workaround to prevent Harmony from calling the other prefixes in the chain.
    // It's unclear at the moment why returning "false" from the Prefix has no effect on the other patches.
    throw new AbortEvilPrefixesException();
  }

  [HarmonyPriority(Priority.First)]
  static Exception Finalizer(Exception __exception, ref Sprite __result, Sprite __state) {
    if (__exception is AbortEvilPrefixesException) {
      __result = __state;
    }
    return null;
  }

  class AbortEvilPrefixesException : Exception {}
}

// [HarmonyPatch(typeof(StatusSpriteLoader), nameof(StatusSpriteLoader.LoadSprite), typeof(string))]
//  static class StatusSpriteLoaderPatch {
//    /// <summary>
//    ///     Resource path delimiter that can be used to rewrite the stock game paths.
//    /// </summary>
//    public const string ResetPathDelimiter = "_TAPI_RESOURCE_/";
//
//    [SuppressMessage("ReSharper", "InconsistentNaming")]
//    // ReSharper disable once UnusedMember.Local
//    static bool Prefix(string spriteName, IResourceAssetLoader ____resourceAssetLoader, ref Sprite __result) {
//      if (!spriteName.StartsWith(ResetPathDelimiter)) {
//        return true;
//      }
//      __result = ____resourceAssetLoader.Load<Sprite>(spriteName.Substring(ResetPathDelimiter.Length));
//      return false;
//    }
//  }

}
