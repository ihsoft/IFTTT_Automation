using System;
using Timberborn.BaseComponentSystem;

namespace Automation.Conditions {

public abstract class AutomationConditionBase : IEquatable<AutomationConditionBase> {
  public readonly string ConditionTypeId;
  public readonly AutomationBehavior Source;

  protected AutomationConditionBase(string conditionTypeId, AutomationBehavior source) {
    ConditionTypeId = conditionTypeId;
    Source = source;
  }

  public abstract bool IsValid();
  public abstract void SetupComponents(BaseInstantiator baseInstantiator);

  public virtual void Trigger() {
    Source.TriggerAction(this);
    // FIXME: in overrides
    // Listener.InvalidateCondition(this);
  }


  public virtual bool Equals(AutomationConditionBase other) {
    return other != null
        && ConditionTypeId == other.ConditionTypeId
        && Source == other.Source;
  }

  public override string ToString() {
    return $"[Condition{{type={ConditionTypeId};source={Source}}}]";
  }
}

}
