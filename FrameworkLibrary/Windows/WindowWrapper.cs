using System;
using System.Windows;

namespace FrameworkLibrary.Windows
{
    public class WindowWrapper : IWindow
    {
        private Window _window;

        public WindowWrapper(Window window)
        {
            _window = window ?? throw new ArgumentNullException(nameof(window));
        }

        public void Show()
        {
            _window.Show();
        }

        public bool? ShowDialog()
        {
            return _window.ShowDialog();
        }
    }
}
