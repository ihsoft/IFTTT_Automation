// Timberborn Utils
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

using Automation.Templates;
using Automation.Tools;
using Automation.Utils;
using Bindito.Core;
using TimberApi.ConfiguratorSystem;
using TimberApi.SceneSystem;
using Timberborn.BlockSystem;
using Timberborn.TemplateSystem;

namespace Automation {

// ReSharper disable once InconsistentNaming
[Configurator(SceneEntrypoint.InGame)]
sealed class Configurator : IConfigurator {
  public void Configure(IContainerDefinition containerDefinition) {
    CustomToolSystem.BindGroupWithConstructionModeEnabler(containerDefinition, "AutomationToolGroup");
    CustomToolSystem.BindTool<PauseTool>(containerDefinition);
    CustomToolSystem.BindTool<ResumeTool>(containerDefinition);
    CustomToolSystem.BindTool<CancelTool>(containerDefinition);
    CustomToolSystem.BindTool<ApplyTemplateTool>(containerDefinition);
    containerDefinition.MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
    containerDefinition.Bind<AutomationService>().AsSingleton();
  }

  static TemplateModule ProvideTemplateModule() {
    TemplateModule.Builder builder = new TemplateModule.Builder();
    //builder.AddDecorator<Constructible, IFTTTAutomationTestRule>();
    builder.AddDecorator<BlockObject, AutomationBehavior>();
    //builder.AddDecorator<BaseInstantiator, AutomationBehavior>();//FIXME
    return builder.Build();
  }
}

}
