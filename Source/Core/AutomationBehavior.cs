// Timberborn Mod: Automation
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

using System.Collections.Generic;
using System.Linq;
using Automation.Actions;
using Automation.Conditions;
using Bindito.Core;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.EntitySystem;
using Timberborn.Localization;
using Timberborn.Persistence;
using UnityDev.LogUtils;

namespace Automation.Core {

public class AutomationBehavior : BaseComponent, IPersistentEntity, IInitializableEntity {
  public BaseInstantiator BaseInstantiator { get; private set; }
  public AutomationService AutomationService { get; private set; }
  public ILoc Loc { get; private set; }

  public BlockObject BlockObject => _blockObject ??= GetComponentFast<BlockObject>();
  BlockObject _blockObject;

  public bool HasActions => _actions.Count > 0;

  public IReadOnlyCollection<IAutomationAction> Actions => _actions.AsReadOnly();
  readonly List<IAutomationAction> _actions = new();

  void OnDestroy() {
    ClearActions();
  }

  [Inject]
  public void InjectDependencies(BaseInstantiator baseInstantiator, AutomationService automationService, ILoc loc) {
    BaseInstantiator = baseInstantiator;
    AutomationService = automationService;
    Loc = loc;
  }

  public bool AddRule(IAutomationCondition condition, IAutomationAction action) {
    if (HasRule(condition, action)) {
      HostedDebugLog.Warning(TransformFast, "Skipping duplicate rule: condition={0}, action={1}", condition, action);
      return false;
    }
    action.Condition = condition;
    _actions.Add(action);
    HostedDebugLog.Fine(TransformFast, "Adding rule: {0}", action);
    UpdateRegistration();
    return true;
  }

  public void ClearActions() {
    foreach (var action in _actions) {
      action.Condition.Behavior = null;
      action.Behavior = null;
    }
    _actions.Clear();
    UpdateRegistration();
  }

  public bool HasRule(IAutomationCondition condition, IAutomationAction action) {
    return _actions.Any(r => r.CheckSameDefinition(action) && r.Condition.CheckSameDefinition(condition));
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
    if (HasActions) {
      AutomationService.RegisterBehavior(this);
    } else {
      AutomationService.UnregisterBehavior(this);
    }
  }
  #endregion
}

}
