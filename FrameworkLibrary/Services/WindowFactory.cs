using CommonServiceLocator;
using FrameworkLibrary.Interfaces;
using System;

namespace FrameworkLibrary.Services
{
    public class WindowFactory : IWindowFactory, IServiceLocatorClient
    {

        public TWindowType? CreateWindow<TWindowType>()
        {
            return (this as IServiceLocatorClient).ServiceLocator.GetInstance<TWindowType>();
        }

        private IServiceLocator? _serviceLocator;
        IServiceLocator IServiceLocatorClient.ServiceLocator
        {
            get => _serviceLocator ?? ServiceLocator.Current;
            set => _serviceLocator = value;
        }
    }
}
