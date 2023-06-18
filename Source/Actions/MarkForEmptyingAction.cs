using Automation.Core;
using Timberborn.Emptying;
using UnityDev.LogUtils;

namespace Automation.Actions {

/// <summary>Action that turns the storage into the empty mode.</summary>
/// <remarks>
/// This action disables empty mode when disconnected from the automation behavior because not evey provides an has
/// explicit control over this mode.
/// </remarks>
public class MarkForEmptyingAction : AutomationActionBase {
  #region AutomationActionBase overrides
  /// <inheritdoc/>
  public override IAutomationAction CloneDefinition() {
    return new MarkForEmptyingAction();
  }

  /// <inheritdoc/>
  public override string UiDescription => "<SolidHighlight>empty storage</SolidHighlight>";

  /// <inheritdoc/>
  public override bool IsValidAt(AutomationBehavior behavior) {
    return base.IsValidAt(behavior) && behavior.GetComponentFast<Emptiable>() != null;
  }

  /// <inheritdoc/>
  public override void OnConditionState(IAutomationCondition automationCondition) {
    if (!Condition.ConditionState) {
      return;
    }
    var component = Behavior.GetComponentFast<Emptiable>();
    if (!component.IsMarkedForEmptying) {
      DebugEx.Fine("Mark for emptying: {0}", Behavior);
      component.MarkForEmptyingWithStatus();
    }
  }

  /// <inheritdoc/>
  protected override void OnBehaviorToBeCleared() {
    if (!Condition.ConditionState) {
      return;
    }
    var component = Behavior.GetComponentFast<Emptiable>();
    if (component.IsMarkedForEmptying) {
      component.UnmarkForEmptying();
    }
    base.OnBehaviorToBeCleared();
  }
  #endregion
}

}
