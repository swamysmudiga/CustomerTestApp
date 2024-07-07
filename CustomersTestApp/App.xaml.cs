﻿using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace CustomersTestApp
{
    public partial class App : Application
    {
        public static ServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ICustomerService, Grpc_customerService>();
            services.AddSingleton<ViewModels.MainViewModel>();
            services.AddSingleton<MainWindow>();
        }
    }
}