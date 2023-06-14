// Timberborn Utils
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

using Timberborn.Localization;

namespace Automation.Conditions {

public sealed class DroughtStartedCondition : WeatherTrackerConditionBase {
  /// <inheritdoc/>
  public DroughtStartedCondition() {
  }

  /// <inheritdoc/>
  public DroughtStartedCondition(DroughtStartedCondition src) : base(src) {
  }

  /// <inheritdoc/>
  public override AutomationConditionBase Clone() {
    return new DroughtStartedCondition(this);
  }

  /// <inheritdoc/>
  public override string GetUiDescription(ILoc loc) {
    return "<SolidHighlight>drought started</SolidHighlight>";
  }
}

}
