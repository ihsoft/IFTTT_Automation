using IFTTT_Automation.Actions;
using IFTTT_Automation.Conditions;
using IFTTT_Automation.Utils;
using Timberborn.BlockSystem;
using Timberborn.ToolSystem;

namespace IFTTT_Automation.Tools {

class ApplyTemplateTool : AbstractAreaSelectionTool {
  #region Tool overrides
  /// <inheritdoc/>
  public override ToolDescription Description() {
    return new ToolDescription.Builder()
        .AddPrioritizedSection("Just section")
        .AddPrioritizedSection("It's in progress")
        .Build();
  }
  #endregion

  #region AbstractAreaSelectionTool overries
  /// <inheritdoc/>
  protected override bool ObjectFilterExpression(BlockObject blockObject) {
    var automationBehavior = blockObject.GetComponentFast<AutomationBehavior>();
    var condition = new ObjectFinishedAutomationCondition(automationBehavior);
    var action = new DetonateDynamiteAutomationAction(automationBehavior, 0);
    return automationBehavior != null && automationBehavior.enabled && condition.IsValid() && action.IsValid();
  }

  /// <inheritdoc/>
  protected override void OnObjectAction(BlockObject blockObject) {
    var automationBehavior = blockObject.GetComponentFast<AutomationBehavior>();
    var condition = new ObjectFinishedAutomationCondition(automationBehavior);
    var action = new DetonateDynamiteAutomationAction(automationBehavior, 2);
    automationBehavior.AddRule(condition, action);
  }
  #endregion
}

}
