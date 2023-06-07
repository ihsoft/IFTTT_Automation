// Timberborn Utils
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

using System.Linq;
using Timberborn.ConstructibleSystem;

namespace Automation.Conditions {

public class ObjectFinishedConditionBehavior : AutomationConditionBehaviorBase, IFinishedStateListener {
  public void OnEnterFinishedState() {
    foreach (var condition in Conditions.OfType<ObjectFinishedCondition>()) {
      condition.Trigger();
    }
  }

  public void OnExitFinishedState() {
  }
}

}
