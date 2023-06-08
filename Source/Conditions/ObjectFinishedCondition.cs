// Timberborn Utils
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

using Timberborn.BlockSystem;
using Timberborn.Localization;

namespace Automation.Conditions {

public sealed class ObjectFinishedCondition : AutomationConditionBase<ObjectFinishedConditionBehavior> {
  /// <inheritdoc/>
  public override bool IsValid() {
    return !Source.GetComponentFast<BlockObject>().Finished;
  }

  /// <inheritdoc/>
  public override string GetUiDescription(ILoc loc) {
    return "<SolidHighlight>construction complete</SolidHighlight>";
  }
}

}
