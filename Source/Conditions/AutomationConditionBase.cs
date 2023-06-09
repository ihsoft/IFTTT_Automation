// Timberborn Utils
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

using System;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Localization;
using Timberborn.Persistence;
using Timberborn.PrefabSystem;
using UnityDev.LogUtils;

namespace Automation.Conditions {

/// <summary>The base class of any automation condition.</summary>
/// <remarks>
/// The descendants of this class must encapsulate all settings of teh condition and provide functionality to set up the
/// dynamic logic.
/// </remarks>
/// <seealso cref="AutomationConditionBehaviorBase"/>
public abstract class AutomationConditionBase : IEquatable<AutomationConditionBase> {
  static readonly PropertyKey<string> ConditionTypeIdPropertyKey = new("TypeId");

  #region Class serializer
  class Serializer : IObjectSerializer<AutomationConditionBase> {
    public void Serialize(AutomationConditionBase value, IObjectSaver objectSaver) {
      value.SaveTo(objectSaver);
    }

    public Obsoletable<AutomationConditionBase> Deserialize(IObjectLoader objectLoader) {
      var typeId = objectLoader.Get(ConditionTypeIdPropertyKey);
      var conditionType = AppDomain.CurrentDomain.GetAssemblies()
          .Select(assembly => assembly.GetType(typeId))
          .FirstOrDefault(t => t != null);
      if (conditionType == null) {
        DebugEx.Error("Cannot find type for condition: {0}", typeId);
        throw new InvalidOperationException("Cannot find condition type");
      }
      var instance = (AutomationConditionBase) Activator.CreateInstance(conditionType);
      instance.LoadFrom(objectLoader);
      return instance;
    }
  }
  #endregion

  public static readonly IObjectSerializer<AutomationConditionBase> ConditionSerializer = new Serializer();

  public readonly string ConditionTypeId;
  public AutomationBehavior Source;

  /// <summary>Default constructor is required for serialization.</summary>
  protected AutomationConditionBase() {
    ConditionTypeId = GetType().FullName;
  }

  /// <summary>Copy constructor is required for cloning.</summary>
  /// <seealso cref="Clone"/>
  protected AutomationConditionBase(AutomationConditionBase src) {
    ConditionTypeId = src.ConditionTypeId;
  }

  /// <summary>Returns a copy of the condition that is not bound to any game object.</summary>
  public abstract AutomationConditionBase Clone();

  /// <summary>Returns a localized string to present as description of the condition.</summary>
  /// <remarks>The string must fully describe what the condition checks, but be as short as possible.</remarks>
  public abstract string GetUiDescription(ILoc loc);

  /// <summary>Sets up behavior and all other <see cref="BaseComponent"/> components.</summary>
  /// <remarks>This method is called when the condition is being associated with the automation component.</remarks>
  public abstract void SetupComponents(BaseInstantiator baseInstantiator);

  /// <summary>Removes all components that were setup in <see cref="SetupComponents"/>.</summary>
  /// <remarks>This method is called when the condition is being removed from the automation component.</remarks>
  public abstract void ClearComponents();

  /// <summary>Verifies that the condition can be used in its current setup.</summary>
  public virtual bool IsValid() {
    return Source.GetComponentFast<BlockObject>().Finished;
  }

  /// <summary>Loads condition state and declaration.</summary>
  protected internal virtual void LoadFrom(IObjectLoader objectLoader) {
    var savedId = objectLoader.Get(ConditionTypeIdPropertyKey);
    if (savedId != ConditionTypeId) {
      DebugEx.Warning("Cannot load type '{0}' from saved state of '{1}'", GetType(), savedId);
      throw new InvalidOperationException("Cannot load condition state");
    }
  }

  /// <summary>Saves condition state and declaration.</summary>
  protected internal virtual void SaveTo(IObjectSaver objectSaver) {
    objectSaver.Set(ConditionTypeIdPropertyKey, ConditionTypeId);
  }

  //FIXME: reconsider in favor of set/reset condition state.
  public virtual void Trigger() {
    Source.TriggerAction(this);
    // FIXME: in overrides
    // Listener.InvalidateCondition(this);
  }

  /// <inheritdoc/>
  public virtual bool Equals(AutomationConditionBase other) {
    return other != null
        && ConditionTypeId == other.ConditionTypeId
        && Source == other.Source;
  }

  /// <inheritdoc/>
  public override string ToString() {
    if (Source == null) {
      return $"[Condition:type={ConditionTypeId};source=NULL]";
    }
    var prefabName = Source.GetComponentFast<Prefab>()?.Name ?? "UNKNOWN";
    var coords = Source.GetComponentFast<BlockObject>().Coordinates;
    return $"[Condition:type={ConditionTypeId};source={prefabName}@{coords}]";
  }
}

/// <summary>The base class for an action that has behavior.</summary>
/// <remarks>It encapsulates te basic logic on dealing with the behavior components.</remarks>
/// <typeparam name="T">type of the behavior component</typeparam>
public abstract class AutomationConditionBase<T> : AutomationConditionBase where T : AutomationConditionBehaviorBase {
  /// <inheritdoc/>
  protected AutomationConditionBase() {
  }

  /// <inheritdoc/>
  protected AutomationConditionBase(AutomationConditionBase src) : base(src) {
  }

  /// <inheritdoc/>
  public override void SetupComponents(BaseInstantiator baseInstantiator) {
    var behavior = Source.GetComponentFast<T>() ?? baseInstantiator.AddComponent<T>(Source.GameObjectFast);
    behavior.AddCondition(this);
  }

  /// <inheritdoc/>
  public override void ClearComponents() {
    Source.GetComponentFast<T>().DeleteCondition(this);
  }
}

}
