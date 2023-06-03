using IFTTT_Automation.Utils;
using TimberApi.ToolSystem;
using Timberborn.BlockSystem;
using Timberborn.BuildingsBlocking;
using Timberborn.ToolSystem;
using UnityEngine;

namespace IFTTT_Automation.Tools {

// ReSharper disable once ClassNeverInstantiated.Global
sealed class ResumeTool : AbstractAreaSelectionTool {
  #region Tool overrides
  /// <inheritdoc/>
  public override ToolDescription Description() {
    var unused = Loc.T("blah");//FIXME
    return new ToolDescription.Builder(Loc.T(ToolSpecification.NameLocKey))
        .AddSection(Loc.T(ToolSpecification.DescriptionLocKey))
        .AddPrioritizedSection(Loc.T("IgorZ.Automation.ResumeTool.Hint"))
        .Build();
  }
  #endregion

  #region CustomTool overrides
  /// <inheritdoc/>
  public override void InitializeTool(ToolGroup toolGroup, ToolSpecification toolSpecification) {
    HighlightColor = ActionColor = Color.green;
    TileColor = SideColor = Color.white;
    base.InitializeTool(toolGroup, toolSpecification);
  }
  #endregion

  #region AbstractAreaSelectionTool overries
  /// <inheritdoc/>
  protected override bool ObjectFilterExpression(BlockObject blockObject) {
    var component = blockObject.GetComponentFast<PausableBuilding>();
    return component != null && component.enabled && component.IsPausable() && component.Paused;
  }

  /// <inheritdoc/>
  protected override void OnObjectAction(BlockObject blockObject) {
    blockObject.GetComponentFast<PausableBuilding>().Resume();
  }
  #endregion
}

}
