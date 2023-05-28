// Timberborn Utils
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

using Bindito.Core;
using Timberborn.ToolSystem;

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
    InjectableRegistry.BindSingleton<T>(containerDefinition);
  }
}

}
