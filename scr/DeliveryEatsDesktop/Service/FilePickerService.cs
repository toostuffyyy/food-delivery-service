using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace desktop.Services
{
    public class FilePickerService : IFilePickerService
    {
        private TopLevel? _topLevel;
        public void RegisterProvider(object provider)
        {
            if (provider is TopLevel)
                _topLevel = (TopLevel)provider;
            Debug.WriteLine("");
        }
        
        public async Task<Stream> OpenFile(IFilePickerService.Filter filter)
        {
            IReadOnlyList<IStorageFile> files = null;
            if (filter == IFilePickerService.Filter.JpgImage)
            {
                files = await _topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
                {
                    Title = "Выбор файла",
                    AllowMultiple = false,
                    FileTypeFilter = new[] {FilePickerFileTypes.ImageJpg}
                });
            }
            if(files != null && files.Count >= 1)
            {
                return await files[0].OpenReadAsync();
            }
            return null;
        }
        
        public static FilePickerFileType Doc { get; } = new("Docx")
        {
            Patterns = new[] {"*.docx"},
            AppleUniformTypeIdentifiers = new[] {"public.document"},
            MimeTypes = new[] {"document/*"}
        };
        
        public async Task<Uri> SaveFile(IFilePickerService.Filter filter)
        {
            IStorageFile? storageFile = null;
            if (filter == IFilePickerService.Filter.Docx)
                storageFile = await _topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions()
                {
                    Title = "Сохранить файл",
                    SuggestedFileName = "template.docx",
                    FileTypeChoices = new[] {Doc}
                });
            if (storageFile != null)
                return storageFile.Path;
            return null;
        }
    }
}
