// Timberborn Mod: Automation
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

using IFTTT_Automation.Utils;
using Timberborn.BlockSystem;
using Timberborn.BuildingsBlocking;
using UnityEngine;

namespace IFTTT_Automation.Tools {

// ReSharper disable once ClassNeverInstantiated.Global
sealed class PauseTool : AbstractLockingTool {
  #region Local fields and properties
  const string ToolHintLoc = "IgorZ.Automation.PauseTool.Hint";
  #endregion
  
  #region CustomTool overrides
  /// <inheritdoc/>
  protected override void Initialize() {
    HighlightColor = ActionColor = Color.red;
    TileColor = SideColor = Color.white;
    DescriptionHintSectionLoc = ToolHintLoc;
    base.Initialize();
  }
  #endregion

  #region AbstractAreaSelectionTool overries
  /// <inheritdoc/>
  protected override bool ObjectFilterExpression(BlockObject blockObject) {
    if (!base.ObjectFilterExpression(blockObject)) {
      return false;
    }
    var component = GetCompatibleComponent(blockObject);
    return component != null && !component.Paused;
  }

  /// <inheritdoc/>
  protected override void OnObjectAction(BlockObject blockObject) {
    blockObject.GetComponentFast<PausableBuilding>().Pause();
  }
  #endregion

  #region AbstractLockingTool overries
  /// <inheritdoc/>
  protected override bool CheckCanLockOnComponent(BlockObject obj) {
    return GetCompatibleComponent(obj) != null;
  }
  #endregion

  #region Implementation
  static PausableBuilding GetCompatibleComponent(BlockObject obj) {
    var component = obj.GetComponentFast<PausableBuilding>();
    if (component != null && component.enabled && component.IsPausable()) {
      return component;
    }
    return null;
  }
  #endregion
}

}
