using System;
using System.Collections.Generic;
using Bindito.Core;
using Timberborn.ToolSystem;
using UnityDev.LogUtils;

namespace IFTTT_Automation.Utils {

/// <summary>Template class that simplifies tool creation via injection.</summary>
/// <typeparam name="T">type of the dependencies object.</typeparam>
public abstract class AbstractToolWithDependencies<T> : Tool where T : class {

  /// <summary>Wrapper around all the side dependencies that are needed by the class.</summary>
  protected readonly T Injected;

  protected AbstractToolWithDependencies(ToolGroup toolGroup, T injected) {
    ToolGroup = toolGroup;
    Injected = injected;
  }

  protected static void Configure(IContainerDefinition containerDefinition) {
    if (ConfiguredTypes.Contains(typeof(T))) {
      return;  // This dependency is already bound.
    }
    DebugEx.Fine("Register dependency for: {0}", typeof(T));
    ConfiguredTypes.Add(typeof(T));
    containerDefinition.Bind<T>().AsSingleton();
  }
  static readonly HashSet<Type> ConfiguredTypes = new();
}

}
