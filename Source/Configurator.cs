using Bindito.Core;
using IFTTT_Automation.Templates;
using IFTTT_Automation.Utils;
using TimberApi.ConfiguratorSystem;
using TimberApi.SceneSystem;
using TimberApi.ToolGroupSystem;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.ConstructionMode;
using Timberborn.TemplateSystem;
using ToolGroupSpecification = TimberApi.ToolGroupSystem.ToolGroupSpecification;

namespace IFTTT_Automation {

// ReSharper disable once InconsistentNaming
[Configurator(SceneEntrypoint.InGame)]
class Configurator : IConfigurator {
  public void Configure(IContainerDefinition containerDefinition) {
    containerDefinition.MultiBind<IToolGroupFactory>().To<RootToolGroupFactory>().AsSingleton();
    CustomToolRegistry.BindTool<ApplyTemplateTool>(containerDefinition, "IFTTTAutomationTemplate");
    containerDefinition.MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
  }

  static TemplateModule ProvideTemplateModule() {
    TemplateModule.Builder builder = new TemplateModule.Builder();
    //builder.AddDecorator<Constructible, IFTTTAutomationTestRule>();
    builder.AddDecorator<BlockObject, AutomationBehavior>();
    builder.AddDecorator<BaseInstantiator, AutomationBehavior>();//FIXME
    return builder.Build();
  }
}

class RootToolGroupFactory : IToolGroupFactory {
  public string Id => "IFTTTAutomationToolGroup";

  public IToolGroup Create(ToolGroupSpecification toolGroupSpecification) {
    return new RootTooGroup(toolGroupSpecification);
  }

  // Implement `IConstructionModeEnabler` to have the unfinished constructable objects shown.
  class RootTooGroup : ApiToolGroup, IConstructionModeEnabler {
    public RootTooGroup(ToolGroupSpecification specification) : base(
        specification.Id, specification.GroupId, specification.Order, specification.Section, specification.NameLocKey,
        specification.DevMode, specification.Icon) {
    }
  }
}

}
