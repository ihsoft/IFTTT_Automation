using Bindito.Core;
using IFTTT_Automation.Templates;
using IFTTT_Automation.Tools;
using IFTTT_Automation.Utils;
using TimberApi.ConfiguratorSystem;
using TimberApi.SceneSystem;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.TemplateSystem;

namespace IFTTT_Automation {

// ReSharper disable once InconsistentNaming
[Configurator(SceneEntrypoint.InGame)]
class Configurator : IConfigurator {
  public void Configure(IContainerDefinition containerDefinition) {
    CustomToolSystem.BindGroupWithConstructionModeEnabler(containerDefinition, "AutomationToolGroup");
    CustomToolSystem.BindTool<PauseTool>(containerDefinition);
    CustomToolSystem.BindTool<ResumeTool>(containerDefinition);
    CustomToolSystem.BindTool<CancelTool>(containerDefinition);
    CustomToolSystem.BindTool<ApplyTemplateTool>(containerDefinition, "IFTTTAutomationTemplate");
    containerDefinition.MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
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
