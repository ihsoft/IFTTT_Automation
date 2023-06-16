// Timberborn Utils
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

using Automation.Core;

namespace Automation.Conditions {

public sealed class DroughtEndedCondition : WeatherTrackerConditionBase {
  #region WeatherTrackerConditionBase implemenantion
  /// <inheritdoc/>
  public override string UiDescription => "<SolidHighlight>drought ended</SolidHighlight>";

  /// <inheritdoc/>
  public override IAutomationCondition CloneDefinition() {
    return new DroughtEndedCondition(this);
  }
  #endregion

  #region Implemenatation
  /// <inheritdoc/>
  public DroughtEndedCondition() {}

  /// <inheritdoc/>
  public DroughtEndedCondition(DroughtEndedCondition src) : base(src) {}
  #endregion
}

}
