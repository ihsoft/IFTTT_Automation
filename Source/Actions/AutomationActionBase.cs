// Timberborn Utils
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

using System;
using Automation.Conditions;
using Timberborn.BlockSystem;
using Timberborn.Localization;
using Timberborn.PrefabSystem;

namespace Automation.Actions {

public abstract class AutomationActionBase : IEquatable<AutomationActionBase> {
  public readonly string ActionTypeId;
  public readonly AutomationBehavior Target;

  protected AutomationActionBase(string actionTypeId, AutomationBehavior target) {
    ActionTypeId = actionTypeId;
    Target = target;
  }

  /// <summary>Returns a localized string to present as description of the condition.</summary>
  /// <remarks>The string must fully describe what the condition checks, but be as short as possible.</remarks>
  public abstract string GetUiDescription(ILoc loc);

  /// <summary>Verifies that the action can be used in its current setup.</summary>
  public abstract bool IsValid();

  // FIXME: in overrides
  // Listener.InvalidateAction(this);
  public abstract void Execute(AutomationConditionBase triggerCondition);

  public virtual bool Equals(AutomationActionBase other) {
    return other != null && ActionTypeId == other.ActionTypeId && Target == other.Target;
  }

  public override string ToString() {
    var prefabName = Target.GetComponentFast<Prefab>()?.Name ?? "NOPREFAB";
    var coords = Target.GetComponentFast<BlockObject>().Coordinates;
    return $"[Condition{{type={ActionTypeId};target={prefabName}@{coords}}}]";
  }
}

}
