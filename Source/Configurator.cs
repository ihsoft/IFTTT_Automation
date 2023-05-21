using Bindito.Core;
using IFTTT_Automation.Templates;
using TimberApi.ConfiguratorSystem;
using TimberApi.SceneSystem;
using TimberApi.ToolGroupSystem;
using Timberborn.ConstructionMode;

using ToolGroupSpecification = TimberApi.ToolGroupSystem.ToolGroupSpecification;

namespace IFTTT_Automation {

// ReSharper disable once InconsistentNaming
[Configurator(SceneEntrypoint.InGame)]
class Configurator : IConfigurator {
  public void Configure(IContainerDefinition containerDefinition) {
    containerDefinition.MultiBind<IToolGroupFactory>().To<RootToolGroupFactory>().AsSingleton();
    ApplyTemplateTool.Configure(containerDefinition);
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
