// Timberborn Utils
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

namespace Automation.Conditions {

/// <summary>Base class for an condition that is controlled by a block object behavior.</summary>
/// <remarks>Block object behavior lives as a component on the same object which the condition belongs to.</remarks>
/// <typeparam name="T">type of the behavior component</typeparam>
public abstract class BlockObjectConditionBase<T> : AutomationConditionBase where T : AutomationConditionBehaviorBase {
  #region AutomationConditionBase implementanton
  /// <inheritdoc/>
  protected override void OnBehaviorAssigned() {
    var behavior = Behavior.GetComponentFast<T>()
        ?? Behavior.AutomationService.BaseInstantiator.AddComponent<T>(Behavior.GameObjectFast);
    behavior.AddCondition(this);
  }

  /// <inheritdoc/>
  protected override void OnBehaviorToBeCleared() {
    Behavior.GetComponentFast<T>().DeleteCondition(this);
  }
  #endregion

  #region Implementation
  /// <inheritdoc/>
  protected BlockObjectConditionBase() {}

  /// <inheritdoc/>
  protected BlockObjectConditionBase(BlockObjectConditionBase<T> src) : base(src) {}
  #endregion
}

}
