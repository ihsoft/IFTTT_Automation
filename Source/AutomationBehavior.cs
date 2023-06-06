// Timberborn Mod: Automation
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

using System.Collections.Generic;
using System.Linq;
using Automation.Actions;
using Automation.Conditions;
using Bindito.Core;
using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.Persistence;
using UnityDev.LogUtils;

namespace Automation {

public class AutomationBehavior : BaseComponent, IPersistentEntity, IInitializableEntity {
  BaseInstantiator _baseInstantiator;
  AutomationService _automationService;

  class Rule {
    public AutomationConditionBase condition;
    public AutomationActionBase action;
  }

  public bool HasRules => _rules.Count > 0;

  readonly List<Rule> _rules = new();

  [Inject]
  public void InjectDependencies(BaseInstantiator baseInstantiator, AutomationService automationService) {
    _baseInstantiator = baseInstantiator;
    _automationService = automationService;
  }

  public bool AddRule(AutomationConditionBase condition, AutomationActionBase action) {
    if (HasRule(condition, action)) {
      HostedDebugLog.Warning(TransformFast, "Skipping duplicate rule: condition={0}, action={1}", condition, action);
      return false;
    }
    condition.SetupComponents(_baseInstantiator);
    action.SetupComponents(_baseInstantiator);
    _rules.Add(new Rule { condition = condition, action = action});
    HostedDebugLog.Fine(TransformFast, "Adding rule: {0}, {1}", condition, action);
    UpdateRegistration();
    return true;
  }

  public void ClearRules() {
    _rules.Clear();
    UpdateRegistration();
  }

  public bool HasRule(AutomationConditionBase condition, AutomationActionBase action) {
    return _rules.Any(r => r.condition.Equals(condition) && r.action.Equals(action));
  }

  public void TriggerAction(AutomationConditionBase condition) {
    DebugEx.Fine("Handle triggered condition: {0}", condition);
    foreach (var rule in _rules) {
      if (rule.condition.Equals(condition)) {
        var action = rule.action;
        DebugEx.Fine("Triggering action: {0}", action);
        action.Execute(rule.condition);
      }
      //FIXME: maybe deleting condition if it fires once
    }
  }

  public void Save(IEntitySaver entitySaver) {
    //throw new NotImplementedException();
  }

  public void Load(IEntityLoader entityLoader) {
    //throw new NotImplementedException();
    //DebugEx.Warning("*** loader state: {0}", GameLoader.LoadedSave);
  }

  public void InitializeEntity() {
    //DebugEx.Warning("*** initializer called");
  }

  #region Implementation
  void UpdateRegistration() {
    if (HasRules) {
      _automationService.RegisterBehavior(this);
    } else {
      _automationService.UnregisterBehavior(this);
    }
  }
  #endregion
}

}
