// Timberborn Utils
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

using System;
using System.Collections.Generic;
using Bindito.Core;
using TimberApi.ConfiguratorSystem;
using TimberApi.SceneSystem;
using UnityDev.LogUtils;

namespace IFTTT_Automation.Utils {

/// <summary>Helper class that allows binding a shared injectable.</summary>
/// <remarks>
/// It's safe to call binding methods for the same type multiple times. The binding will actually be made only once.
/// </remarks>
[Configurator(SceneEntrypoint.All)]
// ReSharper disable once ClassNeverInstantiated.Global
public class InjectableRegistry : IConfigurator {
  static readonly HashSet<Type> ConfiguredTypes = new();

  #region IConfigurator implementation
  public void Configure(IContainerDefinition containerDefinition) {
    ConfiguredTypes.Clear();
  }
  #endregion

  /// <summary>Bind a singleton injectable. It's safe to call this method multiple times for the same type.</summary>
  public static void BindSingleton<T>(IContainerDefinition containerDefinition) where T : class {
    if (ConfiguredTypes.Contains(typeof(T))) {
      return;
    }
    DebugEx.Fine("Register injectable: {0}", typeof(T));
    ConfiguredTypes.Add(typeof(T));
    containerDefinition.Bind<T>().AsSingleton();
  } 
}

}
