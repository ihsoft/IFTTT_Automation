// Timberborn Utils
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.PrefabSystem;

namespace Automation.Conditions {

public abstract class AutomationConditionBase : IEquatable<AutomationConditionBase> {
  public readonly string ConditionTypeId;
  public readonly AutomationBehavior Source;

  protected AutomationConditionBase(string conditionTypeId, AutomationBehavior source) {
    ConditionTypeId = conditionTypeId;
    Source = source;
  }

  public abstract void SetupComponents(BaseInstantiator baseInstantiator);
  public abstract void ClearComponents();

  public virtual bool IsValid() {
    return Source.GetComponentFast<BlockObject>().Finished;
  }

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
    var prefabName = Source.GetComponentFast<Prefab>()?.Name ?? "NOPREFAB";
    var coords = Source.GetComponentFast<BlockObject>().Coordinates;
    return $"[Condition{{type={ConditionTypeId};source={prefabName}@{coords}}}]";
  }
}

}
