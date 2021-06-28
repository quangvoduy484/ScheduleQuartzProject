using Microsoft.AspNetCore.Mvc;
using ScheduleQuartz.Services.FileProviderService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScheduleQuartz.Controllers
{
    [ApiController]
    [Route("api/file")]
    public class FileController : ControllerBase
    {
        private readonly IFileProvider _fileProvider;
        public FileController(IFileProvider fileProvider)
        {
            _fileProvider = fileProvider;
        }

        [HttpPost("send-file")]
        public async Task<IActionResult> AddOrUpdateFile()
        {
            _fileProvider.Combine(@"C:\Users\DELL\Desktop", "v1.jpg");

            //Test File => khi nó bắt đầu bằng một dấu \ tuyệt đối không là tương đôi
            var test = System.IO.Path.IsPathRooted(@"c:\foo"); // true
            var test1 = System.IO.Path.IsPathRooted(@"\foo"); // true
            var test2 = System.IO.Path.IsPathRooted("foo"); // false

            // create file
            var path = @"E:\TestFile";

            //_fileProvider.CreateFile(path);
            //var writeText = Encoding.UTF8.GetBytes("VõDuyQuang");
            //var overrideText = "ĐặngTiếnHoàng";

            //_fileProvider.WriteAllBytes(path, writeText);
            //_fileProvider.WriteAllText(path, overrideText, Encoding.UTF8);

            //var overrideText = _fileProvider.ReadAllTexts(path, Encoding.UTF8);
            //overrideText = overrideText.Replace("oàng", "hùng");
            //_fileProvider.WriteAllText(path, overrideText, Encoding.UTF8);

            _fileProvider.DeleteFiles(path);

            return Ok();
        }
    }
}
