using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScheduleQuartz.Services.FileProviderService
{
    public class FileProvider : IFileProvider
    {
        protected static bool IsUncPath(string path)
        {
            var result =  Uri.TryCreate(path, UriKind.Absolute, out var uri) && uri.IsUnc;
            return result;
        }
            
        public string Combine(params string[] paths)
        {
            var path = Path.Combine(paths.SelectMany(p => IsUncPath(p) ? new[] { p } : p.Split('\\', '/')).ToArray());

            if (Environment.OSVersion.Platform == PlatformID.Unix && !IsUncPath(path))
                //add leading slash to correctly form path in the UNIX system
                path = "/" + path;

            return path;
        }

        public bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        public void CreateDirectory(string path)
        {
            if (!DirectoryExists(path))
                Directory.CreateDirectory(path);
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public void CreateFile(string path)
        {
            if (FileExists(path))
                return;

            FileInfo fileInfo = new FileInfo(path);
            ///test thử file strea coi thử tạo được không

            CreateDirectory(fileInfo.DirectoryName);

            //we use 'using' to close the file after it's created
            using (File.Create(path))
            {

            }
        }

        public DateTime GetCreationTime(string path)
        {
            return File.GetCreationTime(path);
        }

        public DateTime GetLastAccessTime(string path)
        {
            return File.GetLastAccessTime(path);
        }

        public DateTime GetLastWriteTime(string path)
        {
            return File.GetLastWriteTime(path);
        }

        public void WriteAllBytes(string filePath, byte[] bytes)
        {
            if (!FileExists(filePath))
                return;

            File.WriteAllBytes(filePath, bytes);
        }

        public void WriteAllText(string filePath, string contents, Encoding encoding)
        {
            if (!FileExists(filePath))
                return;

            File.WriteAllText(filePath,contents, encoding);
        }

        public byte[] ReadAllBytes(string filePath)
        {
            return File.Exists(filePath) ?
                File.ReadAllBytes(filePath) :
                Array.Empty<byte>();
        }

        public string ReadAllTexts(string filePath, Encoding encoding)
        {
            return File.Exists(filePath) ?
               File.ReadAllText(filePath,encoding) :
               string.Empty;
        }

        public void DeleteFile(string filePath)
        {
            Directory.Delete(filePath , true);
            const int maxIterationToWait = 10;
            var curIteration = 0;

            //according to the documentation(https://msdn.microsoft.com/ru-ru/library/windows/desktop/aa365488.aspx) 
            //System.IO.Directory.Delete method ultimately (after removing the files) calls native 
            //RemoveDirectory function which marks the directory as "deleted". That's why we wait until 
            //the directory is actually deleted. For more details see https://stackoverflow.com/a/4245121
            while (Directory.Exists(filePath))
            {
                curIteration += 1;
                if (curIteration > maxIterationToWait)
                    return;
                Thread.Sleep(100);
            }
        }

        public void DeleteFiles(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(filePath);

            foreach (var item in Directory.GetDirectories(filePath))
            {
                DeleteFiles(item);
            }

            try
            {
                DeleteFile(filePath);
            }
            catch (IOException)
            {
                DeleteFile(filePath);
            }
            catch (UnauthorizedAccessException)
            {
                DeleteFile(filePath);
            }
        }
    }
}
