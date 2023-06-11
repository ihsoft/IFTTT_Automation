// Timberborn Utils
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

using System;
using System.Diagnostics.CodeAnalysis;
using Automation.Core;
using Automation.Utils;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Localization;
using Timberborn.Persistence;
using UnityDev.LogUtils;

namespace Automation.Conditions {

/// <summary>The base class of any automation condition.</summary>
/// <remarks>
/// The descendants of this class must encapsulate all settings of the condition and provide functionality to set up the
/// dynamic logic.
/// </remarks>
/// <seealso cref="AutomationConditionBehaviorBase"/>
[SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public abstract class AutomationConditionBase : IEquatable<AutomationConditionBase>, IGameSerializable {
  /// <summary>Serializer that handles persistence of all the condition types.</summary>
  public static readonly DynamicClassSerializer<AutomationConditionBase> ConditionSerializer = new();

  /// <summary>The rule which owns this condition.</summary>
  /// <remarks>It can be <c>null</c> for an unowned condition.</remarks>
  public AutomationRule Rule = null;

  /// <summary>Default constructor is required for serialization.</summary>
  protected AutomationConditionBase() {}

  /// <summary>Copy constructor is required for cloning.</summary>
  /// <seealso cref="Clone"/>
  protected AutomationConditionBase(AutomationConditionBase src) {}

  /// <summary>Returns a copy of the condition that is not bound to any game object.</summary>
  public abstract AutomationConditionBase Clone();

  /// <summary>Returns a localized string to present as description of the condition.</summary>
  /// <remarks>The string must fully describe what the condition checks, but be as short as possible.</remarks>
  public abstract string GetUiDescription(ILoc loc);

  /// <summary>Notifies that the owning rule association to an automation behavior is going to be changed.</summary>
  /// <remarks>
  /// When the rule gets associated with <see cref="AutomationBehavior"/>, its becomes <i>active</i>. It means, it now
  /// should be checking the condition and triggering the action. The rule can also be detached from the automation
  /// behavior (in which case <see cref="AutomationRule.Behavior"/> becomes <c>null</c>). The condition logic that needs
  /// active <see cref="BaseComponent"/> objects to work, must react appropriately and either setup the needed
  /// components or destroy some of the existing.
  /// </remarks>
  /// <param name="newBehavior">The new automation behavior or <c>null</c>.</param>
  /// <seealso cref="Rule"/>
  /// <seealso cref="AutomationRule.Behavior"/>
  public virtual void OnBeforeRuleAssociationChange(AutomationBehavior newBehavior) {}

  /// <summary>Verifies that the condition can be used at the provided automation behavior.</summary>
  public virtual bool IsValidAt(AutomationBehavior behavior) {
    var blockObject = behavior.GetComponentFast<BlockObject>();
    return blockObject != null && blockObject.Finished;
  }

  /// <summary>Loads condition state and declaration.</summary>
  public virtual void LoadFrom(IObjectLoader objectLoader) {}

  /// <summary>Saves condition state and declaration.</summary>
  public virtual void SaveTo(IObjectSaver objectSaver) {}

  public virtual void Trigger() {
    //FXIME
    DebugEx.Warning("*** rule is: {0}", Rule);
    DebugEx.Warning("*** behavior is: {0}", Rule?.Behavior);
    Rule.Behavior.TriggerAction(this);
  }

  /// <inheritdoc/>
  public virtual bool Equals(AutomationConditionBase other) {
    return other != null && other.GetType() == GetType() && Rule == other.Rule;
  }

  /// <inheritdoc/>
  public override string ToString() {
    return $"TypeId={GetType()}";
  }
}

/// <summary>The base class for an action that has behavior.</summary>
/// <remarks>It encapsulates te basic logic on dealing with the behavior components.</remarks>
/// <typeparam name="T">type of the behavior component</typeparam>
public abstract class AutomationConditionBase<T> : AutomationConditionBase where T : AutomationConditionBehaviorBase {
  /// <inheritdoc/>
  protected AutomationConditionBase() {}

  /// <inheritdoc/>
  protected AutomationConditionBase(AutomationConditionBase src) : base(src) {}

  /// <inheritdoc/>
  public override void OnBeforeRuleAssociationChange(AutomationBehavior newBehavior) {
    if (newBehavior != null) {
      var behavior = newBehavior.GetComponentFast<T>()
          ?? AutomationService.Instance.BaseInstantiator.AddComponent<T>(newBehavior.GameObjectFast);
      behavior.AddCondition(this);
    } else {
      Rule.Behavior.GetComponentFast<T>().DeleteCondition(this);
    }
  }
}

}
