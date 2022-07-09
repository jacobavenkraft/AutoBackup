using FrameworkLibrary.Interfaces;
using FrameworkLibrary.Interop;

namespace FrameworkLibrary.Services
{
    public class CommonDialogService : ICommonDialogService
    {
        public const string FallbackTitle = "Select Folder";
        public const string FallbackOkButtonText = "Select";

        private IFileSystemUtility _fileSystemUtility;
        private IWindowFactory _windowFactory;

        public CommonDialogService(IFileSystemUtility fileSystemUtility, IWindowFactory windowFactory)
        {
            _fileSystemUtility = fileSystemUtility;
            _windowFactory = windowFactory;
        }

        public string? FolderPickerDialog(string? startingDirectory = "", string? title = "", string? okButtonText = "")
        {
            var folderPickerDialog = _windowFactory.CreateWindow<IFolderPickerDialog>();

            if (folderPickerDialog == null)
            {
                return null;
            }

            folderPickerDialog.Title = string.IsNullOrWhiteSpace(title) ? FallbackTitle : title;

            if (!string.IsNullOrWhiteSpace(startingDirectory) && _fileSystemUtility.DirectoryExists(startingDirectory))
            {
                folderPickerDialog.InputPath = startingDirectory;
            }
            
            folderPickerDialog.OkButtonLabel = string.IsNullOrWhiteSpace(okButtonText) ? FallbackOkButtonText : okButtonText;

            if (folderPickerDialog.ShowDialog() == true)
            {
                return folderPickerDialog.ResultPath;
            }
            else
            {
                return null;
            }
        }
    }
}
