using Bindito.Core;
using IFTTT_Automation.Utils;
using TimberApi.ToolSystem;
using Timberborn.BlockSystem;
using Timberborn.ConstructibleSystem;
using Timberborn.ToolSystem;
using UnityDev.LogUtils;

namespace IFTTT_Automation.Templates {

class ApplyTemplateTool : AbstractAreaSelectionTool {
  readonly ToolSpecification _toolSpecification;

  #region Tool factory class
  internal class Factory : IToolFactory {
    readonly Injections _injections;

    public Tool Create(ToolSpecification toolSpecification, ToolGroup toolGroup = null) {
      return new ApplyTemplateTool(toolGroup, toolSpecification, _injections);
    }

    public string Id => "IFTTTAutomationTemplate";

    public Factory(Injections injections) {
      _injections = injections;
    }
  }
  #endregion

  public new static void Configure(IContainerDefinition containerDefinition) {
    AbstractAreaSelectionTool.Configure(containerDefinition);
    containerDefinition.MultiBind<IToolFactory>().To<Factory>().AsSingleton();
  }

  public ApplyTemplateTool(ToolGroup toolGroup, ToolSpecification toolSpecification, Injections injections) : base(
      toolGroup, injections, injections.Colors.PriorityHighlightColor, injections.Colors.PriorityActionColor,
      injections.Colors.PriorityTileColor, injections.Colors.PrioritySideColor) {
    _toolSpecification = toolSpecification;
  }

  public override ToolDescription Description() {
    return new ToolDescription.Builder()
        .AddPrioritizedSection("Just section")
        .AddPrioritizedSection("It's in progress")
        .Build();
  }

  protected override bool ObjectFilterExpression(BlockObject obj) {
    var component = obj.GetComponentFast<Constructible>();
    return component != null && component.enabled && component.IsUnfinished;
  }

  protected override void OnObjectAction(BlockObject obj) {
    var selectedObject = obj.GetComponentFast<Constructible>();
    DebugEx.Warning("*** selected: {0}, status={1}", selectedObject, selectedObject.ConstructionState);
    var rule = Injected.BaseInstantiator.AddComponent<IFTTTAutomationTestRule>(selectedObject.GameObjectFast);
  }
}

}
