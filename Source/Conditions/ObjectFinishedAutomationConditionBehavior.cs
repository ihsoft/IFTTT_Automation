using Timberborn.BaseComponentSystem;
using Timberborn.ConstructibleSystem;
using UnityDev.LogUtils;

namespace IFTTT_Automation.Conditions {

public class ObjectFinishedAutomationConditionBehavior : BaseComponent, IFinishedStateListener {
  ObjectFinishedAutomationCondition _condition;

  public void SetCondition(ObjectFinishedAutomationCondition condition) {
    _condition = condition;
  }

  void Awake() {
    HostedDebugLog.Warning(TransformFast, "*** Awake");
  }

  public void OnEnterFinishedState() {
    HostedDebugLog.Warning(TransformFast, "*** entered finished state");
    _condition.Trigger();
  }

  public void OnExitFinishedState() {
  }
}

}
