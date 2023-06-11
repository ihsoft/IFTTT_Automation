// Timberborn Utils
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

using Automation.Core;
using Timberborn.BlockSystem;
using Timberborn.Localization;

namespace Automation.Conditions {

public sealed class ObjectFinishedCondition : AutomationConditionBase<ObjectFinishedConditionBehavior> {
  /// <inheritdoc/>
  public ObjectFinishedCondition() {
  }

  /// <inheritdoc/>
  ObjectFinishedCondition(ObjectFinishedCondition src) : base(src) {
  }

  /// <inheritdoc/>
  public override AutomationConditionBase Clone() {
    return new ObjectFinishedCondition(this);
  }

  /// <inheritdoc/>
  public override bool IsValidAt(AutomationBehavior behavior) {
    return !behavior.GetComponentFast<BlockObject>().Finished;
  }

  /// <inheritdoc/>
  public override string GetUiDescription(ILoc loc) {
    return "<SolidHighlight>construction complete</SolidHighlight>";
  }
}

}
