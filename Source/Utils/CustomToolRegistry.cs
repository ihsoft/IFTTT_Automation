// Timberborn Utils
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

using Bindito.Core;
using TimberApi.DependencyContainerSystem;
using TimberApi.ToolSystem;
using Timberborn.ToolSystem;

namespace IFTTT_Automation.Utils {

public static class CustomToolRegistry {
  /// <summary>Base class for all custom tools.</summary>
  public abstract class CustomTool : Tool {
    protected ToolSpecification ToolSpecification;

    /// <summary>Initializes the tool. Do all logic here instead of the constructor.</summary>
    /// <param name="toolGroup">The group this toll is a child of.</param>
    /// <param name="toolSpecification">The specification of this tool from TimberAPI.</param>
    public virtual void InitializeTool(ToolGroup toolGroup, ToolSpecification toolSpecification) {
      ToolGroup = toolGroup;
      ToolSpecification = toolSpecification;
    }
  }

  #region API
  /// <summary>Registers a customer tool.</summary>
  /// <remarks>
  /// Call this method from the configurator to define the tools of your mod. Each tool class can be bound only once, or
  /// an exception will be thrown.
  /// </remarks>
  /// <param name="containerDefinition">The configurator interface.</param>
  /// <param name="typeName">
  /// The tool type as specified in the TimberAPI specification. Can be omitted, in which case the name of the tool
  /// class will be used. The same name cannot be bound to different classes.
  /// </param>
  /// <typeparam name="TTool">the class that implements the tool.</typeparam>
  public static void BindTool<TTool>(IContainerDefinition containerDefinition, string typeName = null)
      where TTool : CustomTool {
    containerDefinition.Bind<TTool>().AsSingleton();
    containerDefinition.MultiBind<IToolFactory>().ToInstance(new Factory<TTool>(typeName ?? typeof(TTool).Name));
  }
  #endregion

  #region Implementation
  class Factory<TTool> : IToolFactory where TTool : CustomTool {
    public string Id { get; }
    
    public Factory(string id) {
      Id = id;
    }

    public Tool Create(ToolSpecification toolSpecification, ToolGroup toolGroup = null) {
      var tool = DependencyContainer.GetInstance<TTool>();
      tool.InitializeTool(toolGroup, toolSpecification);
      return tool;
    }
  }
  #endregion
}

}
