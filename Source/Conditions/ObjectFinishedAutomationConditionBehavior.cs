using Timberborn.BaseComponentSystem;
using Timberborn.ConstructibleSystem;

namespace Automation.Conditions {

public class ObjectFinishedAutomationConditionBehavior : BaseComponent, IFinishedStateListener {
  ObjectFinishedAutomationCondition _condition;

  public void SetCondition(ObjectFinishedAutomationCondition condition) {
    _condition = condition;
  }

  public void OnEnterFinishedState() {
    _condition.Trigger();
  }

  public void OnExitFinishedState() {
  }
}

}
