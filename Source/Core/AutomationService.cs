// Timberborn Mod: Automation
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Automation.Conditions;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.SelectionSystem;
using Timberborn.SingletonSystem;
using Timberborn.ToolSystem;
using UnityEngine;

namespace Automation.Core {

/// <summary>Central point for all the automation related logic.</summary>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public class AutomationService : IPostLoadableSingleton {
  #region Internal fields
  readonly HashSet<AutomationBehavior> _registeredBehaviors = new();
  readonly Color _highlightColor = Color.cyan * 0.5f;
  readonly Highlighter _highlighter;
  bool _highlightingEnabled;
  BaseComponent _rootComponent;
  #endregion

  public AutomationService(EventBus eventBus, Highlighter highlighter, BaseInstantiator baseInstantiator) {
    eventBus.Register(this);
    _highlighter = highlighter;
    BaseInstantiator = baseInstantiator;
  }

  #region Implemetation of IPostLoadableSingleton
  /// <inheritdoc/>
  public void PostLoad() {}
  #endregion

  #region API
  /// <summary>Shortcut to get the instantiator fro the conditions and actions.</summary>
  public readonly BaseInstantiator BaseInstantiator;

  /// <summary>All automation behaviors that has at least one active condition.</summary>
  public ReadOnlyHashSet<AutomationBehavior> AutomationBehaviors => _registeredBehaviors.AsReadOnly();

  /// <summary>Highlights all registered behaviours on the map.</summary>
  public void HighlightAutomationObjects(Color? useColor = null) {
    _highlightingEnabled = true;
    foreach (var behavior in _registeredBehaviors) {
      _highlighter.HighlightSecondary(behavior, useColor ?? _highlightColor);
    }
  }

  /// <summary>Resets highlightings.</summary>
  public void UnhighlightAutomationObjects() {
    _highlightingEnabled = false;
    _highlighter.UnhighlightAllSecondary();
  }

  /// <summary>Obtains a global automation behavior object.</summary>
  /// <remarks>
  /// Such object exists as singletons in the game scene. It's an object that monitors the overall game state and
  /// doesn't depend on a specific game object.
  /// </remarks>
  /// <typeparam name="T">the behavior type to return or create</typeparam>
  /// <returns>Existing or new instance of the behavior of the requested type.</returns>
  public T GetGlobalBehavior<T>() where T : AutomationConditionBehaviorBase {
    if (_rootComponent == null) {
      var prefabObj = new GameObject("#rootPrefab-" + GetType().ToString());
      var rootObj = BaseInstantiator.InstantiateInactive(prefabObj, null);
      Object.Destroy(prefabObj);
      _rootComponent = BaseInstantiator.AddComponent<RootComponent>(rootObj);
    }
    return _rootComponent.GetComponentFast<T>() ?? BaseInstantiator.AddComponent<T>(_rootComponent.GameObjectFast);
  }
  internal class RootComponent : BaseComponent {}
  #endregion

  #region Implementation
  internal void RegisterBehavior(AutomationBehavior behavior) {
    _registeredBehaviors.Add(behavior);
    if (_highlightingEnabled) {
      _highlighter.HighlightSecondary(behavior, _highlightColor);
    }
  }

  internal void UnregisterBehavior(AutomationBehavior behavior) {
    if (_highlightingEnabled) {
      _highlighter.UnhighlightSecondary(behavior);
    }
    _registeredBehaviors.Remove(behavior);
  }

  [OnEvent]
  public void OnToolEntered(ToolEnteredEvent toolEnteredEvent) {
    if (toolEnteredEvent.Tool is not IAutomationModeEnabler) {
      return;
    }
    HighlightAutomationObjects();
  }

  [OnEvent]
  public void OnToolExited(ToolExitedEvent toolExitedEvent) {
    UnhighlightAutomationObjects();
  }
  #endregion
}

}
