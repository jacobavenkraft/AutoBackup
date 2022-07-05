using CommonServiceLocator;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace FrameworkLibrary.Services
{
    public class ServiceLocatorService : IServiceLocator
    {
        private IServiceProvider _serviceProvider;

        public ServiceLocatorService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public IEnumerable<object?> GetAllInstances(Type serviceType) => _serviceProvider.GetServices(serviceType);

        public IEnumerable<TService> GetAllInstances<TService>() => _serviceProvider.GetServices<TService>();

        public object? GetInstance(Type serviceType) => _serviceProvider.GetService(serviceType);

        public object? GetInstance(Type serviceType, string key) => _serviceProvider.GetService(serviceType); //TODO: what do we do with the key? not used for my purposes

        public TService? GetInstance<TService>() => _serviceProvider.GetService<TService>();

        public TService? GetInstance<TService>(string key) => _serviceProvider.GetService<TService>(); //TODO: what do we do with the key? not used for my purposes

        public object? GetService(Type serviceType) => _serviceProvider.GetService(serviceType);
    }
}
