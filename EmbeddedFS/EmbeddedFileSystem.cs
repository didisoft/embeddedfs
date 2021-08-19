using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using LiteDB;

/// <summary>
/// Embedded file system stored in-memory or in a single file
/// </summary>
namespace EmbeddedFS
{
    internal class EmbeddedFileSystem : IDisposable
    {
        private string fileName;
        private LiteDatabase db;
        private ILiteStorage<string> fs;
                
        public static string DirectorySeparator = new String(new char[] { Path.DirectorySeparatorChar });
        public static char DirectorySeparatorChar = Path.DirectorySeparatorChar;
        internal const string ACCESS_TIME = "LastAccessTime";
        internal const string WRITE_TIME = "LastWriteTime";
        internal const string FILE_PATH = "filepath";


        public EmbeddedFileSystem(Stream stream)
        {
            if (stream is MemoryStream)
            { 
                this.fileName = "MEMORY";
            }
            else
            {
                this.fileName = "STREAM";
            }

            this.db = new LiteDatabase(stream);
            InitFS(false);
        }



        public EmbeddedFileSystem(string fileName, string password = null, bool readOnly = false, bool shared = true)
        {
            this.fileName = Path.GetFullPath(fileName);
            if (password == null)
            {
                this.db = new LiteDatabase(String.Format(@"Filename={0}; Connection={1}; ReadOnly={2}", 
                    this.fileName,
                    shared ? "Shared" : "Direct",
                    readOnly));
            }
            else
            {
                this.db = new LiteDatabase(String.Format(@"Filename={0}; Connection={1}; Password={2}; ReadOnly={3}", 
                    this.fileName,
                    shared ? "Shared" : "Direct",
                    password, 
                    readOnly));
            }

            InitFS(readOnly);
        }



        internal void InitFS(bool readOnly)
        {
            this.fs = db.GetStorage<string>("files", "chunks");

            if (!readOnly)
            {
                var col = db.GetCollection<LiteFileInfo<string>>("files");
                col.EnsureIndex("filepath", "$.metadata.filepath", true);

                //
                // Create root folder at start
                //
                if (!FileExists(DirectorySeparator))
                {
                    CreateDirectory(DirectorySeparator);
                }
            }
        }



        internal bool IsDisposed { get; private set; }


        public void Dispose()
        {
            if (!IsDisposed)
            { 
                IsDisposed = true;
                db.Dispose();
            }
        }


        /// <summary>
        /// Gets the name of a drive, such as C:\.
        /// </summary>
        public string Name { get { return fileName;  } }



        #region Internal_Infrastructure

        internal string FixDirectoryName(string name)
        {
            string fullPath = name.Replace("/", DirectorySeparator);
            if (!fullPath.StartsWith(DirectorySeparator))
                fullPath = DirectorySeparator + fullPath;

            if (!fullPath.EndsWith(DirectorySeparator))
                fullPath = fullPath + DirectorySeparator;

            return fullPath;
        }



        internal string FixFileName(string name)
        {
            string fullPath = name.Replace("/", DirectorySeparator);
            if (!fullPath.StartsWith(DirectorySeparator))
                fullPath = DirectorySeparator + fullPath;

            return fullPath;
        }

        internal void Delete(string Id)
        {
            fs.Delete(Id);
        }


        internal string GetDirectoryName(string name)
        {
            return FixDirectoryName(Path.GetFileName(Path.GetDirectoryName(name)));
        }



        internal void CreateDirectory(string name)
        {
            string Id = Guid.NewGuid().ToString();

            BsonDocument metadata = new BsonDocument();
            metadata[EmFileSystemInfo.ACCESS_TIME] = DateTime.Now;
            metadata[WRITE_TIME] = DateTime.Now;
            metadata[FILE_PATH] = name;
            using (FS.OpenWrite(Id, name, metadata)) { }
        }



        internal ILiteStorage<string> FS { get { return fs; }  }

        internal ILiteDatabase DB { get { return db; } }


        internal LiteFileInfo<string> GetFile(string name)
        {
            LiteFileInfo<string> f = this.fs.Find(
                file => file.Metadata[EmFileSystemInfo.FILE_PATH].AsString.Equals(name)).FirstOrDefault();

            return f;
        }



        internal bool FileExists(string name)
        {
            string s = FixFileName(name);
            bool res = this.db.GetCollection("files").Find(Query.EQ("metadata.filepath", s)).Count() > 0;
            return res;
        }



        internal bool DirectoryExists(string name)
        {
            string s = FixDirectoryName(name);
            bool res = this.db.GetCollection("files").Find(Query.EQ("metadata.filepath", s)).Count() > 0;
            return res;
        }



        internal FileAttributes GetAttributes(string name)
        {
            LiteFileInfo<string> file = GetFile(name);
            if (file != null)
            {
                if (file.Metadata[FILE_PATH].AsString.EndsWith(DirectorySeparator))
                {
                    return FileAttributes.Normal | FileAttributes.Directory;
                }
                else
                {
                    return FileAttributes.Normal;
                }
            }

            throw new FileNotFoundException(String.Format("Cannot find file {0}", name));
        }
        #endregion




        internal List<string> GetFiles(string path)
        {
            List<string> filesList = new List<string>();
            IEnumerator<LiteFileInfo<string>> files = 
                        this.fs.Find(file => file.Metadata[EmFileSystemInfo.FILE_PATH].AsString.StartsWith(path))
                                                        .GetEnumerator();

            while (files.MoveNext())
                filesList.Add(files.Current.Metadata[EmFileSystemInfo.FILE_PATH]);

            return filesList;
        }


        internal List<string> DeleteFiles(string path)
        {
            List<string> filesList = new List<string>();
            IEnumerator<LiteFileInfo<string>> files = this.fs.Find(file => file.Metadata[EmFileSystemInfo.FILE_PATH].AsString.StartsWith(path))
                                                        .GetEnumerator();
            while (files.MoveNext())
                this.fs.Delete(files.Current.Id);

            return filesList;
        }



        internal List<EmFileSystemInfo> GetAllFiles()
        {
            List<EmFileSystemInfo> filesList = new List<EmFileSystemInfo>();
            IEnumerator<LiteFileInfo<string>> files = this.fs.FindAll().GetEnumerator();
            while (files.MoveNext())
            {
                if (files.Current.Metadata[EmFileSystemInfo.FILE_PATH].AsString.EndsWith(DirectorySeparator))
                {
                    filesList.Add(new EmDirectoryInfo(new EmDriveInfo(this), files.Current.Metadata[EmFileSystemInfo.FILE_PATH].AsString));
                }
                else
                {
                    filesList.Add(new EmFileInfo(new EmDriveInfo(this), files.Current.Metadata[EmFileSystemInfo.FILE_PATH].AsString));
                }
                   
            }
                

            return filesList;
        }

    }
}
