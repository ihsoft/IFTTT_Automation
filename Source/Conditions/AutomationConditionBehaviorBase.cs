// Timberborn Utils
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

using System.Collections.Generic;
using Timberborn.BaseComponentSystem;

namespace Automation.Conditions {

public class AutomationConditionBehaviorBase : BaseComponent {
  protected IReadOnlyCollection<AutomationConditionBase> Conditions => _conditions.AsReadOnly();
  readonly List<AutomationConditionBase> _conditions = new();

  public void AddCondition(AutomationConditionBase condition) {
    _conditions.Add(condition);
  }

  public void DeleteCondition(AutomationConditionBase condition) {
    _conditions.Remove(condition);
    if (_conditions.Count == 0) {
      Destroy(this);
    }
  }
}

}
