// Timberborn Utils
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Automation.Conditions;
using Automation.Core;
using Automation.UnityDevCandidates;
using TimberApi.DependencyContainerSystem;
using Timberborn.BlockObjectTools;
using Timberborn.BlockSystem;
using Timberborn.Coordinates;
using Timberborn.Explosions;
using Timberborn.Localization;
using Timberborn.Persistence;
using Timberborn.ToolSystem;
using UnityDev.LogUtils;
using UnityEngine;

namespace Automation.Actions {

/// <summary>Action that triggers the dynamite and then (optionally) places new one at the same spot.</summary>
/// <remarks>Use it to drill down deep holes in terrain.</remarks>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public sealed class DetonateDynamiteAction : AutomationActionBase {
  static readonly PropertyKey<int> RepeatPropertyKey = new("Repeat");
  const int DetonateDelayTicks = 3; // 1 tick = 300ms

  /// <summary>
  /// Number of times to place a new dynamite. Any value less or equal to zero results in no extra actions on trigger.
  /// </summary>
  /// <remarks>
  /// A too big value is not a problem. When the bottom of the map is reached, the dynamite simply won't get placed.
  /// </remarks>
  public int RepeatCount;

  #region AutomationActionBase overrides
  /// <inheritdoc/>
  public DetonateDynamiteAction() {
  }

  /// <inheritdoc/>
  public DetonateDynamiteAction(DetonateDynamiteAction src) : base(src) {
    RepeatCount = src.RepeatCount;
  }

  /// <inheritdoc/>
  public override AutomationActionBase Clone() {
    return new DetonateDynamiteAction(this);
  }

  /// <inheritdoc/>
  public override bool IsValid() {
    return Target.GetComponentFast<Dynamite>() != null;
  }

  /// <summary>Loads action state and declaration.</summary>
  public override void LoadFrom(IObjectLoader objectLoader) {
    base.LoadFrom(objectLoader);
    RepeatCount = objectLoader.Get(RepeatPropertyKey);
  }

  /// <summary>Saves action state and declaration.</summary>
  public override void SaveTo(IObjectSaver objectSaver) {
    objectSaver.Set(RepeatPropertyKey, RepeatCount);
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
      if (coordinates.z <= 1) {
        DebugEx.Fine("Reached the bottom of the map. Abort placing dynamite.");
        yield break;
      }
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
          new ObjectFinishedCondition { Source = automationObj },
          new DetonateDynamiteAction { Target = automationObj, RepeatCount = repeatCount });
      Destroy(gameObject);
    }
  }
  #endregion
}

}
