using Automation.Conditions;
using Automation.Core;
using Timberborn.BlockSystem;
using Timberborn.BuildingsBlocking;
using Timberborn.Localization;

namespace Automation.Actions {

/// <summary>Action that resumes a pausable building.</summary>
/// <remarks>Due to any construction site is pausable, this action can only be applied to a finished building.</remarks>
public class UnpauseAction : AutomationActionBase {
  #region AutomationActionBase overrides

  /// <inheritdoc/>
  public UnpauseAction() {
  }

  /// <inheritdoc/>
  public UnpauseAction(UnpauseAction src) : base(src) {
  }

  /// <inheritdoc/>
  public override IAutomationAction CloneDefinition() {
    return new UnpauseAction(this);
  }

  /// <inheritdoc/>
  public override string UiDescription => "<SolidHighlight>unpause building</SolidHighlight>";

  /// <inheritdoc/>
  public override bool IsValidAt(AutomationBehavior behavior) {
    if (!behavior.GetComponentFast<BlockObject>().Finished) {
      return false;
    }
    var component = behavior.GetComponentFast<PausableBuilding>();
    return component != null && component.IsPausable();
  }

  /// <inheritdoc/>
  public override void OnConditionState(IAutomationCondition automationCondition) {
    var component = Behavior.GetComponentFast<PausableBuilding>();
    if (component.Paused) {
      component.Resume();
    }
  }
  #endregion
}

}
