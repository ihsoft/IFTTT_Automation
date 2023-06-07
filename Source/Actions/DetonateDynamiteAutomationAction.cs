// Timberborn Utils
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Automation.Conditions;
using Automation.UnityDevCandidates;
using TimberApi.DependencyContainerSystem;
using Timberborn.BlockObjectTools;
using Timberborn.BlockSystem;
using Timberborn.Coordinates;
using Timberborn.Explosions;
using Timberborn.Localization;
using Timberborn.ToolSystem;
using UnityDev.LogUtils;
using UnityEngine;

namespace Automation.Actions {

public sealed class DetonateDynamiteAutomationAction : AutomationActionBase {
  const int DetonateDelayTicks = 3; // 1 tick = 300ms

  public readonly int RepeatCount;

  public DetonateDynamiteAutomationAction(AutomationBehavior target, int repeatCount) : base(
      nameof(DetonateDynamiteAutomationAction), target) {
    RepeatCount = repeatCount;
  }

  #region AutomationActionBase overrides
  /// <inheritdoc/>
  public override bool IsValid() {
    return Target != null && Target.GetComponentFast<Dynamite>() != null;
  }

  /// <inheritdoc/>
  public override string GetUiDescription(ILoc loc) {
    var res = "<SolidHighlight>detonate dynamite</SolidHighlight>";
    if (RepeatCount > 0) {
      res += string.Format(" and add another <GreenHighlight>{0} times</GreenHighlight>", RepeatCount);
    }
    return res;
  }

  /// <inheritdoc/>
  public override void Execute(AutomationConditionBase triggerCondition) {
    var blockObject = Target.GetComponentFast<BlockObject>();
    var dynamite = Target.GetComponentFast<Dynamite>();
    DebugEx.Fine("Detonate dynamite: coordinates={0}, tries={1}", blockObject.Coordinates, RepeatCount);
    dynamite.TriggerDelayed(DetonateDelayTicks);
    if (RepeatCount <= 0) {
      return;
    }
    var component = new GameObject("#Automation_PlaceDynamiteAction").AddComponent<RepeatRule>();
    component.blockObject = blockObject;
    component.repeatCount = RepeatCount - 1;
  }
  #endregion

  #region MonoBehavior object to handle action repeat
  class RepeatRule : MonoBehaviour {
    static BlockObjectTool DynamiteTool =>
        _dynamiteTool ??= DependencyContainer.GetInstance<ToolButtonService>()
            .ToolButtons.Select(x => x.Tool)
            .OfType<BlockObjectTool>()
            .FirstOrDefault(x => x.Prefab.name.StartsWith("Dynamite"));
    static BlockObjectTool _dynamiteTool;

    static readonly ReflectedAction<BlockObjectTool, IEnumerable<OrientedCoordinates>> BlockObjectToolPlace =
        new("Place");

    public BlockObject blockObject;
    public int repeatCount;

    void Awake() {
      if (DynamiteTool == null || !BlockObjectToolPlace.IsValid()) {
        DebugEx.Error("Cannot execute dynamite place tool");
        Destroy(gameObject);
        return;
      }
      StartCoroutine(WaitAndPlace());
    }

    IEnumerator WaitAndPlace() {
      yield return null;

      var coordinates = blockObject.Coordinates;
      yield return new WaitUntil(() => blockObject == null);

      coordinates.z = coordinates.z - 1;
      BlockObjectToolPlace.Invoke(DynamiteTool, new List<OrientedCoordinates> { new(coordinates, Orientation.Cw0) });
      var blockService = DependencyContainer.GetInstance<BlockService>();
      BlockObject newDynamite;
      do {
        yield return null;
        newDynamite = blockService.GetBottomObjectAt(coordinates);
      } while (newDynamite == null);

      var automationObj = newDynamite.GetComponentFast<AutomationBehavior>();
      automationObj.AddRule(
          new ObjectFinishedCondition(automationObj),
          new DetonateDynamiteAutomationAction(automationObj, repeatCount));
      Destroy(gameObject);
    }
  }
  #endregion
}

}
