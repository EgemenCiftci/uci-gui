using Prism.Ioc;
using Prism.Unity;
using System.Windows;
using UciGui.Services;
using UciGui.Views;

namespace UciGui;

public partial class App : PrismApplication
{
    protected override Window CreateShell()
    {
        return Container.Resolve<MainWindow>();
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        _ = containerRegistry.RegisterSingleton<UciService>();
    }
}
