using FrameworkLibrary.Services;
using UnitTests.FrameworkLibraryTests.Utils;
using Xunit;

namespace UnitTests.FrameworkLibraryTests.ServicesTests
{
    public class WindowFactoryTests
    {
        public interface IMockWindowType1
        {

        }

        public interface IMockWindowType2
        {

        }

        [Fact]
        public void WindowFactory_CreateWindow_WhenWindowTypeIsRegistered_ReturnsValidInstance()
        {
            //setup
            var services = new MockUtils<WindowFactory>();
            services.AddSingleton<IMockWindowType1>();
            services.AddSingleton<IMockWindowType2>();

            var windowFactory = services.GetMainInstance();

            //execute
            var window1 = windowFactory.CreateWindow<IMockWindowType1>();
            var window2 = windowFactory.CreateWindow<IMockWindowType2>();

            //assert
            Assert.NotNull(window1);
            Assert.NotNull(window2);
        }

        [Fact]
        public void WindowFactory_CreateWindow_WhereWindowTypeIsNotRegistered_ReturnsNull()
        {
            //setup
            var services = new MockUtils<WindowFactory>();

            var windowFactory = services.GetMainInstance();

            //execute
            var window1 = windowFactory.CreateWindow<IMockWindowType1>();

            //assert
            Assert.Null(window1);
        }
    }
}
