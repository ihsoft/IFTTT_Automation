// Timberborn Utils
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

using System;
using Automation.Conditions;
using Automation.Core;
using Automation.Utils;
using Timberborn.Localization;
using Timberborn.Persistence;

namespace Automation.Actions {

public abstract class AutomationActionBase : IEquatable<AutomationActionBase>, IGameSerializable {
  /// <summary>Serializer that handles persistence of all the action types.</summary>
  public static readonly DynamicClassSerializer<AutomationActionBase> ActionSerializer = new();

  /// <summary>The rule which owns this condition.</summary>
  /// <remarks>Condition is not considered "active" until it's attached to a rule.</remarks>
  public AutomationRule Rule { get; internal set; }

  /// <inheritdoc cref="AutomationConditionBase.OnBeforeRuleAssociationChange"/>
  public virtual void OnBeforeRuleAssociationChange(AutomationBehavior newBehavior) {}

  /// <summary>Loads action state and declaration.</summary>
  public virtual void LoadFrom(IObjectLoader objectLoader) {
  }

  /// <summary>Saves action state and declaration.</summary>
  public virtual void SaveTo(IObjectSaver objectSaver) {
  }

  /// <summary>Default constructor is required for serialization.</summary>
  protected AutomationActionBase() {
  }

  /// <summary>Copy constructor is required for cloning.</summary>
  /// <seealso cref="Clone"/>
  protected AutomationActionBase(AutomationActionBase src) {
  }

  /// <summary>Returns a copy of the action that is not bound to any game object.</summary>
  public abstract AutomationActionBase Clone();

  /// <summary>Returns a localized string to present as description of the condition.</summary>
  /// <remarks>The string must fully describe what the condition checks, but be as short as possible.</remarks>
  public abstract string GetUiDescription(ILoc loc);

  /// <summary>Verifies that the action can work on the provided automation behavior.</summary>
  public abstract bool IsValidAt(AutomationBehavior behavior);

  // FIXME: in overrides
  // Listener.InvalidateAction(this);
  public abstract void Execute(AutomationConditionBase triggerCondition);

  public virtual bool Equals(AutomationActionBase other) {
    return other != null && other.GetType() == GetType() && Rule == other.Rule;
  }

  public override string ToString() {
    return $"TypeId={GetType()}";
  }
}

}
