using System;

namespace FrameworkLibrary.Options
{
    public class ApplicationOptions
    {
        public ApplicationOptions()
        {
            Console.WriteLine("Hello");
        }

        public string MainWindowClassName { get; set; } = string.Empty;
        public int HostStopTimeoutSeconds { get; set; } = 0;
    }
}
