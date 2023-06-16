// Timberborn Utils
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

namespace Automation.Conditions {

public abstract class WeatherTrackerConditionBase : GlobalConditionBase<WeatherTrackerBehavior> {
  #region Implementation
  /// <inheritdoc/>
  protected WeatherTrackerConditionBase() {}

  /// <inheritdoc/>
  protected WeatherTrackerConditionBase(WeatherTrackerConditionBase src) : base(src) {}
  #endregion
}

}
