// Timberborn Utils
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

using System;
using System.Linq;
using Automation.Conditions;
using Timberborn.BlockSystem;
using Timberborn.Localization;
using Timberborn.Persistence;
using Timberborn.PrefabSystem;
using UnityDev.LogUtils;

namespace Automation.Actions {

public abstract class AutomationActionBase : IEquatable<AutomationActionBase> {
  static readonly PropertyKey<string> TypeIdPropertyKey = new("TypeId");

  #region Class serializer
  class Serializer : IObjectSerializer<AutomationActionBase> {
    public void Serialize(AutomationActionBase value, IObjectSaver objectSaver) {
      value.SaveTo(objectSaver);
    }

    public Obsoletable<AutomationActionBase> Deserialize(IObjectLoader objectLoader) {
      var typeId = objectLoader.Get(TypeIdPropertyKey);
      var conditionType = AppDomain.CurrentDomain.GetAssemblies()
          .Select(assembly => assembly.GetType(typeId))
          .FirstOrDefault(t => t != null);
      if (conditionType == null) {
        DebugEx.Error("Cannot find type for action: {0}", typeId);
        throw new InvalidOperationException("Cannot find condition type");
      }
      var instance = (AutomationActionBase) Activator.CreateInstance(conditionType);
      instance.LoadFrom(objectLoader);
      return instance;
    }
  }
  #endregion

  public static readonly IObjectSerializer<AutomationActionBase> ActionSerializer = new Serializer();

  /// <summary>Loads action state and declaration.</summary>
  protected internal virtual void LoadFrom(IObjectLoader objectLoader) {
    var savedId = objectLoader.GetValueOrNull(TypeIdPropertyKey);
    if (savedId != ActionTypeId) {
      DebugEx.Warning("Cannot load type '{0}' from saved state of '{1}'", GetType(), savedId);
      throw new InvalidOperationException("Cannot load condition state");
    }
  }

  /// <summary>Saves action state and declaration.</summary>
  protected internal virtual void SaveTo(IObjectSaver objectSaver) {
    objectSaver.Set(TypeIdPropertyKey, ActionTypeId);
  }

  public readonly string ActionTypeId;
  public AutomationBehavior Target;

  /// <summary>Default constructor is required for serialization.</summary>
  protected AutomationActionBase() {
    ActionTypeId = GetType().FullName;
  }

  /// <summary>Copy constructor is required for cloning.</summary>
  /// <seealso cref="Clone"/>
  protected AutomationActionBase(AutomationActionBase src) {
    ActionTypeId = src.ActionTypeId;
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
    return other != null && ActionTypeId == other.ActionTypeId && Target == other.Target;
  }

  public override string ToString() {
    if (Target == null) {
      return $"[Action:type={ActionTypeId};target=NULL]";
    }
    var prefabName = Target.GetComponentFast<Prefab>()?.Name ?? "UNKNOWN";
    var coords = Target.GetComponentFast<BlockObject>().Coordinates;
    return $"[Action:type={ActionTypeId};target={prefabName}@{coords}]";
  }
}

}
