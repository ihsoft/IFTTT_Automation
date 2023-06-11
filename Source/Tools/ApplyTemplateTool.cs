// Timberborn Mod: Automation
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

using System.Collections.ObjectModel;
using System.Linq;
using Automation.Core;
using Automation.Utils;
using Timberborn.BlockSystem;
using Timberborn.Persistence;
using UnityEngine;

namespace Automation.Templates {

// ReSharper disable once ClassNeverInstantiated.Global
sealed class ApplyTemplateTool : AbstractAreaSelectionTool, IAutomationModeEnabler {
  const string UnityCursorName = "igorz.automation/cursors/cog-cursor-large";
  static readonly Color ToolColor = new(0, 1, 1, 0.7f);

  #region Tool information
  static readonly ListKey<AutomationRule> RulesListKey = new("Rules");
  public sealed class ToolInfo : CustomToolSystem.ToolInformation {
    public ReadOnlyCollection<AutomationRule> Rules { get; private set; }

    /// <inheritdoc/>
    public override void Load(IObjectLoader objectLoader) {
      Rules = objectLoader.Get(RulesListKey, AutomationRule.RuleSerializer).AsReadOnly();
    }
  }
  #endregion

  #region AbstractAreaSelectionTool overries
  /// <inheritdoc/>
  protected override bool ObjectFilterExpression(BlockObject blockObject) {
    var automationBehavior = blockObject.GetComponentFast<AutomationBehavior>();
    if (automationBehavior == null || !automationBehavior.enabled) {
      return false;
    }
    var info = (ToolInfo) ToolInformation;
    return info.Rules.All(rule => rule.IsValidAt(automationBehavior));
  }

  /// <inheritdoc/>
  protected override void OnObjectAction(BlockObject blockObject) {
    var automationBehavior = blockObject.GetComponentFast<AutomationBehavior>();
    automationBehavior.ClearRules();
    var info = (ToolInfo) ToolInformation;
    foreach (var rule in info.Rules) {
      automationBehavior.AddRule(rule.Clone());
    }
  }

  /// <inheritdoc/>
  protected override void Initialize() {
    SetColorSchema(ToolColor, ToolColor, Color.cyan, Color.cyan);
    SetUnityCursor(UnityCursorName);
    base.Initialize();
  }
  #endregion
}

}
