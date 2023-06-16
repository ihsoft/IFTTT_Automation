// Timberborn Utils
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

using Automation.Core;

namespace Automation.Conditions {

public sealed class DroughtStartedCondition : WeatherTrackerConditionBase {
  #region WeatherTrackerConditionBase
  /// <inheritdoc/>
  public override string UiDescription => "<SolidHighlight>drought started</SolidHighlight>";

  /// <inheritdoc/>
  public override IAutomationCondition CloneDefinition() {
    return new DroughtStartedCondition(this);
  }
  #endregion

  #region Implementation
  /// <inheritdoc/>
  public DroughtStartedCondition() {}

  /// <inheritdoc/>
  public DroughtStartedCondition(DroughtStartedCondition src) : base(src) {}
  #endregion
}

}
