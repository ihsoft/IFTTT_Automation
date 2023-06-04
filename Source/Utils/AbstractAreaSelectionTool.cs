// Timberborn Utils
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Bindito.Core;
using TimberApi.ToolSystem;
using Timberborn.AreaSelectionSystem;
using Timberborn.BlockSystem;
using Timberborn.BuilderPrioritySystem;
using Timberborn.InputSystem;
using Timberborn.ToolSystem;
using UnityEngine;

namespace IFTTT_Automation.Utils {

public abstract class AbstractAreaSelectionTool : ToolWithDescription, IInputProcessor {
  #region Internal fields
  BlockObjectSelectionDrawer _highlightSelectionDrawer;
  BlockObjectSelectionDrawer _actionSelectionDrawer;
  AreaBlockObjectPicker _areaBlockObjectPicker;
  #endregion

  #region Injections
  protected InputService InputService { get; private set; }
  AreaBlockObjectPickerFactory _areaBlockObjectPickerFactory;
  BlockObjectSelectionDrawerFactory _blockObjectSelectionDrawerFactory;
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

  #region Inhertable properties
  /// <summary>Indicates if selection mode has started (mouse click in the map).</summary>
  /// <value><c>true</c> if player clicks and holds LMB over a valid block object on the map.</value>
  /// <seealso cref="OnSelectionModeChange"/>
  protected bool SelectionModeActive {
    get => _selectionModeActive;
    private set {
      if (value == _selectionModeActive) {
        return;
      }
      OnSelectionModeChange(value);
      _selectionModeActive = value;
      if (!value) {
        SelectedObjects = null;
      }
    }
  }
  bool _selectionModeActive;

  /// <summary>Returns the block object that is currently highlighted.</summary>
  /// <value>The object under cursor or <c>null</c> if no block object can be detected.</value>
  /// <seealso cref="OnHighlightChange"/>
  protected BlockObject HighlightedBlockObject {
    get => _highlightedBlockObject;
    private set {
      if (value == _highlightedBlockObject) {
        return;
      }
      OnHighlightChange(value);
      _highlightedBlockObject = value;
    }
  }
  BlockObject _highlightedBlockObject;

  #endregion

  #region API
  /// <summary>Filters the objects that the tool will handle.</summary>
  /// <param name="blockObject">The object to check the expression for.</param>
  protected abstract bool ObjectFilterExpression(BlockObject blockObject);

  /// <summary>A callback that is called on every selected object when the action is committed.</summary>
  /// <remarks>Only objects that matches the <see cref="ObjectFilterExpression"/> will be passed.</remarks>
  /// <param name="blockObject">The object to apply action to.</param>
  protected abstract void OnObjectAction(BlockObject blockObject);

  /// <summary>A callback that is called when a new block object is about to be highlighted.</summary>
  /// <seealso cref="HighlightedBlockObject"/>
  protected virtual void OnHighlightChange(BlockObject newObject) {
  }
  /// <summary>A callback that is called when selection mode is about to change.</summary>
  /// <seealso cref="SelectionModeActive"/>
  protected virtual void OnSelectionModeChange(bool newMode) {
  }
  #endregion

  #region Tool overrides
  /// <inheritdoc/>
  public override void Enter() {
    InputService.AddInputProcessor(this);
    _areaBlockObjectPicker = _areaBlockObjectPickerFactory.Create();
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
  public virtual bool ProcessInput() {
    return _areaBlockObjectPicker.PickBlockObjects<BuilderPrioritizable>(
        PreviewCallback, ActionCallback, ShowNoneCallback);
  }
  #endregion

  #region CustomTool overrides
  /// <inheritdoc/>
  public override void InitializeTool(ToolGroup toolGroup, ToolSpecification toolSpecification) {
    base.InitializeTool(toolGroup, toolSpecification);
    _highlightSelectionDrawer = _blockObjectSelectionDrawerFactory.Create(HighlightColor, TileColor, SideColor);
    _actionSelectionDrawer = _blockObjectSelectionDrawerFactory.Create(ActionColor, TileColor, SideColor);
  }
  #endregion

  #region Local methods
  [Inject]
  public void InjectDependencies(AreaBlockObjectPickerFactory areaBlockObjectPickerFactory, InputService inputService,
                                 BlockObjectSelectionDrawerFactory blockObjectSelectionDrawerFactory) {
    _areaBlockObjectPickerFactory = areaBlockObjectPickerFactory;
    InputService = inputService;
    _blockObjectSelectionDrawerFactory = blockObjectSelectionDrawerFactory;
  }

  void PreviewCallback(IEnumerable<BlockObject> blockObjects, Vector3Int start, Vector3Int end,
                       bool selectionStarted, bool selectingArea) {
    var objects = blockObjects.ToList();
    if (selectionStarted) {
      SelectedObjects = objects.AsReadOnly();
    }
    HighlightedBlockObject = !selectingArea ? objects.FirstOrDefault() : objects.LastOrDefault();
    SelectionModeActive = selectionStarted;
    var targetObjects = objects.Where(ObjectFilterExpression);
    if (selectionStarted) {
      _actionSelectionDrawer.Draw(targetObjects, start, end, selectingArea);
    } else {
      _highlightSelectionDrawer.Draw(targetObjects, start, end, selectingArea: false);
    }
  }

  protected ReadOnlyCollection<BlockObject> SelectedObjects { get; private set; }

  void ActionCallback(IEnumerable<BlockObject> blockObjects, Vector3Int start, Vector3Int end,
                      bool selectionStarted, bool selectingArea) {
    blockObjects
        .Where(ObjectFilterExpression)
        .ToList()
        .ForEach(OnObjectAction);
    CancelSelectionMode();
  }

  void ShowNoneCallback() {
    CancelSelectionMode();
  }

  void CancelSelectionMode() {
    _highlightSelectionDrawer.StopDrawing();
    _actionSelectionDrawer.StopDrawing();
    SelectionModeActive = false;
  }
  #endregion
}

}
