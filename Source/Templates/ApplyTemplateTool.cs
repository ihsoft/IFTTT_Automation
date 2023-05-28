using Bindito.Core;
using IFTTT_Automation.Actions;
using IFTTT_Automation.Conditions;
using IFTTT_Automation.Utils;
using TimberApi.ToolSystem;
using Timberborn.BlockSystem;
using Timberborn.ToolSystem;

namespace IFTTT_Automation.Templates {

class ApplyTemplateTool : AbstractAreaSelectionTool {
  readonly ToolSpecification _toolSpecification;

  #region Tool factory class
  internal class Factory : IToolFactory {
    public string Id => "IFTTTAutomationTemplate";
    
    readonly Injections _injections;

    public Factory(Injections injections) {
      _injections = injections;
    }

    public Tool Create(ToolSpecification toolSpecification, ToolGroup toolGroup = null) {
      return new ApplyTemplateTool(toolGroup, toolSpecification, _injections);
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
    var automationBehavior = obj.GetComponentFast<AutomationBehavior>();
    var condition = new ObjectFinishedAutomationCondition(automationBehavior);
    var action = new DetonateDynamiteAutomationAction(automationBehavior, 0);
    return automationBehavior != null && automationBehavior.enabled && condition.IsValid() && action.IsValid();
  }

  protected override void OnObjectAction(BlockObject obj) {
    var automationBehavior = obj.GetComponentFast<AutomationBehavior>();
    var condition = new ObjectFinishedAutomationCondition(automationBehavior);
    var action = new DetonateDynamiteAutomationAction(automationBehavior, 2);
    automationBehavior.AddRule(condition, action);
  }
}

}
