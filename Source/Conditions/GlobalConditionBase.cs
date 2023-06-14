using Automation.Core;

namespace Automation.Conditions {

/// <summary>Base class for an condition that is controlled by a global behavior.</summary>
/// <remarks>Global behaviors run as singletons in <see cref="AutomationService"/>.</remarks>
/// <typeparam name="T">type of the behavior component</typeparam>
public abstract class GlobalConditionBase<T> : AutomationConditionBase where T : AutomationConditionBehaviorBase {
  /// <inheritdoc/>
  protected GlobalConditionBase() {}

  /// <inheritdoc/>
  protected GlobalConditionBase(GlobalConditionBase<T> src) : base(src) {}

  /// <inheritdoc/>
  public override void OnBeforeRuleAssociationChange(AutomationBehavior newBehavior) {
    if (newBehavior != null) {
      newBehavior.AutomationService.GetGlobalBehavior<T>().AddCondition(this);
    } else {
      Rule.Behavior.AutomationService.GetGlobalBehavior<T>().DeleteCondition(this);
    }
  }
}

}
