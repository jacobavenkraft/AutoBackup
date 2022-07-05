using FrameworkLibrary.Interfaces;
using FrameworkLibrary.Interop;

namespace FrameworkLibrary.Services
{
    public class CommonDialogService : ICommonDialogService
    {
        private IFileSystemUtility _fileSystemUtility;

        public CommonDialogService(IFileSystemUtility fileSystemUtility)
        {
            _fileSystemUtility = fileSystemUtility;
        }

        public string? FolderPickerDialog(string startingDirectory = "", string title = "", string okButtonText = "")
        {
            var folderPickerDialog = new FolderPickerDialog();
            folderPickerDialog.Title = string.IsNullOrWhiteSpace(title) ? "Select Folder" : title;

            if (_fileSystemUtility.DirectoryExists(startingDirectory))
            {
                folderPickerDialog.InputPath = startingDirectory;
            }
            
            folderPickerDialog.OkButtonLabel = string.IsNullOrWhiteSpace(okButtonText) ? "Select" : okButtonText;

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
