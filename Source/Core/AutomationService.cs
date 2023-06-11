// Timberborn Mod: Automation
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.SelectionSystem;
using Timberborn.SingletonSystem;
using Timberborn.ToolSystem;
using UnityEngine;

namespace Automation.Core {

/// <summary>Central point for all the automation related logic.</summary>
/// <remarks>
/// This components supports a regular singleton pattern. Thus, once in the game, the client code can get the automation
/// system object via a static field <see cref="Instance"/>.
/// </remarks>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public class AutomationService : IPostLoadableSingleton {
  #region Internal fields
  readonly HashSet<AutomationBehavior> _registeredBehaviors = new();
  readonly Color _highlightColor = Color.cyan * 0.5f;
  readonly Highlighter _highlighter;
  bool _highlightingEnabled;
  #endregion

  public AutomationService(EventBus eventBus, Highlighter highlighter, BaseInstantiator baseInstantiator) {
    eventBus.Register(this);
    _highlighter = highlighter;
    BaseInstantiator = baseInstantiator;
    Instance = this;
  }

  #region API
  /// <summary>Shortcut to get to the service without injections.</summary>
  public static AutomationService Instance { get; private set; }

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

  public void PostLoad() {
  }
  #endregion
}

}
