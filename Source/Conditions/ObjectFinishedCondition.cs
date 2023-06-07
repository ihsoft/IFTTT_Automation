// Timberborn Utils
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using UnityEngine;

namespace Automation.Conditions {

public sealed class ObjectFinishedCondition : AutomationConditionBase {
  public ObjectFinishedCondition(AutomationBehavior source) : base(
      nameof(ObjectFinishedCondition), source) {
  }

  public override bool IsValid() {
    return !Source.GetComponentFast<BlockObject>().Finished;
  }

  public override string GetUiDescription() {
    return "<SolidHighlight>construction complete</SolidHighlight>";
  }

  public override void SetupComponents(BaseInstantiator baseInstantiator) {
    var behavior = Source.GetComponentFast<ObjectFinishedConditionBehavior>()
        ?? baseInstantiator.AddComponent<ObjectFinishedConditionBehavior>(Source.GameObjectFast);
    behavior.AddCondition(this);
  }

  public override void ClearComponents() {
    Source.GetComponentFast<ObjectFinishedConditionBehavior>().DeleteCondition(this);
  }
}

}
