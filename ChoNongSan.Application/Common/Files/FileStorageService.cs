using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.Application.Common.Files
{
    public class FileStorageService : IStorageService
    {
        private readonly string _userContentFolder;
        private readonly string _bannerContentFolder;
        private readonly string _postContentFolder;
        private readonly string _categoryContentFolder;
        private const string USER_CONTENT_FOLDER_NAME = "user-content";
        private const string BANNER_CONTENT_FOLDER_NAME = "banner-content";
        private const string POST_CONTENT_FOLDER_NAME = "post-content";
        private const string CATEGORY_CONTENT_FOLDER_NAME = "category-content";

        public FileStorageService(IWebHostEnvironment webHostEnvironment)
        {
            _userContentFolder = Path.Combine(webHostEnvironment.WebRootPath, USER_CONTENT_FOLDER_NAME);
            _bannerContentFolder = Path.Combine(webHostEnvironment.WebRootPath, BANNER_CONTENT_FOLDER_NAME);
            _postContentFolder = Path.Combine(webHostEnvironment.WebRootPath, POST_CONTENT_FOLDER_NAME);
            _categoryContentFolder = Path.Combine(webHostEnvironment.WebRootPath, CATEGORY_CONTENT_FOLDER_NAME);
        }

        public string GetFileUrl(string fileName)
        {
            return $"/{USER_CONTENT_FOLDER_NAME}/{fileName}";
        }

        public async Task SaveFileAsync(Stream mediaBinaryStream, string fileName, string type)
        {
            string filePath = null;
            if (type.Contains("user")) filePath = Path.Combine(_userContentFolder, fileName);
            if (type.Contains("post")) filePath = Path.Combine(_postContentFolder, fileName);
            if (type.Contains("banner")) filePath = Path.Combine(_bannerContentFolder, fileName);
            if (type.Contains("category")) filePath = Path.Combine(_categoryContentFolder, fileName);
            using var output = new FileStream(filePath, FileMode.Create);
            await mediaBinaryStream.CopyToAsync(output);
        }

        public async Task DeleteFileAsync(string fileName)
        {
            var filePath = Path.Combine(_userContentFolder, fileName);
            if (File.Exists(filePath))
            {
                await Task.Run(() => File.Delete(filePath));
            }
        }
    }
}