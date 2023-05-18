using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.ConstructibleSystem;
using UnityDev.LogUtils;
using UnityEngine;
using UnityEngine.Windows.Speech;

namespace IFTTT_Automation {

public interface IAutomationCondition {
  bool IsValid { get; }
  bool IsHandled { get; }
  bool IsTriggered { get; }
}

public interface IAutomationAction {
  bool Execute();
}

public interface IAutomationRule {
  IAutomationCondition IfCondition { get; }
  IAutomationAction ThenAction { get; }
}

public class IFTTTAutomationBehaviorBase : BaseComponent {
}

public class IFTTTAutomationTestRule : IFTTTAutomationBehaviorBase, IFinishedStateListener {
  public string RuleTypeId => "TestRuleId";

  bool _conditionTriggered;
  bool _conditionHandled;
  bool _conditionBeingHandled;
  bool _invalidState;

  List<IAutomationAction> _actions = new();

  void Awake() {
    HostedDebugLog.Warning(TransformFast, "*** starting IFTTT rule");
  }

  public void OnEnterFinishedState() {
    if (_conditionBeingHandled) {
      HostedDebugLog.Error(TransformFast, "Unexpected chain of conditions");
      return;
    }
    _conditionTriggered = true;
    _conditionBeingHandled = true;
    HostedDebugLog.Error(TransformFast, "Trigger {0} actions", _actions.Count);
    foreach (var action in _actions) {
      action.Execute();
    }
    _conditionBeingHandled = false;
    _conditionHandled = true;
  }

  public void OnExitFinishedState() {
    _invalidState = true;
  }
}

}
