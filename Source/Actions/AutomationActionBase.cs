// Timberborn Utils
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

using System.Diagnostics.CodeAnalysis;
using Automation.Core;
using Automation.Utils;
using Timberborn.Persistence;

namespace Automation.Actions {

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public abstract class AutomationActionBase : IAutomationAction, IAutomationConditionListener {
  /// <summary>Serializer that handles persistence of all the action types.</summary>
  public static readonly DynamicClassSerializer<AutomationActionBase> ActionSerializer = new();

  #region ICondition implementation
  /// <inheritdoc/>
  public virtual AutomationBehavior Behavior {
    get => _behavior;
    set {
      if (value == _behavior) {
        return;
      }
      if (value == null || _behavior != null) {
        OnBehaviorToBeCleared();
      }
      _behavior = value;
      if (_behavior != null) {
        OnBehaviorAssigned();
      }
    }
  }
  AutomationBehavior _behavior;

  /// <inheritdoc/>
  public virtual IAutomationCondition Condition { get; set; }

  /// <inheritdoc/>
  public bool IsMarkedForCleanup { get; protected set; }
  #endregion

  #region IGameSerializable implemenation
  /// <inheritdoc/>
  public virtual void LoadFrom(IObjectLoader objectLoader) {}

  /// <inheritdoc/>
  public virtual void SaveTo(IObjectSaver objectSaver) {}
  #endregion

  #region API
  /// <inheritdoc/>
  public abstract string UiDescription { get; }

  /// <inheritdoc/>
  public abstract IAutomationAction CloneDefinition();

  /// <inheritdoc/>
  public virtual bool CheckSameDefinition(IAutomationAction other) {
    return other != null && other.GetType() == GetType();
  }

  /// <inheritdoc/>
  public virtual bool IsValidAt(AutomationBehavior behavior) {
    return behavior.BlockObject.Finished;
  }

  /// <summary>
  /// Notifies that a new behavior has been assigned to the condition. It's the time to setup the behaviors. 
  /// </summary>
  /// <seealso cref="Behavior"/>
  protected virtual void OnBehaviorAssigned() {}

  /// <summary>
  /// Notifies that the current behavior is about to be cleared. It's the time to cleanup the behaviors. 
  /// </summary>
  /// <seealso cref="Behavior"/>
  protected virtual void OnBehaviorToBeCleared() {}
  #endregion

  #region IAutomationConditionListener
  /// <inheritdoc/>
  public abstract void OnConditionState(IAutomationCondition automationCondition);
  #endregion

  #region Implementation
  /// <summary>Default constructor is required for serialization.</summary>
  protected AutomationActionBase() {}

  /// <summary>Copy constructor is required for cloning.</summary>
  /// <seealso cref="CloneDefinition"/>
  protected AutomationActionBase(AutomationActionBase src) {}

  /// <inheritdoc/>
  public override string ToString() {
    return $"TypeId={GetType()},Condition={Condition?.GetType()}";
  }
  #endregion
}

}
