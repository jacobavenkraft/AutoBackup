using CommonServiceLocator;

namespace FrameworkLibrary.Interfaces
{
    public interface IServiceLocatorClient
    {
        IServiceLocator ServiceLocator { get; set; }
    }
}
