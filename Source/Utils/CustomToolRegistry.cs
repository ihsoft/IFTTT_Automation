// Timberborn Utils
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

using Bindito.Core;
using TimberApi.DependencyContainerSystem;
using TimberApi.ToolGroupSystem;
using TimberApi.ToolSystem;
using Timberborn.ConstructionMode;
using Timberborn.ToolSystem;
using UnityDev.LogUtils;
using ToolGroupSpecification = TimberApi.ToolGroupSystem.ToolGroupSpecification;

namespace IFTTT_Automation.Utils {

public static class CustomToolRegistry {

  /// <summary>Base class for all custom tool groups.</summary>
  public class CustomToolGroup : ToolGroup, IToolGroup {

    #region IToolGroup implementation
    public string Id => _specification.Id;
    public string GroupId => _specification.GroupId;
    public int Order => _specification.Order;
    public string Section => _specification.Section;
    public bool DevMode => _specification.DevMode;
    #endregion

    ToolGroupSpecification _specification;

    /// <summary>Initializes the tool group. Do all logic here instead of the constructor.</summary>
    /// <param name="specification">The specification of this tool group from TimberAPI.</param>
    public virtual void InitializeGroup(ToolGroupSpecification specification) {
      Icon = specification.Icon;
      DisplayNameLocKey = specification.NameLocKey;
      _specification = specification;
    }
  }

  /// <summary>Base class for all tool groups that need construction mode enabled.</summary>
  public class CustomToolGroupWithConstructionModeEnabler : CustomToolGroup, IConstructionModeEnabler {
  }

  /// <summary>Base class for all custom tools.</summary>
  public abstract class CustomTool : Tool {
    protected ToolSpecification ToolSpecification;

    /// <summary>Initializes the tool. Do all logic here instead of the constructor.</summary>
    /// <param name="toolGroup">The group this tool is a child of.</param>
    /// <param name="toolSpecification">The specification of this tool from TimberAPI.</param>
    public virtual void InitializeTool(ToolGroup toolGroup, ToolSpecification toolSpecification) {
      ToolGroup = toolGroup;
      ToolSpecification = toolSpecification;
    }
  }

  #region API
  public static void BindGroupTrivial(IContainerDefinition containerDefinition, string typeName) {
    containerDefinition.MultiBind<IToolGroupFactory>().ToInstance(new ToolGroupFactory<CustomToolGroup>(typeName));
  }

  public static void BindGroupWithConstructionModeEnabler(IContainerDefinition containerDefinition, string typeName) {
    containerDefinition.MultiBind<IToolGroupFactory>()
        .ToInstance(new ToolGroupFactory<CustomToolGroupWithConstructionModeEnabler>(typeName));
  }

  public static void BindToolGroup<TToolGroup>(IContainerDefinition containerDefinition, string typeName = null)
      where TToolGroup : CustomToolGroup {
    containerDefinition.Bind<TToolGroup>().AsSingleton();
    containerDefinition.MultiBind<IToolGroupFactory>()
        .ToInstance(new ToolGroupFactory<TToolGroup>(typeName ?? typeof(TToolGroup).FullName));
  }

  /// <summary>Registers a customer tool.</summary>
  /// <remarks>
  /// Call this method from the configurator to define the tools of your mod. Each tool class can be bound only once, or
  /// an exception will be thrown.
  /// </remarks>
  /// <param name="containerDefinition">The configurator interface.</param>
  /// <param name="typeName">
  /// The tool type as specified in the TimberAPI specification. Can be omitted, in which case the tool's full class
  /// name will be used. The same name cannot be bound to different classes.
  /// </param>
  /// <typeparam name="TTool">the class that implements the tool.</typeparam>
  public static void BindTool<TTool>(IContainerDefinition containerDefinition, string typeName = null)
      where TTool : CustomTool {
    containerDefinition.Bind<TTool>().AsSingleton();
    containerDefinition.MultiBind<IToolFactory>().ToInstance(new ToolFactory<TTool>(typeName ?? typeof(TTool).FullName));
  }
  #endregion

  #region Implementation
  class ToolGroupFactory<TToolGroup> : IToolGroupFactory where TToolGroup : CustomToolGroup {
    public string Id { get; }

    public ToolGroupFactory(string id) {
      Id = id;
    }

    public IToolGroup Create(ToolGroupSpecification toolGroupSpecification) {
      CustomToolGroup group;
      if (typeof(TToolGroup) == typeof(CustomToolGroup)) {
        group = new CustomToolGroup();
      } else if (typeof(TToolGroup) == typeof(CustomToolGroupWithConstructionModeEnabler)) {
        group = new CustomToolGroupWithConstructionModeEnabler();
      } else {
        group = DependencyContainer.GetInstance<TToolGroup>();
      }
      group.InitializeGroup(toolGroupSpecification);
      return group;
    }
  }

  class ToolFactory<TTool> : IToolFactory where TTool : CustomTool {
    public string Id { get; }
    
    public ToolFactory(string id) {
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
