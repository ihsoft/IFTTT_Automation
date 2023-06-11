// Timberborn Utils
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

using Automation.Conditions;
using Automation.Core;
using Timberborn.BlockSystem;
using Timberborn.BuildingsBlocking;
using Timberborn.Localization;

namespace Automation.Actions {

/// <summary>Action that pauses a pausable building.</summary>
/// <remarks>Due to any construction site is pausable, this action acn only be applied to a finished building.</remarks>
public class PauseAction : AutomationActionBase {
  #region AutomationActionBase overrides
  /// <inheritdoc/>
  public PauseAction() {
  }

  /// <inheritdoc/>
  public PauseAction(PauseAction src) : base(src) {
  }

  /// <inheritdoc/>
  public override AutomationActionBase Clone() {
    return new PauseAction(this);
  }

  /// <inheritdoc/>
  public override string GetUiDescription(ILoc loc) {
    return "<SolidHighlight>pause building</SolidHighlight>";
  }

  /// <inheritdoc/>
  public override bool IsValidAt(AutomationBehavior behavior) {
    if (!behavior.GetComponentFast<BlockObject>().Finished) {
      return false;
    }
    var component = behavior.GetComponentFast<PausableBuilding>();
    return component != null && component.IsPausable();
  }

  /// <inheritdoc/>
  public override void Execute(AutomationConditionBase triggerCondition) {
    var component = Rule.Behavior.GetComponentFast<PausableBuilding>();
    if (!component.Paused) {
      component.Pause();
    }
  }
  #endregion
}

}
