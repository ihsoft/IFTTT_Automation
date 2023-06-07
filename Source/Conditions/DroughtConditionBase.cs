// Timberborn Utils
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

namespace Automation.Conditions {

public abstract class DroughtConditionBase : AutomationConditionBase<DroughtTrackerBehavior> {
  protected DroughtConditionBase(string conditionTypeId, AutomationBehavior source) : base(conditionTypeId, source) {
  }

  /// <inheritdoc/>
  public override bool IsValid() {
    return true;
  }
}

}
