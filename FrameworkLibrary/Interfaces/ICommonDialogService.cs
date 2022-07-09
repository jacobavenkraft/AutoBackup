using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameworkLibrary.Interfaces
{
    public interface ICommonDialogService
    {
        string? FolderPickerDialog(string? startingDirectory = "", string? title = "", string? okButtonText = "");
    }
}
