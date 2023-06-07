// Timberborn Utils
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

using Timberborn.BlockSystem;

namespace Automation.Conditions {

public sealed class ObjectFinishedCondition : AutomationConditionBase<ObjectFinishedConditionBehavior> {
  public ObjectFinishedCondition(AutomationBehavior source) : base(
      nameof(ObjectFinishedCondition), source) {
  }

  public override bool IsValid() {
    return !Source.GetComponentFast<BlockObject>().Finished;
  }

}

}
