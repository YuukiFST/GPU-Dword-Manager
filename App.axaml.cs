using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using GPU_Dword_Manager_Avalonia.Models;
using GPU_Dword_Manager_Avalonia.Services;
using GPU_Dword_Manager_Avalonia.Services.Strategies;
using GPU_Dword_Manager_Avalonia.ViewModels;
using GPU_Dword_Manager_Avalonia.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace GPU_Dword_Manager_Avalonia
{
    public partial class App : Application
    {
        public IServiceProvider? Services { get; private set; }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            var collection = new ServiceCollection();
            ConfigureServices(collection);
            Services = collection.BuildServiceProvider();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.ShutdownMode = ShutdownMode.OnLastWindowClose;
                var vendorSelectionVm = Services.GetRequiredService<VendorSelectionViewModel>();
                var window = new VendorSelectionWindow(vendorSelectionVm);
                desktop.MainWindow = window;
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IRegistryService, RegistryService>();
            services.AddSingleton<Func<GpuVendor, IDwordParser>>(provider => vendor => new DwordParser(vendor));
            services.AddSingleton<Func<GpuVendor, ITweakParser>>(provider => vendor => new TweakParser(vendor));
            services.AddTransient<ITweakActionHandler, DwordActionHandler>();
            services.AddTransient<ITweakActionHandler, CommandActionHandler>();
            services.AddTransient<ITweakActionHandler, RegistryImportActionHandler>();
            services.AddSingleton<Func<GpuVendor, ITweakService>>(provider => vendor => 
                new TweakManager(
                    provider.GetRequiredService<IRegistryService>(),
                    provider.GetRequiredService<IEnumerable<ITweakActionHandler>>(),
                    vendor
                ));
            services.AddTransient<VendorSelectionViewModel>();
            services.AddTransient<MainViewModel>();
        }
    }
}
