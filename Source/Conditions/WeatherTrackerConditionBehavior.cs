// Timberborn Utils
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

using Bindito.Core;
using Timberborn.SingletonSystem;
using Timberborn.WeatherSystem;

namespace Automation.Conditions {

public sealed class WeatherTrackerBehavior : AutomationConditionBehaviorBase {
  [Inject]
  public void InjectDependencies(EventBus eventBus) {
    eventBus.Register(this);
  }

  [OnEvent]
  public void OnDroughtStartedEvent(DroughtStartedEvent @event) {
    foreach (var condition in Conditions) {
      switch (condition) {
        case DroughtStartedCondition droughtStarted:
          droughtStarted.ConditionState = true;
          break;
        case DroughtEndedCondition droughtEnded:
          droughtEnded.ConditionState = false;
          break;
      }
    }
  }

  [OnEvent]
  public void OnDroughtEndedEvent(DroughtEndedEvent @event) {
    foreach (var condition in Conditions) {
      switch (condition) {
        case DroughtStartedCondition droughtStarted:
          droughtStarted.ConditionState = false;
          break;
        case DroughtEndedCondition droughtEnded:
          droughtEnded.ConditionState = true;
          break;
      }
    }
  }
}

}
