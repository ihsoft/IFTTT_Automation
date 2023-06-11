// Timberborn Mod: Automation
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

using Automation.Actions;
using Automation.Conditions;
using Automation.Utils;
using Timberborn.Persistence;

namespace Automation.Core {

public sealed class AutomationRule : IGameSerializable {
  #region Implementation of IGameSerializable
  static readonly PropertyKey<AutomationConditionBase> ConditionPropertyKey = new("Condition");
  static readonly PropertyKey<AutomationActionBase> ActionPropertyKey = new("Action");

  /// <inheritdoc/>
  public void LoadFrom(IObjectLoader objectLoader) {
    Condition = objectLoader.Get(ConditionPropertyKey, AutomationConditionBase.ConditionSerializer);
    Action = objectLoader.Get(ActionPropertyKey, AutomationActionBase.ActionSerializer);
  }

  /// <inheritdoc/>
  public void SaveTo(IObjectSaver objectSaver) {
    objectSaver.Set(ConditionPropertyKey, Condition, AutomationConditionBase.ConditionSerializer);
    objectSaver.Set(ActionPropertyKey, Action, AutomationActionBase.ActionSerializer);
  }
  #endregion

  #region API
  public static readonly StaticClassSerializer<AutomationRule> RuleSerializer = new();

  public AutomationConditionBase Condition;
  public AutomationActionBase Action;
  public bool IsValid => Condition != null && Condition.IsValid() && Action != null && Action.IsValid();

  /// <summary>Returns a rule that is not bound to any game objects.</summary>
  public AutomationRule Clone() {
    return new AutomationRule { Condition = Condition.Clone(), Action = Action.Clone() };
  }
  #endregion

  #region Implentation
  /// <inheritdoc/>
  public override string ToString() {
    return $"[Rule:condition={Condition};action={Action}]";
  }
  #endregion
}

}
