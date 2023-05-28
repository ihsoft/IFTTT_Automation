// Timberborn Utils
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Timberborn.AreaSelectionSystem;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.BuilderPrioritySystem;
using Timberborn.CoreUI;
using Timberborn.InputSystem;
using Timberborn.Localization;
using Timberborn.ToolSystem;
using UnityDev.LogUtils;
using UnityEngine;

namespace IFTTT_Automation.Utils {

public abstract class AbstractAreaSelectionTool : AbstractToolWithDependencies<AbstractAreaSelectionTool.Injections>,
                                                  IInputProcessor {
  #region Dependency injection class
  /// <summary>Common injections that may be needed by a regular area tool.</summary>
  /// <remarks>The descendants can extend this class with more injectable definitions and pass it down.</remarks>
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
  public class Injections {
    public readonly AreaBlockObjectPickerFactory AreaBlockObjectPickerFactory;
    public readonly InputService InputService;
    public readonly BlockObjectSelectionDrawerFactory BlockObjectSelectionDrawerFactory;
    public readonly CursorService CursorService;
    public readonly ILoc Loc;
    public readonly Colors Colors;
    public readonly BaseInstantiator BaseInstantiator;

    protected internal Injections(
        AreaBlockObjectPickerFactory areaBlockObjectPickerFactory, InputService inputService,
        BlockObjectSelectionDrawerFactory blockObjectSelectionDrawerFactory, CursorService cursorService, ILoc loc,
        Colors colors, BaseInstantiator baseInstantiator) {
      AreaBlockObjectPickerFactory = areaBlockObjectPickerFactory;
      InputService = inputService;
      BlockObjectSelectionDrawerFactory = blockObjectSelectionDrawerFactory;
      CursorService = cursorService;
      Loc = loc;
      Colors = colors;
      BaseInstantiator = baseInstantiator;
    }
  }
  #endregion

  #region Internal fields
  readonly BlockObjectSelectionDrawer _highlightSelectionDrawer;
  readonly BlockObjectSelectionDrawer _actionSelectionDrawer;
  AreaBlockObjectPicker _areaBlockObjectPicker;
  #endregion

  #region API
  /// <summary>Filters the objects that the tool will handle.</summary>
  protected abstract bool ObjectFilterExpression(BlockObject obj);

  /// <summary>
  /// A callback that is called on every selected object that matches <see cref="ObjectFilterExpression"/>. 
  /// </summary>
  protected abstract void OnObjectAction(BlockObject obj);
  #endregion

  #region Tool overrides
  public override void Enter() {
    Injected.InputService.AddInputProcessor(this);
    _areaBlockObjectPicker = Injected.AreaBlockObjectPickerFactory.Create();
  }

  public override void Exit() {
    _highlightSelectionDrawer.StopDrawing();
    _actionSelectionDrawer.StopDrawing();
    Injected.InputService.RemoveInputProcessor(this);
  }
  #endregion

  #region IInputProcessor implementation
  public bool ProcessInput() {
    return _areaBlockObjectPicker.PickBlockObjects<BuilderPrioritizable>(
        PreviewCallback, ActionCallback, ShowNoneCallback);
  }
  #endregion

  /// <summary>Constructs a tool that handles all the selection logic.</summary>
  /// <param name="toolGroup"></param>
  /// <param name="injected">
  /// The injectabales that are needed for this implementation. See <see cref="AbstractToolWithDependencies&lt;T&gt;"/>
  /// for more details.
  /// </param>
  /// <param name="highlightColor"></param>
  /// <param name="actionColor"></param>
  /// <param name="tileColor"></param>
  /// <param name="sideColor"></param>
  /// FIXME: make docs
  protected AbstractAreaSelectionTool(ToolGroup toolGroup, Injections injected, Color highlightColor, Color actionColor,
                                      Color tileColor, Color sideColor) : base(toolGroup, injected) {
    _highlightSelectionDrawer = injected.BlockObjectSelectionDrawerFactory.Create(highlightColor, tileColor, sideColor);
    _actionSelectionDrawer = injected.BlockObjectSelectionDrawerFactory.Create(actionColor, tileColor, sideColor);
  }

  #region Local methods
  void PreviewCallback(IEnumerable<BlockObject> blockObjects, Vector3Int start, Vector3Int end,
                       bool selectionStarted, bool selectingArea) {
    var objects = blockObjects.Where(ObjectFilterExpression);
    if (selectionStarted) {
      _actionSelectionDrawer.Draw(objects, start, end, selectingArea);
    } else {
      _highlightSelectionDrawer.Draw(objects, start, end, selectingArea: false);
    }
  }

  void ActionCallback(IEnumerable<BlockObject> blockObjects, Vector3Int start, Vector3Int end,
                      bool selectionStarted, bool selectingArea) {
    blockObjects.Where(ObjectFilterExpression).ToList().ForEach(OnObjectAction);
    ClearHighlights();
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
