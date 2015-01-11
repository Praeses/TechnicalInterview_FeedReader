using System.Web.Mvc;
using FeedReader.Controllers;
using FeedReader.Domain;
using FeedReader.Infastructure;
using Microsoft.Practices.Unity;
using Unity.Mvc4;

namespace FeedReader
{
  public static class Bootstrapper
  {
    public static IUnityContainer Initialise()
    {
      var container = BuildUnityContainer();

      DependencyResolver.SetResolver(new UnityDependencyResolver(container));

      return container;
    }

    private static IUnityContainer BuildUnityContainer()
    {
      var container = new UnityContainer();
        
      RegisterTypes(container);

      return container;
    }

    public static void RegisterTypes(IUnityContainer container)
    {
        container.RegisterType<AccountController>(new InjectionConstructor());
        container.RegisterType<IFeedReaderDataSource, FeedReaderDb>();
    }
  }
}