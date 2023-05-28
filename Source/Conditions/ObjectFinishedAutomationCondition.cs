using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;

namespace IFTTT_Automation.Conditions {

public sealed class ObjectFinishedAutomationCondition : AutomationConditionBase {
  public ObjectFinishedAutomationCondition(AutomationBehavior source) : base(
      nameof(ObjectFinishedAutomationCondition), source) {
  }

  public override bool IsValid() {
    return Source != null && !Source.GetComponentFast<BlockObject>().Finished;
  }

  public override void SetupComponents(BaseInstantiator baseInstantiator) {
    baseInstantiator.AddComponent<ObjectFinishedAutomationConditionBehavior>(Source.GameObjectFast)
        .SetCondition(this);
  }
}

}
