using System.Runtime.CompilerServices;
using Bindito.Core;
using TimberApi.ConfiguratorSystem;
using TimberApi.SceneSystem;
using TimberApi.ToolGroupSystem;
using TimberApi.ToolSystem;
using Timberborn.SingletonSystem;
using Timberborn.ToolSystem;
using UnityDev.LogUtils;
using UnityEngine;
using ToolGroupSpecification = TimberApi.ToolGroupSystem.ToolGroupSpecification;

namespace IFTTT_Automation {

// ReSharper disable once InconsistentNaming
[Configurator(SceneEntrypoint.InGame)]
class Configurator : IConfigurator {
  public void Configure(IContainerDefinition containerDefinition) {
    containerDefinition.MultiBind<IToolGroupFactory>().To<RootToolGroupFactory>().AsSingleton();
    //containerDefinition.MultiBind<IToolFactory>().To<ApplyTemplateTool.Factory>().AsSingleton();
    ApplyTemplateTool.Configure(containerDefinition);
  }
}
class RootToolGroupFactory : IToolGroupFactory {
  public string Id => "IFTTTAutomationToolGroup";

  public IToolGroup Create(ToolGroupSpecification toolGroupSpecification) {
    return new ApiToolGroup(
        toolGroupSpecification.Id, toolGroupSpecification.GroupId, toolGroupSpecification.Order,
        toolGroupSpecification.Section, toolGroupSpecification.NameLocKey, toolGroupSpecification.DevMode,
        toolGroupSpecification.Icon);
  }
}

}
