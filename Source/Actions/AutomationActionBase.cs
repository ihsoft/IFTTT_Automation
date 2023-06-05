using System;
using Automation.Conditions;
using Timberborn.BaseComponentSystem;

namespace Automation.Actions {

public abstract class AutomationActionBase : IEquatable<AutomationActionBase> {
  public readonly string ActionTypeId;
  public readonly AutomationBehavior Target;

  protected AutomationActionBase(string actionTypeId, AutomationBehavior target) {
    ActionTypeId = actionTypeId;
    Target = target;
  }

  public abstract bool IsValid();
  
  // FIXME: in overrides
  // Listener.InvalidateAction(this);
  public abstract void Execute(AutomationConditionBase triggerCondition);

  public abstract void SetupComponents(BaseInstantiator baseInstantiator);

  public virtual bool Equals(AutomationActionBase other) {
    return other != null && ActionTypeId == other.ActionTypeId && Target == other.Target;
  }

  public override string ToString() {
    return $"[Action{{type={ActionTypeId};target={Target}}}]";
  }
}

}
