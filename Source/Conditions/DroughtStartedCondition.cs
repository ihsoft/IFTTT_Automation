// Timberborn Utils
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

using Timberborn.Localization;

namespace Automation.Conditions {

public class DroughtStartedCondition : DroughtConditionBase {
  public DroughtStartedCondition(AutomationBehavior source) : base(nameof(DroughtStartedCondition), source) {
  }

  /// <inheritdoc/>
  public override string GetUiDescription(ILoc loc) {
    return "<SolidHighlight>drought started</SolidHighlight>";
  }
}

}
