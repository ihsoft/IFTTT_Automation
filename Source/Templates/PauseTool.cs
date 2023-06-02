using IFTTT_Automation.Utils;
using TimberApi.ToolSystem;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.BuildingsBlocking;
using Timberborn.PrefabSystem;
using Timberborn.ToolSystem;
using UnityEngine;

namespace IFTTT_Automation.Templates {

// ReSharper disable once ClassNeverInstantiated.Global
sealed class PauseTool : AbstractAreaSelectionTool {
  #region Loacl fielda and properties
  string _selectingPrefabName;
  #endregion
  
  #region Tool overrides
  /// <inheritdoc/>
  public override ToolDescription Description() {
    return new ToolDescription.Builder(Loc.T(ToolSpecification.NameLocKey))
        .AddSection(Loc.T(ToolSpecification.DescriptionLocKey))
        .AddPrioritizedSection(Loc.T("IgorZ.Automation.PauseTool.Hint"))
        //.AddPrioritizedSection("Hold SHIFT to select only a specific construction type")
        //.AddExternalSection()
        .Build();
  }
  #endregion

  #region CustomTool overrides
  /// <inheritdoc/>
  public override void InitializeTool(ToolGroup toolGroup, ToolSpecification toolSpecification) {
    HighlightColor = ActionColor = Color.red;
    TileColor = SideColor = Color.white;
    base.InitializeTool(toolGroup, toolSpecification);
  }

  /// <inheritdoc/>
  public override string WarningText() {
    if (_selectingPrefabName != null) {
      return "Selecting: " + GetEntityNiceName(_selectingPrefabName);
    }
    if (!InputService.IsShiftHeld || CursorOnUI || SelectionModeActive) {
      return "";
    }
    if (GetCompatibleComponent(HighlightedBlockObject) != null) {
      return "Click to start selecting: " + GetEntityNiceName(HighlightedBlockObject);
    }
    return "Point to an entity and click to start selecting";
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
      }
    } else {
      _selectingPrefabName = null;
    }
  }
  #endregion

  #region Local methods
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

  string GetEntityNiceName(string prefabName) {
    // FIXME: lookup name via Loc somehow.
    return prefabName.Split(new[] { '.' }, 2)[0];
  }

  string GetEntityNiceName(BaseComponent obj) {
    // FIXME: lookup name via Loc somehow.
    return GetEntityNiceName(
        obj.GetComponentFast<Prefab>()
            .Name);
  }
  #endregion
}

}
