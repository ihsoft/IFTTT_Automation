using System.Linq;
using Bindito.Core;
using IFTTT_Automation.Utils;
using TimberApi.ToolSystem;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.BuildingsBlocking;
using Timberborn.EntityPanelSystem;
using Timberborn.EntitySystem;
using Timberborn.PrefabSystem;
using Timberborn.ToolSystem;
using UnityDev.LogUtils;
using UnityEngine;

namespace IFTTT_Automation.Tools {

// ReSharper disable once ClassNeverInstantiated.Global
sealed class PauseTool : AbstractAreaSelectionTool {
  #region Local fields and properties
  const string StartSelectingPromptLoc = "IgorZ.Automation.Common.LockSelectTool.StartSelectingPrompt";
  const string StartObjectSelectingPromptLoc = "IgorZ.Automation.Common.LockSelectTool.StartObjectSelectingPrompt";
  const string SelectingOneObjectLoc = "IgorZ.Automation.Common.LockSelectTool.SelectingObject";
  const string SelectingNObjectsLoc = "IgorZ.Automation.Common.LockSelectTool.SelectingNObjects";
  const string ToolHintLoc = "IgorZ.Automation.PauseTool.Hint";

  string _selectingPrefabName;
  string _selectingEntityNiceName;
  EntityBadgeService _entityBadgeService;
  #endregion
  
  #region CustomTool overrides
  /// <inheritdoc/>
  public override void InitializeTool(ToolGroup toolGroup, ToolSpecification toolSpecification) {
    HighlightColor = ActionColor = Color.red;
    TileColor = SideColor = Color.white;
    base.InitializeTool(toolGroup, toolSpecification);

    DescriptionHintSectionLoc = ToolHintLoc;
  }

  /// <inheritdoc/>
  public override string WarningText() {
    if (_selectingPrefabName != null) {
      return SelectedObjects.Count == 1
          ? Loc.T(SelectingOneObjectLoc, _selectingEntityNiceName)
          : Loc.T(SelectingNObjectsLoc, _selectingEntityNiceName, SelectedObjects.Count(ObjectFilterExpression));
    }
    if (!InputService.IsShiftHeld || CursorOnUI || SelectionModeActive) {
      return "";
    }
    if (GetCompatibleComponent(HighlightedBlockObject) != null) {
      return Loc.T(StartObjectSelectingPromptLoc, GetEntityNiceName(HighlightedBlockObject));
    }
    return Loc.T(StartSelectingPromptLoc);
  }
  #endregion

  #region AbstractAreaSelectionTool overries
  /// <inheritdoc/>
  protected override bool ObjectFilterExpression(BlockObject blockObject) {
    var component = GetCompatibleComponent(blockObject);
    return component != null
        && !component.Paused
        && (_selectingPrefabName == null || blockObject.GetComponentFast<Prefab>().IsNamed(_selectingPrefabName));
  }

  /// <inheritdoc/>
  protected override void OnObjectAction(BlockObject blockObject) {
    blockObject.GetComponentFast<PausableBuilding>().Pause();
  }

  /// <inheritdoc/>
  protected override void OnSelectionModeChange(bool newMode) {
    base.OnSelectionModeChange(newMode);
    if (newMode) {
      if (InputService.IsShiftHeld && GetCompatibleComponent(HighlightedBlockObject)) {
        _selectingPrefabName = HighlightedBlockObject.GetComponentFast<Prefab>().Name;
        _selectingEntityNiceName = GetEntityNiceName(HighlightedBlockObject);
      }
    } else {
      _selectingPrefabName = null;
      _selectingEntityNiceName = null;
    }
  }
  #endregion

  #region Local methods
  [Inject]
  public void InjectDependencies(EntityBadgeService entityBadgeService) {
    _entityBadgeService = entityBadgeService;
  }

  PausableBuilding GetCompatibleComponent(BlockObject obj) {
    if (obj == null) {
      return null;
    }
    var component = obj.GetComponentFast<PausableBuilding>();
    if (component != null && component.enabled && component.IsPausable()) {
      return component;
    }
    return null;
  }

  /// <summary>Returns a user friendly localized name of the entity.</summary>
  string GetEntityNiceName(BaseComponent obj) {
    string niceName;
    if (obj.TryGetComponentFast<EntityComponent>(out var component)) {
      niceName = _entityBadgeService.GetEntityName(component);
    } else {
      DebugEx.Error("Cannot get entity for: {0}", obj);
      niceName = obj.GetComponentFast<Prefab>().Name.Split(new[] { '.' }, 2)[0];
    }
    return niceName;
  }
  #endregion
}

}
