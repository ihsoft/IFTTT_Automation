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

namespace Automation.Core {

public class AutomationBehavior : BaseComponent, IPersistentEntity, IInitializableEntity {
  BaseInstantiator _baseInstantiator;
  AutomationService _automationService;

  public bool HasRules => _rules.Count > 0;

  public IReadOnlyCollection<AutomationRule> Rules => _rules.AsReadOnly();

  readonly List<AutomationRule> _rules = new();

  [Inject]
  public void InjectDependencies(BaseInstantiator baseInstantiator, AutomationService automationService) {
    _baseInstantiator = baseInstantiator;
    _automationService = automationService;
  }

  public bool AddRule(AutomationRule rule) {
    if (HasRule(rule.Condition, rule.Action)) {
      HostedDebugLog.Warning(TransformFast, "Skipping duplicate rule: {0}", rule);
      return false;
    }
    rule.AttachToBehavior(this);
    _rules.Add(rule);
    HostedDebugLog.Fine(TransformFast, "Adding rule: {0}", rule);
    UpdateRegistration();
    return true;
  }

  public void ClearRules() {
    foreach (var rule in _rules) {
      rule.AttachToBehavior(null);
    }
    _rules.Clear();
    UpdateRegistration();
  }

  public bool HasRule(AutomationConditionBase condition, AutomationActionBase action) {
    return _rules.Any(r => r.Condition.Equals(condition) && r.Action.Equals(action));
  }

  public void TriggerAction(AutomationConditionBase condition) {
    DebugEx.Fine("Handle triggered condition: {0}", condition);
    foreach (var rule in _rules) {
      if (rule.Condition.Equals(condition)) {
        var action = rule.Action;
        DebugEx.Fine("Triggering action: {0}", action);
        action.Execute(rule.Condition);
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
