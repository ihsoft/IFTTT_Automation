// Timberborn Utils
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

using System.Text;
using TimberApi.UiBuilderSystem;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.Localization;
using UnityEngine;
using UnityEngine.UIElements;

namespace Automation.UI {

sealed class AutomationFragment : IEntityPanelFragment {
  readonly UIBuilder _builder;
  readonly ILoc _loc;

  VisualElement _root;
  Label _caption;
  Label _rulesList;

  public AutomationFragment(UIBuilder builder, ILoc loc) {
    _builder = builder;
    _loc = loc;
  }

  public VisualElement InitializeFragment() {
    var presets = _builder.Presets();
    _caption = presets.Labels().Label(color: Color.cyan);
    _rulesList = presets.Labels().GameText();

    UIFragmentBuilder uIFragmentBuilder = _builder.CreateFragmentBuilder()
        .AddComponent(_caption)
        .AddComponent(_rulesList);
    _root = uIFragmentBuilder.BuildAndInitialize();
    _root.ToggleDisplayStyle(visible: false);
    return _root;
  }

  public void ShowFragment(BaseComponent entity) {
    var component = entity.GetComponentFast<AutomationBehavior>();
    if (component == null || !component.HasRules) {
      return;
    }
    var rules = new StringBuilder();
    var rulesAdded = 0;
    foreach (var rule in component.Rules) {
      if (rulesAdded++ > 0) {
        rules.AppendLine();
      }
      rules.Append(SpecialStrings.RowStarter);
      rules.Append(string.Format("If {0}, then {1}", rule.Condition.GetUiDescription(_loc), rule.Action.GetUiDescription(_loc)));
    }
    _caption.text = "Automation rules:";
    _rulesList.text = TextColors.ColorizeText(rules.ToString());
    _root.ToggleDisplayStyle(visible: true);
  }

  public void ClearFragment() {
    _root.ToggleDisplayStyle(visible: false);
  }

  public void UpdateFragment() {
  }
}
}
