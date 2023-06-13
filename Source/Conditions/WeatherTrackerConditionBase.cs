// Timberborn Utils
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

using Automation.Core;
using UnityDev.LogUtils;

namespace Automation.Conditions {

public abstract class DroughtConditionBase : AutomationConditionBase {
  /// <inheritdoc/>
  protected DroughtConditionBase() {}

  /// <inheritdoc/>
  protected DroughtConditionBase(DroughtConditionBase src) : base(src) {}

  /// <inheritdoc/>
  public override void OnBeforeRuleAssociationChange(AutomationBehavior newBehavior) {
    if (newBehavior != null) {
      newBehavior.AutomationService.GetGlobalBehavior<WeatherTrackerBehavior>().AddCondition(this);
    } else {
      Rule.Behavior.AutomationService.GetGlobalBehavior<WeatherTrackerBehavior>().DeleteCondition(this);
    }
  }

  /// <inheritdoc/>
  public override bool IsValidAt(AutomationBehavior behavior) {
    return true;
  }
}

}
