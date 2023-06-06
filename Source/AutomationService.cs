// Timberborn Mod: Automation
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

using System.Collections.Generic;
using Bindito.Core;
using Timberborn.SelectionSystem;
using Timberborn.SingletonSystem;
using Timberborn.ToolSystem;
using UnityEngine;

namespace Automation {

public class AutomationService : IPostLoadableSingleton {
  #region Internal fields
  readonly HashSet<AutomationBehavior> _registeredBehaviors = new();
  readonly Color _highlightColor = Color.cyan * 0.5f;
  Highlighter _highlighter;
  bool _highlightingEnabled;
  #endregion

  #region API
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
  #endregion

  #region Implementation
  [Inject]
  public void InjectDependencies(EventBus eventBus, Highlighter highlighter) {
    eventBus.Register(this);
    _highlighter = highlighter;
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
