using System;
using System.Windows;

namespace FrameworkLibrary.Interop
{
    public interface IFolderPickerDialog
    {
        string ResultPath { get; }
        string ResultName { get; }
        string InputPath { get; set; }
        bool ForceFileSystem { get; set; }
        string Title { get; set; }
        string OkButtonLabel { get; set; }
        string FileNameLabel { get; set; }

        bool? ShowDialog(Window? owner = null, bool throwOnError = false);

        bool? ShowDialog(IntPtr owner, bool throwOnError = false);
    }
}
