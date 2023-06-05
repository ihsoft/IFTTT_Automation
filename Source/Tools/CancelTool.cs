// Timberborn Mod: Automation
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

using Automation.Utils;
using Timberborn.BlockSystem;

namespace Automation.Tools {

// ReSharper disable once ClassNeverInstantiated.Global
sealed class CancelTool : AbstractAreaSelectionTool {
  protected override bool ObjectFilterExpression(BlockObject blockObject) {
    var automationBehavior = blockObject.GetComponentFast<AutomationBehavior>();
    return automationBehavior != null && automationBehavior.enabled && automationBehavior.HasRules;
  }

  protected override void OnObjectAction(BlockObject blockObject) {
    blockObject.GetComponentFast<AutomationBehavior>().ClearRules();
  }
}

}
