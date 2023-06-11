// Timberborn Utils
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

using System;
using Automation.Conditions;
using Automation.Core;
using Automation.Utils;
using Timberborn.BlockSystem;
using Timberborn.Localization;
using Timberborn.Persistence;
using Timberborn.PrefabSystem;

namespace Automation.Actions {

public abstract class AutomationActionBase : IEquatable<AutomationActionBase>, IGameSerializable {
  public static readonly DynamicClassSerializer<AutomationActionBase> ActionSerializer = new();

  /// <summary>Loads action state and declaration.</summary>
  public virtual void LoadFrom(IObjectLoader objectLoader) {
  }

  /// <summary>Saves action state and declaration.</summary>
  public virtual void SaveTo(IObjectSaver objectSaver) {
  }

  public AutomationBehavior Target;

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

  /// <summary>Verifies that the action can be used in its current setup.</summary>
  public abstract bool IsValid();

  // FIXME: in overrides
  // Listener.InvalidateAction(this);
  public abstract void Execute(AutomationConditionBase triggerCondition);

  public virtual bool Equals(AutomationActionBase other) {
    return other != null && other.GetType() == GetType() && Target == other.Target;
  }

  public override string ToString() {
    if (Target == null) {
      return $"[Action:type={GetType()};target=NULL]";
    }
    var prefabName = Target.GetComponentFast<Prefab>()?.Name ?? "UNKNOWN";
    var coords = Target.GetComponentFast<BlockObject>().Coordinates;
    return $"[Action:type={GetType()};target={prefabName}@{coords}]";
  }
}

}
