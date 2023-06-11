// Timberborn Mod: Automation
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

using Automation.Actions;
using Automation.Conditions;
using Automation.Utils;
using Timberborn.BlockSystem;
using Timberborn.Persistence;
using Timberborn.PrefabSystem;

namespace Automation.Core {

/// <summary>The base item of the automation.</summary>
/// <remarks>
/// Each rules defines and executes a term: "if &lt;condition> is true, then execute this &lt;action>"
/// </remarks>
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
  /// <summary>Serializer that handles persistence the rules.</summary>
  public static readonly StaticClassSerializer<AutomationRule> RuleSerializer = new();
  public AutomationConditionBase Condition { get; private set; }
  public AutomationActionBase Action { get; private set; }
  public AutomationBehavior Behavior { get; private set; }

  /// <summary>The block object this rule is attached to.</summary>
  /// <remarks>
  /// This field cannot be <c>null</c> if the rule is attached to the scene (see <see cref="Behavior"/>). However, if
  /// the rule is not attached, then here will be no block object.
  /// </remarks>
  public BlockObject BlockObject { get; private set; }

  /// <summary>Needed for the persistence.</summary>
  public AutomationRule() {}

  /// <summary>Creates a rule that takes ownership on the provided condition and action.</summary>
  public AutomationRule(AutomationConditionBase condition, AutomationActionBase action) {
    Condition = condition;
    condition.Rule = this;
    Action = action;
    action.Rule = this;
  }

  /// <summary>Associates this rule with a specific automation behavior.</summary>
  /// <remarks>
  /// <p>
  /// This action in most cases must be assumed to be <i>final</i>. Once the rule is attached to a behavior, all the
  /// boilerplate to handle this rule will also be setup. It may include very expensive code logic.
  /// </p>
  /// <p>
  /// If a rule needs to be deleted, it must first be <b>detached</b> from he automation behavior. This can be done by
  /// "attaching" to <c>null</c>.
  /// </p>
  /// </remarks>
  /// <param name="obj">The new automation behavior or <c>null</c> if the link is being removed.</param>
  public void AttachToBehavior(AutomationBehavior obj) {
    if (Behavior == obj) {
      return;
    }
    Condition.OnBeforeRuleAssociationChange(obj);
    Behavior = obj;
    BlockObject = obj != null ? obj.GetComponentFast<BlockObject>() : null;
  }

  /// <summary>Verifies if the rule makes sense on the provided automation behavior.</summary>
  /// <remarks>This check must not result in any state changes of the rule, condition or action.</remarks>
  /// <param name="obj">The automation behavior candidate.</param>
  public bool IsValidAt(AutomationBehavior obj) {
    return Condition.IsValidAt(obj) && Action.IsValidAt(obj);
  }

  /// <summary>Returns a rule that is not bound to any game objects.</summary>
  public AutomationRule Clone() {
    return new AutomationRule(Condition.Clone(), Action.Clone());
  }
  #endregion

  #region Implentation
  /// <inheritdoc/>
  public override string ToString() {
    if (Behavior == null) {
      return $"[Rule:condition=[{Condition}];action=[{Action}];at=NULL]";
    }
    var prefabName = Behavior.GetComponentFast<Prefab>()?.Name ?? "UNKNOWN";
    var coords = Behavior.GetComponentFast<BlockObject>().Coordinates;
    return $"[Rule:condition=[{Condition}];action=[{Action}];at={prefabName}@{coords}]";
  }
  #endregion
}

}
