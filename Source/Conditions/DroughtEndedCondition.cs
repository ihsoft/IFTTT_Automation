// Timberborn Utils
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

using Timberborn.Localization;

namespace Automation.Conditions {

public sealed class DroughtEndedCondition : DroughtConditionBase {
  /// <inheritdoc/>
  public DroughtEndedCondition() {
  }

  /// <inheritdoc/>
  public DroughtEndedCondition(DroughtEndedCondition src) : base(src) {
  }

  /// <inheritdoc/>
  public override AutomationConditionBase Clone() {
    return new DroughtEndedCondition(this);
  }

  /// <inheritdoc/>
  public override string GetUiDescription(ILoc loc) {
    return "<SolidHighlight>drought ended</SolidHighlight>";
  }
}

}
