// Timberborn Mod: Automation
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

using Automation.Actions;
using Automation.Conditions;
using Automation.Utils;
using Timberborn.BlockSystem;
using UnityEngine;

namespace Automation.Templates {

// ReSharper disable once ClassNeverInstantiated.Global
class ApplyTemplateTool : AbstractAreaSelectionTool, IAutomationModeEnabler {
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
    automationBehavior.ClearRules();
    automationBehavior.AddRule(condition, action);
  }

  /// <inheritdoc/>
  protected override void Initialize() {
    //FIXME: et color from system or make it globally adjustable 
    //SetColorSchema(Color.cyan, Color.cyan, Color.cyan, Color.cyan);
    SetColorSchema(ToolColor, ToolColor, Color.white, Color.white);
    base.Initialize();
  }

  static readonly Color ToolColor = new(0, 1, 1, 0.7f);

  #endregion
}

}
