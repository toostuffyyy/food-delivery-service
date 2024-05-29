using System;
using System.IO;
using System.Threading.Tasks;

namespace desktop.Services
{
    public interface IFilePickerService
    {
        public enum Filter
        {
            JpgImage,
            Docx
        }
        public void RegisterProvider(object provider);
        public Task<Stream> OpenFile(Filter filter);
        public Task<Uri> SaveFile(Filter filter);
    }
}
