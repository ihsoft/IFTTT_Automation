using Automation.Conditions;
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
  public override AutomationActionBase Clone() {
    return new UnpauseAction(this);
  }

  /// <inheritdoc/>
  public override string GetUiDescription(ILoc loc) {
    return "<SolidHighlight>unpause building</SolidHighlight>";
  }

  /// <inheritdoc/>
  public override bool IsValid() {
    if (!Target.GetComponentFast<BlockObject>().Finished) {
      return false;
    }
    var component = Target.GetComponentFast<PausableBuilding>();
    return component != null && component.IsPausable();
  }

  /// <inheritdoc/>
  public override void Execute(AutomationConditionBase triggerCondition) {
    var component = Target.GetComponentFast<PausableBuilding>();
    if (component.Paused) {
      component.Resume();
    }
  }
  #endregion
}

}
