using Bindito.Core;
using IFTTT_Automation.Actions;
using IFTTT_Automation.Conditions;
using IFTTT_Automation.Utils;
using TimberApi.DependencyContainerSystem;
using TimberApi.ToolSystem;
using Timberborn.BlockSystem;
using Timberborn.ToolSystem;
using UnityDev.LogUtils;
using UnityEngine;

namespace IFTTT_Automation.Templates {

class ApplyTemplateTool : AbstractAreaSelectionTool {
  readonly ToolSpecification _toolSpecification;

  #region Tool configuration
  class Factory : IToolFactory {
    public string Id => "IFTTTAutomationTemplate";
    
    readonly Injections _injections;

    public Factory(Injections injections) {
      _injections = injections;
    }

    public Tool Create(ToolSpecification toolSpecification, ToolGroup toolGroup = null) {
      return new ApplyTemplateTool(toolGroup, toolSpecification, _injections);
    }
  }

  /// <summary>Configures this tool bindings.</summary>
  public new static void Configure(IContainerDefinition containerDefinition) {
    AbstractAreaSelectionTool.Configure(containerDefinition);
    containerDefinition.MultiBind<IToolFactory>().To<Factory>().AsSingleton();
  }
  #endregion

  ApplyTemplateTool(ToolGroup toolGroup, ToolSpecification toolSpecification, Injections injections) : base(
      toolGroup, injections, injections.Colors.PriorityHighlightColor, injections.Colors.PriorityActionColor,
      injections.Colors.PriorityTileColor, injections.Colors.PrioritySideColor) {
    _toolSpecification = toolSpecification;
  }

  #region Tool overrides
  public override ToolDescription Description() {
    return new ToolDescription.Builder()
        .AddPrioritizedSection("Just section")
        .AddPrioritizedSection("It's in progress")
        .Build();
  }
  #endregion

  #region AbstractAreaSelectionTool overries
  /// <inheritdoc/>
  protected override bool ObjectFilterExpression(BlockObject blockObject, bool singleElement) {
    var automationBehavior = blockObject.GetComponentFast<AutomationBehavior>();
    var condition = new ObjectFinishedAutomationCondition(automationBehavior);
    var action = new DetonateDynamiteAutomationAction(automationBehavior, 0);
    return automationBehavior != null && automationBehavior.enabled && condition.IsValid() && action.IsValid();
  }

  /// <inheritdoc/>
  protected override void OnObjectAction(BlockObject blockObject, bool singleElement) {
    var automationBehavior = blockObject.GetComponentFast<AutomationBehavior>();
    var condition = new ObjectFinishedAutomationCondition(automationBehavior);
    var action = new DetonateDynamiteAutomationAction(automationBehavior, 2);
    automationBehavior.AddRule(condition, action);
  }

  /// <inheritdoc/>
  protected override void OnSelectionStarted(BlockObject blockObject, Vector3Int position) {
  }
  #endregion
}

}
