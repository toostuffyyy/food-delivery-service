﻿using Refit;
using System.Threading.Tasks;

namespace desktop.Services
{
    public interface IImageService
    {
        [Multipart]
        [Post("/Image/UploadImage")]
        public Task<string> UploadImage([Authorize("Bearer")] string accessToken, 
            [AliasAs("file")] StreamPart file, [Query]string path);
    }
}
