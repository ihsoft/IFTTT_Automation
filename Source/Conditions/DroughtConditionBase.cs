// Timberborn Utils
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

using Automation.Core;

namespace Automation.Conditions {

public abstract class DroughtConditionBase : AutomationConditionBase<DroughtTrackerBehavior> {
  /// <inheritdoc/>
  protected DroughtConditionBase() {
  }

  /// <inheritdoc/>
  protected DroughtConditionBase(DroughtConditionBase src) : base(src) {
  }

  /// <inheritdoc/>
  public override bool IsValidAt(AutomationBehavior behavior) {
    return true;
  }
}

}
