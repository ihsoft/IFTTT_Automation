// Timberborn Utils
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

using System.Linq;
using Bindito.Core;
using Timberborn.SingletonSystem;
using Timberborn.WeatherSystem;

namespace Automation.Conditions {

public class DroughtTrackerBehavior : AutomationConditionBehaviorBase {
  [Inject]
  public void InjectDependencies(EventBus eventBus) {
    eventBus.Register(this);
  }

  [OnEvent]
  public void OnDroughtStartedEvent(DroughtStartedEvent @event) {
    foreach (var condition in Conditions.OfType<DroughtStartedCondition>()) {
      condition.Trigger();
    }
  }

  [OnEvent]
  public void OnDroughtEndedEvent(DroughtEndedEvent @event) {
    foreach (var condition in Conditions.OfType<DroughtEndedCondition>()) {
      condition.Trigger();
    }
  }
}

}
