using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using LiteDB;

namespace EmbeddedFS
{
    /// <summary>
    /// Base class for <see cref="EmDirectoryInfo"/> and <see cref="EmFileInfo"/>
    /// </summary>
    public abstract class EmFileSystemInfo
    {
        internal const string ACCESS_TIME = "LastAccessTime";
        internal const string CREATION_TIME = "CreationTime";
        internal const string FILE_PATH = "filepath";
        internal const string DIRECTORY = "directory";
        protected string name;

        protected LiteFileInfo<string> file;

        private DateTime accessTime = DateTime.Now;
        private DateTime writeTime = DateTime.Now;

        protected FileAttributes attributes = FileAttributes.Normal;

        
        
        /// <summary>
        /// For files, gets the name of the file. For directories, gets the name of the last directory in the hierarchy if a hierarchy exists. 
        /// Otherwise, the Name property gets the name of the directory.
        /// </summary>
        /// <value>
        /// A string that is the name of the parent directory, the name of the last directory in the hierarchy, 
        /// or the name of a file, including the file name extension.
        /// </value>
        /// <remarks>
        /// For a directory, Name returns only the name of the parent directory, such as Dir, not /Dir. For a subdirectory, Name returns only the name of the subdirectory, 
        /// such as Sub1, not /Dir/Sub1.
        /// For a file, Name returns only the file name and file name extension, such as MyFile.txt, not /Dir/Myfile.txt.
        /// </remarks>
        public abstract string Name { get; }


        /// <summary>
        /// Gets a value indicating whether the file or directory exists.
        /// </summary>
        /// <value>true if the file or directory exists; otherwise, false.</value>
        public bool Exists
        {
            get
            {
                return this.file != null;
            }
        }



        /// <summary>
        /// Gets or sets the write time of the current file or directory.
        /// </summary>
        /// <value>The last write date and time of the current <see cref="EmFileSystemInfo"/> object.</value>
        public DateTime LastWriteTime {
            get
            {
                return writeTime;
            }
            set
            {
                writeTime = value;
                // TODO update UploadDate
            }
        }



        /// <summary>
        /// Gets or sets the last write time, in coordinated universal time (UTC), of the current file or directory.
        /// </summary>
        /// <value>The last write date and time in UTC of the current <see cref="EmFileSystemInfo"/> object.</value>
        public DateTime LastWriteTimeUtc {
            get
            {
                return writeTime.ToUniversalTime();
            }
            set
            {
                LastWriteTime = value.ToLocalTime();
            }
        }




        /// <summary>
        /// Gets or sets the last access time, in coordinated universal time (UTC), of the current file or directory.
        /// </summary>
        /// <value>The last access date and time in UTC of the current <see cref="EmFileSystemInfo"/> object.</value>
        public DateTime LastAccessTimeUtc {
            get
            {
                return accessTime.ToUniversalTime();
            }
            set
            {
                LastAccessTime = value.ToLocalTime();
            }
        }



        /// <summary>
        /// Gets or sets the last access time of the current file or directory.
        /// </summary>
        /// <value>The last access date and time of the current <see cref="EmFileSystemInfo"/> object.</value>
        public DateTime LastAccessTime {
            get
            {
                return accessTime;
            }
            set
            {
                accessTime = value;
                this.file.Metadata[ACCESS_TIME] = value;
            }
        }



        /// <summary>
        /// Gets or sets the creation time, in coordinated universal time (UTC), of the current file or directory.
        /// </summary>
        /// <value>The creation date and time in UTC of the current <see cref="EmFileSystemInfo"/> object.</value>
        public DateTime CreationTimeUtc
        {
            get
            {
                return CreationTime.ToUniversalTime();
            }
            set
            {
                CreationTime = value.ToLocalTime();
            }
        }



        /// <summary>
        /// Gets or sets the creation time of the current file or directory.
        /// </summary>
        /// <value>The creation date and time of the current <see cref="EmFileSystemInfo"/> object.</value>
        public DateTime CreationTime { get; set; }



        /// <summary>
        /// Gets the string representing the extension part of the file.
        /// </summary>
        /// <value>A string containing the EmFileSystemInfo extension.</value>
        public string Extension
        {
            get
            {
                return Path.GetExtension(this.Name);
            }
        }




        /// <summary>
        /// Gets the full path of the directory or file.
        /// </summary>
        /// <value>A string containing the full path.</value>
        /// <remarks>
        /// For example, for a file /NewFile.txt, this property returns "NewFile.txt".
        /// </remarks>
        public virtual string FullName { get { return this.name; } }


        /// <summary>
        /// Gets the attributes for the current file or directory.
        /// </summary>
        /// <value><see cref="FileAttributes"/> of the current <see cref="EmFileSystemInfo"/>.</value>
        public FileAttributes Attributes { get { return attributes; } }
    }
}
