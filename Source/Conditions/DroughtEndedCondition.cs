// Timberborn Utils
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

using Timberborn.Localization;

namespace Automation.Conditions {

public class DroughtEndedCondition : DroughtConditionBase {
  public DroughtEndedCondition(AutomationBehavior source) : base(nameof(DroughtEndedCondition), source) {
  }

  /// <inheritdoc/>
  public override string GetUiDescription(ILoc loc) {
    return "<SolidHighlight>drought ended</SolidHighlight>";
  }
}

}
