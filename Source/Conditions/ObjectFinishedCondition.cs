// Timberborn Utils
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

using Automation.Core;
using Timberborn.BlockSystem;

namespace Automation.Conditions {

public sealed class ObjectFinishedCondition : BlockObjectConditionBase<ObjectFinishedConditionBehavior> {
  #region BlockObjectConditionBase implementation

  /// <inheritdoc/>
  public override bool ConditionState {
    get => base.ConditionState;
    internal set {
      base.ConditionState = value;
      if (ConditionState) {
        IsMarkedForCleanup = true; // Block object can be constructed only once.
      }
    }
  }

  /// <inheritdoc/>
  public override string UiDescription => "<SolidHighlight>construction complete</SolidHighlight>";

  /// <inheritdoc/>
  public override IAutomationCondition CloneDefinition() {
    return new ObjectFinishedCondition();
  }

  /// <inheritdoc/>
  public override bool IsValidAt(AutomationBehavior behavior) {
    return !behavior.GetComponentFast<BlockObject>().Finished;
  }
  #endregion
}

}
