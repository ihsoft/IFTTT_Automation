using IFTTT_Automation.Actions;
using IFTTT_Automation.Conditions;
using IFTTT_Automation.Utils;
using Timberborn.BlockSystem;
using Timberborn.ToolSystem;
using UnityEngine;

namespace IFTTT_Automation.Templates {

class ApplyTemplateTool : AbstractAreaSelectionTool {
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
