using DHK.Blazor.Module.Interfaces;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHK.Blazor.Module.Services
{
    public class LocalFileService : ILocalFileService
    {
        private readonly IWebHostEnvironment _env;

        public LocalFileService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public string SaveExcelTemplate(byte[] content, string fileName)
        {
            string templatesPath = Path.Combine(_env.WebRootPath, "templates");
            Directory.CreateDirectory(templatesPath);

            string fullPath = Path.Combine(templatesPath, fileName);
            File.WriteAllBytes(fullPath, content);

            return $"/templates/{fileName}";
        }
    }
}
