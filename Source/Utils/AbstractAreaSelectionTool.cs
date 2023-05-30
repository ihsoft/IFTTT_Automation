// Timberborn Utils
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

using System.Collections.Generic;
using System.Linq;
using Bindito.Core;
using TimberApi.ToolSystem;
using Timberborn.AreaSelectionSystem;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.BuilderPrioritySystem;
using Timberborn.CoreUI;
using Timberborn.InputSystem;
using Timberborn.Localization;
using Timberborn.ToolSystem;
using UnityEngine;

namespace IFTTT_Automation.Utils {

public abstract class AbstractAreaSelectionTool : CustomToolRegistry.CustomTool, IInputProcessor {
  #region Internal fields
  BlockObjectSelectionDrawer _highlightSelectionDrawer;
  BlockObjectSelectionDrawer _actionSelectionDrawer;
  AreaBlockObjectPicker _areaBlockObjectPicker;
  #endregion

  #region Injections
  protected AreaBlockObjectPickerFactory AreaBlockObjectPickerFactory;
  protected InputService InputService;
  protected BlockObjectSelectionDrawerFactory BlockObjectSelectionDrawerFactory;
  protected CursorService CursorService;
  protected ILoc Loc;
  #endregion

  #region Tool setup
  /// <summary>Color of the matching object when hovering over in non-selecting mode.</summary>
  protected Color HighlightColor = Color.blue;

  /// <summary>Color of the matching object in the selection range.</summary>
  protected Color ActionColor = Color.red;

  /// <summary>Color of the ground tile in the selection range.</summary>
  protected Color TileColor = Color.blue;

  /// <summary>Color of the border of the selection range.</summary>
  protected Color SideColor = Color.blue;
  #endregion

  #region API
  /// <summary>Filters the objects that the tool will handle.</summary>
  /// <param name="blockObject">The object to check the expression for.</param>
  /// <param name="singleElement">Tells if it's a single element click vs area selection.</param>
  protected abstract bool ObjectFilterExpression(BlockObject blockObject, bool singleElement);

  /// <summary>
  /// A callback that is called on every selected object that matches <see cref="ObjectFilterExpression"/>. 
  /// </summary>
  /// <param name="blockObject">The object to check the expression for.</param>
  /// <param name="singleElement">Tells if it's a single element click vs area selection.</param>
  protected abstract void OnObjectAction(BlockObject blockObject, bool singleElement);

  /// <summary>A callback that is called when a new selection has just started.</summary>
  /// <param name="blockObject">
  /// Any block object that was focused at the moment of the selection start. It's not filtered by
  /// <see cref="ObjectFilterExpression"/>.
  /// </param>
  /// <param name="position">The selection start position.</param>
  protected abstract void OnSelectionStarted(BlockObject blockObject, Vector3Int position);
  #endregion

  #region Tool overrides
  /// <inheritdoc/>
  public override void Enter() {
    InputService.AddInputProcessor(this);
    _areaBlockObjectPicker = AreaBlockObjectPickerFactory.Create();
  }

  /// <inheritdoc/>
  public override void Exit() {
    _highlightSelectionDrawer.StopDrawing();
    _actionSelectionDrawer.StopDrawing();
    InputService.RemoveInputProcessor(this);
  }
  #endregion

  #region IInputProcessor implementation
  /// <inheritdoc/>
  public bool ProcessInput() {
    return _areaBlockObjectPicker.PickBlockObjects<BuilderPrioritizable>(
        PreviewCallback, ActionCallback, ShowNoneCallback);
  }
  #endregion

  #region CustomTool overrides
  /// <inheritdoc/>
  public override void InitializeTool(ToolGroup toolGroup, ToolSpecification toolSpecification) {
    base.InitializeTool(toolGroup, toolSpecification);
    _highlightSelectionDrawer = BlockObjectSelectionDrawerFactory.Create(HighlightColor, TileColor, SideColor);
    _actionSelectionDrawer = BlockObjectSelectionDrawerFactory.Create(ActionColor, TileColor, SideColor);
  }
  #endregion

  #region Local methods
  [Inject]
  public void InjectDependencies(AreaBlockObjectPickerFactory areaBlockObjectPickerFactory, InputService inputService,
                                 BlockObjectSelectionDrawerFactory blockObjectSelectionDrawerFactory,
                                 CursorService cursorService, ILoc loc) {
    AreaBlockObjectPickerFactory = areaBlockObjectPickerFactory;
    InputService = inputService;
    BlockObjectSelectionDrawerFactory = blockObjectSelectionDrawerFactory;
    CursorService = cursorService;
    Loc = loc;
  }

  void PreviewCallback(IEnumerable<BlockObject> blockObjects, Vector3Int start, Vector3Int end,
                       bool selectionStarted, bool selectingArea) {
    if (selectionStarted && !_selectionModeActive) {
      _selectionModeActive = true;
      OnSelectionStarted(blockObjects.FirstOrDefault(), start);
    }
    var objects = blockObjects.Where(x => ObjectFilterExpression(x, !selectingArea));
    if (selectionStarted) {
      _actionSelectionDrawer.Draw(objects, start, end, selectingArea);
    } else {
      _highlightSelectionDrawer.Draw(objects, start, end, selectingArea: false);
    }
  }
  bool _selectionModeActive;

  void ActionCallback(IEnumerable<BlockObject> blockObjects, Vector3Int start, Vector3Int end,
                      bool selectionStarted, bool selectingArea) {
    blockObjects
        .Where(x => ObjectFilterExpression(x, !selectingArea))
        .ToList()
        .ForEach(x => OnObjectAction(x, !selectingArea));
    ClearHighlights();
    _selectionModeActive = false;
  }

  void ShowNoneCallback() {
    ClearHighlights();
  }

  void ClearHighlights() {
    _highlightSelectionDrawer.StopDrawing();
    _actionSelectionDrawer.StopDrawing();
  }
  #endregion
}

}
