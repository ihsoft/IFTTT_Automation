// Timberborn Mod: Automation
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

using Automation.Actions;
using Automation.Conditions;
using Timberborn.Persistence;
using UnityDev.LogUtils;

namespace Automation {

public sealed class AutomationRule {
  #region Class serializer
  static readonly PropertyKey<AutomationConditionBase> ConditionPropertyKey = new("Condition");
  static readonly IObjectSerializer<AutomationConditionBase> ConditionPropertySerializer =
      AutomationConditionBase.ConditionSerializer;
  static readonly PropertyKey<AutomationActionBase> ActionPropertyKey = new("Action");
  static readonly IObjectSerializer<AutomationActionBase> ActionPropertySerializer =
      AutomationActionBase.ActionSerializer;

  class SerializerImpl : IObjectSerializer<AutomationRule> {
    public void Serialize(AutomationRule value, IObjectSaver objectSaver) {
      objectSaver.Set(ConditionPropertyKey, value.Condition, ConditionPropertySerializer);
    }

    public Obsoletable<AutomationRule> Deserialize(IObjectLoader objectLoader) {
      var rule = new AutomationRule {
          Condition = objectLoader.Get(ConditionPropertyKey, ConditionPropertySerializer),
          Action = objectLoader.Get(ActionPropertyKey, ActionPropertySerializer)
      };
      return new Obsoletable<AutomationRule>(rule);
    }
  }
  #endregion

  #region API
  public static readonly IObjectSerializer<AutomationRule> Serializer = new SerializerImpl();

  public AutomationConditionBase Condition;
  public AutomationActionBase Action;
  public bool IsValid => Condition != null && Condition.IsValid() && Action != null && Action.IsValid();

  /// <summary>Returns a rule that is not bound to any game objects.</summary>
  public AutomationRule Clone() {
    return new AutomationRule { Condition = Condition.Clone(), Action = Action.Clone() };
  }
  #endregion

  #region Implentation
  public override string ToString() {
    return $"[Rule:condition={Condition};action={Action}]";
  }
  #endregion
}

}
