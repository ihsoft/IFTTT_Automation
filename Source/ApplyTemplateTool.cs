using System;
using System.Collections.Generic;
using System.Linq;
using Bindito.Core;
using TimberApi.ToolGroupSystem;
using TimberApi.ToolSystem;
using Timberborn.AreaSelectionSystem;
using Timberborn.BlockSystem;
using Timberborn.BuilderPrioritySystem;
using Timberborn.ConstructibleSystem;
using Timberborn.CoreUI;
using Timberborn.InputSystem;
using Timberborn.Localization;
using Timberborn.ToolSystem;
using UnityDev.LogUtils;
using UnityEngine;
using ToolGroupSpecification = Timberborn.ToolSystem.ToolGroupSpecification;

namespace IFTTT_Automation {

class ApplyTemplateTool : Tool, IInputProcessor {
  readonly ToolSpecification _toolSpecification;
  readonly DependencyInjection _injected;
     
  BlockObjectSelectionDrawer _highlightSelectionDrawer;
  BlockObjectSelectionDrawer _actionSelectionDrawer;
  AreaBlockObjectPicker _areaBlockObjectPicker;

  bool _isInProgress;

  #region Dependency injection class
  internal class DependencyInjection {
    public readonly AreaBlockObjectPickerFactory AreaBlockObjectPickerFactory;
    public readonly InputService InputService;
    public readonly BlockObjectSelectionDrawerFactory BlockObjectSelectionDrawerFactory;
    public readonly CursorService CursorService;
    public readonly ILoc Loc;
    public readonly Colors Colors;

    public DependencyInjection(AreaBlockObjectPickerFactory areaBlockObjectPickerFactory, InputService inputService,
                               BlockObjectSelectionDrawerFactory blockObjectSelectionDrawerFactory,
                               CursorService cursorService, ILoc loc, Colors colors) {
      AreaBlockObjectPickerFactory = areaBlockObjectPickerFactory;
      InputService = inputService;
      BlockObjectSelectionDrawerFactory = blockObjectSelectionDrawerFactory;
      CursorService = cursorService;
      Loc = loc;
      Colors = colors;
    }
  }
  #endregion

  #region Tool factory class
  internal class Factory : IToolFactory {
    readonly DependencyInjection _dependencyInjection;
    
    public string Id => "IFTTTAutomationTemplate";

    public Factory(DependencyInjection dependencyInjection) {
      _dependencyInjection = dependencyInjection;
    }

    public Tool Create(ToolSpecification toolSpecification, ToolGroup toolGroup) {
      return new ApplyTemplateTool(toolGroup, toolSpecification, _dependencyInjection);
    }
  }
  #endregion

  public static void Configure(IContainerDefinition containerDefinition) {
    containerDefinition.Bind<DependencyInjection>().AsSingleton();
    containerDefinition.MultiBind<IToolFactory>().To<Factory>().AsSingleton();
  }

  public ApplyTemplateTool(ToolGroup toolGroup, ToolSpecification toolSpecification,
                           DependencyInjection dependencyInjection) {
    ToolGroup = toolGroup;
    _toolSpecification = toolSpecification;
    _injected = dependencyInjection;
    _highlightSelectionDrawer = _injected.BlockObjectSelectionDrawerFactory.Create(
        _injected.Colors.PriorityHighlightColor, _injected.Colors.PriorityTileColor,
        _injected.Colors.PrioritySideColor);
    _actionSelectionDrawer = dependencyInjection.BlockObjectSelectionDrawerFactory.Create(
        _injected.Colors.PriorityActionColor, _injected.Colors.PriorityTileColor, _injected.Colors.PrioritySideColor);
  }

  public override ToolDescription Description() {
    if (!_isInProgress) {
      return null;
    }
    return new ToolDescription.Builder()
        .AddPrioritizedSection("Just section")
        .AddPrioritizedSection("It's in progress")
        .Build();
  }

  public override void Enter() {
    DebugEx.Warning("**** Enter");
    _injected.InputService.AddInputProcessor(this);
    _areaBlockObjectPicker = _injected.AreaBlockObjectPickerFactory.Create();
    _isInProgress = true;
  }

  public override void Exit() {
    DebugEx.Warning("**** Exit");
    _highlightSelectionDrawer.StopDrawing();
    _actionSelectionDrawer.StopDrawing();
    _injected.InputService.RemoveInputProcessor(this);
    _isInProgress = false;
  }

  public bool ProcessInput() {
    return _areaBlockObjectPicker.PickBlockObjects<BuilderPrioritizable>(PreviewCallback, ActionCallback, ShowNoneCallback);
  }

  void PreviewCallback(IEnumerable<BlockObject> blockObjects, Vector3Int start, Vector3Int end, bool selectionStarted,
                       bool selectingArea) {
    // FIXME: Move to abstract
    var objects = blockObjects.Where(
        bo => {
          var component = bo.GetComponentFast<Constructible>();
          return component != null && component.enabled && component.IsUnfinished;
        });
    if (selectionStarted && !selectingArea) {
      _actionSelectionDrawer.Draw(objects, start, end, selectingArea: false);
    } else if (selectingArea) {
      _actionSelectionDrawer.Draw(objects, start, end, selectingArea: true);
    } else {
      _highlightSelectionDrawer.Draw(objects, start, end, selectingArea: false);
    }
  }

  void ActionCallback(IEnumerable<BlockObject> blockObjects, Vector3Int start, Vector3Int end, bool selectionStarted, bool selectingArea) {
    foreach (BlockObject blockObject in blockObjects)
    {
      var component = blockObject.GetComponentFast<Constructible>();
      if (component == null) {
        DebugEx.Warning("*** unexpectably null");
        continue;
      }
      DebugEx.Warning("*** selected: {0}, status={1}", component, component.ConstructionState);
    }
    ClearHighlights();
  }

  void ShowNoneCallback() {
    ClearHighlights();
  }

  void ClearHighlights() {
    _highlightSelectionDrawer.StopDrawing();
    _actionSelectionDrawer.StopDrawing();
  }
}

}
