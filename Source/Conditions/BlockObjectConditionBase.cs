using Automation.Core;

namespace Automation.Conditions {

/// <summary>Base class for an condition that is controlled by a block object behavior.</summary>
/// <remarks>Block object behavior lives as a component on the same object which the condition belongs to.</remarks>
/// <typeparam name="T">type of the behavior component</typeparam>
public abstract class BlockObjectConditionBase<T> : AutomationConditionBase where T : AutomationConditionBehaviorBase {
  /// <inheritdoc/>
  protected BlockObjectConditionBase() {}

  /// <inheritdoc/>
  protected BlockObjectConditionBase(BlockObjectConditionBase<T> src) : base(src) {}

  /// <inheritdoc/>
  public override void OnBeforeRuleAssociationChange(AutomationBehavior newBehavior) {
    if (newBehavior != null) {
      var behavior = newBehavior.GetComponentFast<T>()
          ?? newBehavior.BaseInstantiator.AddComponent<T>(newBehavior.GameObjectFast);
      behavior.AddCondition(this);
    } else {
      Rule.Behavior.GetComponentFast<T>().DeleteCondition(this);
    }
  }
}

}
