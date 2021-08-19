using System;
using System.Collections.Generic;
using System.IO;

using LiteDB;

namespace EmbeddedFS
{
    /// <summary>
    /// Provides properties and instance methods for the creation, copying, deletion, moving, and opening of files located in a <see cref="EmDriveInfo"/>, 
    /// and aids in the creation of Stream objects for reading and writing to the embedded files. This class cannot be inherited.
    /// </summary>
    /// <remarks>
    /// Use the <see cref="EmFileInfo"/> class for typical operations such as copying, moving, renaming, creating, opening, deleting, 
    /// and appending to files.
    ///
    /// If you are performing multiple operations on the same file, it can be more efficient to use <see cref="EmFileInfo"/> instance methods instead of the corresponding static methods of the File class.
    /// 
    /// Many of the <see cref="EmFileInfo"/> methods return other I/O types when you create or open files. You can use these other types to further manipulate a file. For more information, see specific FileInfo members such as Open, OpenRead, OpenText, CreateText, or Create.
    /// </remarks>
    public sealed class EmFileInfo : EmFileSystemInfo
    {
        private string Id;
        private EmDriveInfo drive;
        private EmbeddedFileSystem fs;



        /// <summary>
        /// Creates an instance pointing to a file in an embedded file system
        /// </summary>
        /// <param name="drive">embedded file system</param>
        /// <param name="fileName">absolute or relative path to the file</param>
        /// <example>
        /// This example illustrates how to open a text file from an embedded file system
        /// <code lang="C#">
        /// using System;
        /// using EmbeddedFS;
        /// 
        /// class Program
        /// {
        ///  static void Main(string[] args)
        ///  {
        ///		EmDriveInfo drive = new EmDriveInfo();
        ///
        ///		EmFileInfo file = new EmFileInfo(drive, "myfile.txt");
        ///		using (StreamWriter WriteLiner = file.CreateText())
        ///		{
        ///			WriteLiner.WriteLine("Hello World");
        ///		}
        ///
        ///		Console.WriteLine(file.Name);
        ///		Console.WriteLine(file.FullName);
        ///		Console.WriteLine(EmFile.ReadAllText(drive, "myfile.txt");
        ///  }
        /// }
        /// </code>
        /// </example>
        public EmFileInfo(EmDriveInfo drive, string fileName)
        {
            Utils.CheckParameter(fileName, "fileName");

            if (fileName.EndsWith(@"\") || fileName.EndsWith("/"))
                throw new ArgumentException(String.Format("File name cannot end with \\ or /"));

            this.drive = drive;
            this.fs = drive.EFS;
            this.name = fs.FixFileName(fileName);

            BsonValue tmpVal;
            this.file = fs.GetFile(this.name);
            if (file != null)
            {
                this.Id = this.file.Id;
                this.LastWriteTime = this.file.UploadDate;
                this.LastAccessTime = this.file.Metadata.TryGetValue(ACCESS_TIME, out tmpVal) ?  this.file.Metadata[ACCESS_TIME].AsDateTime : DateTime.Now;
                this.CreationTime = this.file.Metadata.TryGetValue(CREATION_TIME, out tmpVal) ?  this.file.Metadata[CREATION_TIME].AsDateTime : DateTime.Now;
            }
            else
            {
                this.Id = Guid.NewGuid().ToString();
                //
                // FileNotFound on attempt to OpenRead()
                //
            }
        }



        /// <summary>
        /// Returns the <see cref="EmDriveInfo"/> that contains this folder
        /// </summary>
        /// <value>
        /// The <see cref="EmDriveInfo"/> that contains this folder
        /// </value>
        public EmDriveInfo Drive
        {
            get
            {
                return drive;
            }
        }



        /// <summary>
        /// Gets a string representing the name portion of the file path
        /// </summary>
        /// <value>A string representing the name portion of the file path</value>
        /// <example>
        /// This example illustrates how to print the file name
        /// <code lang="C#">
        /// using System;
        /// using EmbeddedFS;
        /// 
        /// class Program
        /// {
        ///  static void Main(string[] args)
        ///  {
        ///     // In-memory embedded file system
        ///		EmDriveInfo drive = new EmDriveInfo();
        ///		
        ///		EmFileInfo file = new EmFileInfo(drive, "myfile.txt");
        ///		Console.WriteLine(file.Name);
        ///  }
        /// }
        /// </code>
        /// </example>
        public override string Name
        {
            get
            {
                return Path.GetFileName(this.name);
            }
        }


        /// <summary>
        /// Gets an instance of the parent directory.
        /// </summary>
        /// <value>A <see cref="EmDirectoryInfo"/> object representing the parent directory of this file.</value>
        /// <example>
        /// This example illustrates how to get the parent directory of a file
        /// <code lang="C#">
        /// using System;
        /// using EmbeddedFS;
        /// 
        /// class Program
        /// {
        ///  static void Main(string[] args)
        ///  {
        ///     // In-memory embedded file system
        ///		EmDriveInfo drive = new EmDriveInfo();
        ///		
        ///		EmFileInfo file = new EmFileInfo(drive, "myfile.txt");
        ///		EmDirectoryInfo dir = file.Directory;
        ///		Console.WriteLine(dir.FullName);
        ///  }
        /// }
        /// </code>
        /// </example>
        public EmDirectoryInfo Directory
        {
            get
            {
                return new EmDirectoryInfo(this.drive, Path.GetDirectoryName(this.name));
            }
        }


        /// <summary>
        /// Gets a string representing the directory's full path.
        /// </summary>
        /// <value>A string representing the directory's full path.</value>
        /// <example>
        /// This example illustrates how to retrieves the full path of a file
        /// <code lang="C#">
        /// using System;
        /// using EmbeddedFS;
        /// 
        /// class Program
        /// {
        ///  static void Main(string[] args)
        ///  {
        ///     // In-memory embedded file system
        ///		EmDriveInfo drive = new EmDriveInfo();
        ///		
        ///		EmFileInfo file = new EmFileInfo(drive, "myfile.txt");
        ///		Console.WriteLine(file.DirectoryName);
        ///  }
        /// }
        /// </code>
        /// </example>        
        public string DirectoryName
        {
            get
            {
                return Path.GetDirectoryName(this.name);
            }
        }



        /// <summary>
        /// Gets the size, in bytes, of the current file.
        /// </summary>
        /// <value>The size of the current file in bytes.</value>
        /// <exception cref="FileNotFoundException">The file does not exist.</exception>
        /// <example>
        /// The following example displays the size of the specified files.
        /// <code lang="C#">
        /// using System;
        /// using EmbeddedFS;
        /// 
        /// class Program
        /// {
        ///  static void Main(string[] args)
        ///  {
        ///		EmDriveInfo drive = new EmDriveInfo(@"c:\File\drive.fs");
        ///		
        ///		EmFileInfo file = new EmFileInfo(drive, "myfile.txt");
        ///		Console.WriteLine(file.Length);
        ///  }
        /// }
        /// </code>
        /// </example>        
        public long Length
        {
            get
            {
                CheckExists();
                return this.file.Length;
            }
        }



        /// <summary>
        /// Permanently deletes an embedded file.
        /// </summary>
        /// <remarks>If the file does not exist, this method does nothing.</remarks>
        /// <exception cref="IOException">The target file is open </exception>
        /// <example>
        /// The following example creates, closes, and deletes a file.
        /// <code lang="C#">
        /// using System;
        /// using System.IO;
        /// using EmbeddedFS;
        /// 
        /// class Program
        /// {
        ///  static void Main(string[] args)
        ///  {
        ///		EmDriveInfo drive = new EmDriveInfo(@"c:\File\drive.fs");
        ///		
        ///		EmFileInfo file = new EmFileInfo(drive, "myfile.txt");
        ///		Stream fs = file.Create();
        ///		fs.Close();
        ///		file.Delete();
        ///  }
        /// }
        /// </code>
        /// </example>        
        public void Delete()
        {
            fs.Delete(this.Id);
            this.file = null;
        }



        /// <summary>
        /// Creates a new embedded file.
        /// </summary>
        /// <remarks>Full write access to new files is granted to all users.</remarks>
        /// <returns>a <see cref="Stream"/> that writes to the embedded file.</returns>
        /// <example>
        /// The following example creates a file and adds some text to it
        /// <code lang="C#">
        /// using System;
        /// using System.IO;
        /// using System.Text;
        /// using EmbeddedFS;
        /// 
        /// class Program
        /// {
        ///  static void Main(string[] args)
        ///  {
        ///		EmDriveInfo drive = new EmDriveInfo(@"c:\File\drive.fs");
        ///		
        ///		EmFileInfo file = new EmFileInfo(drive, "myfile.txt");
        ///		using (Stream fs = file.Create())
        ///		{
        ///		 Byte[] info =
        ///		 new UTF8Encoding(true).GetBytes("This is some text in the file.");
        ///		
        ///		 //Add some information to the file.
        ///		 fs.Write(info, 0, info.Length);
        ///		}
        ///  }
        /// }
        /// </code>
        /// </example>        
        public Stream Create()
        {
            return this.OpenWrite();
        }


        /// <summary>
        /// Creates a <see cref="StreamWriter"/> that writes a new embedded text file.
        /// </summary>
        /// <returns>A <see cref="StreamWriter"/> that writes a new embedded text file.</returns>
        /// <example>
        /// The following example demonstrates the CreateText method
        /// <code lang="C#">
        /// using System;
        /// using System.IO;
        /// using EmbeddedFS;
        /// 
        /// class Program
        /// {
        ///  static void Main(string[] args)
        ///  {
        ///		EmDriveInfo drive = new EmDriveInfo(@"c:\File\drive.fs");
        ///		
        ///		EmFileInfo file = new EmFileInfo(drive, "myfile.txt");
        ///		using (StreamWriter fs = file.CreateText())
        ///		{
        ///		 fs.WriteLine("This is some text in the file.");
        ///		}
        ///  }
        /// }
        /// </code>
        /// </example>        
        public System.IO.StreamWriter CreateText()
        {
            return new StreamWriter(this.OpenWrite());
        }



        /// <summary>
        /// Creates a read-only <see cref="FileStream"/>.
        /// </summary>
        /// <returns>A <see cref="Stream"/> for reading</returns>
        /// <exception cref="IOException">The file is already open.</exception>
        /// <exception cref="FileNotFoundException">The file is not found.</exception>
        /// <example>
        /// The following example opens a file as read-only and reads from it.
        /// <code lang="C#">
        /// using System;
        /// using System.Text;
        /// using System.IO;
        /// using EmbeddedFS;
        /// 
        /// class Program
        /// {
        ///  static void Main(string[] args)
        ///  {
        ///		EmDriveInfo drive = new EmDriveInfo(@"c:\File\drive.fs");
        ///		
        ///		EmFileInfo file = new EmFileInfo(drive, "myfile.txt");
        ///		using (Stream fs = file.OpenRead())
        ///		{
        ///		 byte[] b = new byte[1024];
        ///		 UTF8Encoding temp = new UTF8Encoding(true)
        ///		 while (fs.Read(b,0,b.Length) > 0)
        ///		 {
        ///		    Console.WriteLine(temp.GetString(b));
        ///		 }
        ///		}
        ///  }
        /// }
        /// </code>
        /// </example>        
        public Stream OpenRead()
        {
            CheckExists();

            LastAccessTime = DateTime.Now;
            return this.file.OpenRead();
        }



        /// <summary>
        /// Creates a <see cref="StreamReader"/> with UTF8 encoding that reads from an existing text file.
        /// </summary>
        /// <returns><see cref="StreamReader"/> with UTF8 encoding that reads from this file</returns>
        /// <exception cref="IOException">The file is already open.</exception>
        /// <exception cref="FileNotFoundException">The file is not found.</exception>
        /// <example>
        /// The following example opens a file as read-only and reads from it.
        /// <code lang="C#">
        /// using System;
        /// using System.IO;
        /// using EmbeddedFS;
        /// 
        /// class Program
        /// {
        ///  static void Main(string[] args)
        ///  {
        ///		EmDriveInfo drive = new EmDriveInfo(@"c:\File\drive.fs");
        ///		
        ///		EmFileInfo file = new EmFileInfo(drive, "myfile.txt");
        ///		using (StreamReader reader = file.OpenText())
        ///		{
        ///		 string s = "";
        ///		 while ((s = reader.ReadLine()) != null)
        ///		 {
        ///		    Console.WriteLine(s);
        ///		 }
        ///		}
        ///  }
        /// }
        /// </code>
        /// </example>        
        public StreamReader OpenText()
        {
            return new StreamReader(this.OpenRead());
        }



        /// <summary>
        /// Creates a <see cref="StreamWriter"/> that appends text to the file represented by this instance of the <see cref="EmbeddedFS.EmFileInfo"/>.
        /// </summary>
        /// <returns>A new <see cref="StreamWriter"/></returns>
        /// <example>
        /// The following example demonstrates the AppendText method
        /// <code lang="C#">
        /// using System;
        /// using System.IO;
        /// using EmbeddedFS;
        /// 
        /// class Program
        /// {
        ///  static void Main(string[] args)
        ///  {
        ///		EmDriveInfo drive = new EmDriveInfo(@"c:\File\drive.fs");
        ///		
        ///		EmFileInfo file = new EmFileInfo(drive, "myfile.txt");
        ///		using (StreamWriter fs = file.AppendText())
        ///		{
        ///		 fs.Write("This is an extra line.");
        ///		}
        ///  }
        /// }
        /// </code>
        /// </example>        
        public StreamWriter AppendText()
        {
            if (!Exists)
            {
                BsonDocument metadata = new BsonDocument();
                metadata[ACCESS_TIME] = DateTime.Now;
                metadata[CREATION_TIME] = DateTime.Now;
                metadata[FILE_PATH] = this.name;
                using (fs.FS.OpenWrite(this.Id, this.Name, metadata)) { }
                this.file = fs.GetFile(this.name);
            }

            Stream writeStream = this.file.OpenAppend();
            return new StreamWriter(writeStream);
        }



        internal Stream OpenAppend()
        {
            if (!Exists)
            {
                BsonDocument metadata = new BsonDocument();
                metadata[ACCESS_TIME] = DateTime.Now;
                metadata[CREATION_TIME] = DateTime.Now;
                metadata[FILE_PATH] = this.name;
                using (fs.FS.OpenWrite(this.Id, this.Name, metadata)) { }
                this.file = fs.GetFile(this.name);
            }

            Stream writeStream = this.file.OpenAppend();
            return writeStream;
        }



        /// <summary>
        /// Creates a write-only <see cref="Stream"/>.
        /// </summary>
        /// <remarks>The OpenWrite method opens a file if one already exists for the file path, or creates a new file if one does not exist. 
        /// For an existing file, it does not append the new text to the existing text. Instead, it overwrites the existing characters with the new characters. 
        /// If you overwrite a longer string (such as "This is a test of the OpenWrite method") with a shorter string (like "Second run"), the file will contain a mix of the strings ("Second runtest of the OpenWrite method").
        /// </remarks>
        /// <returns>A write-only <see cref="Stream"/> for this embedded file</returns>
        /// <exception cref="UnauthorizedAccessException">The path specified when creating an instance of the FileInfo object is read-only or is a directory.</exception>
        /// <exception cref="DirectoryNotFoundException">The path specified when creating an instance of the <see cref="EmFileInfo"/> object is invalid, such as being on an unmapped drive.</exception>
        /// <example>
        /// The following example creates a file and adds some text to it
        /// <code lang="C#">
        /// using System;
        /// using System.IO;
        /// using System.Text;
        /// using EmbeddedFS;
        /// 
        /// class Program
        /// {
        ///  static void Main(string[] args)
        ///  {
        ///		EmDriveInfo drive = new EmDriveInfo(@"c:\File\drive.fs");
        ///		
        ///		EmFileInfo file = new EmFileInfo(drive, "myfile.txt");
        ///		using (Stream fs = file.OpenWrite())
        ///		{
        ///		 Byte[] info =
        ///		 new UTF8Encoding(true).GetBytes("This is some text in the file.");
        ///		
        ///		 //Add some information to the file.
        ///		 fs.Write(info, 0, info.Length);
        ///		}
        ///  }
        /// }
        /// </code>
        /// </example>        
        public Stream OpenWrite()
        {
            if (!this.fs.DirectoryExists(this.DirectoryName))
                throw new DirectoryNotFoundException(String.Format("Could not find a part of the path {0}", this.name));

            // If directory with same name exists
            if (this.fs.FileExists(this.name + EmbeddedFileSystem.DirectorySeparator))
                throw new System.UnauthorizedAccessException(String.Format("Access to the path '{0}' is denied.", this.name));

            // Create file if not exists
            if (!this.fs.FileExists(this.name)) {
                BsonDocument metadata = new BsonDocument();
                metadata[ACCESS_TIME] = DateTime.Now;
                metadata[CREATION_TIME] = DateTime.Now;
                metadata[FILE_PATH] = this.name;
                using (fs.FS.OpenWrite(this.Id, this.Name, metadata)) { }
                this.file = fs.GetFile(this.name);
            }
            else if (this.file == null)
            {
                this.file = fs.GetFile(this.name);
            }

            LastAccessTime = DateTime.Now;
            LastWriteTime = DateTime.Now;

            return this.file.OpenWrite();
        }



        /// <summary>
        /// Replaces the contents of a specified file with the file described by the current FileInfo object, deleting the original file, and creating a backup of the replaced file.
        /// </summary>
        /// <exception cref="ArgumentNullException">The destFileName parameter is null.</exception>
        /// <exception cref="ArgumentException">The destFileName parameter is empty.</exception>
        /// <exception cref="FileNotFoundException">The file described by the current or destination <see cref="EmFileInfo"/> object could not be found.</exception>
        /// <remarks>Use the Replace methods when you need to quickly replace a file with the contents of the file described by the current <see cref="EmFileInfo"/> object.</remarks>
        /// <param name="destinationFileName">The name of a file to replace with the current file.</param>
        /// <param name="destinationBackupFileName">The name of a file with which to create a backup of the file described by the destFileName parameter</param>
        /// <returns>A <see cref="System.IO.FileInfo"/> object that encapsulates information about the file described by the destFileName parameter.</returns>
        /// <example>
        /// The following example replaces the contents of a remote file with this embedded file
        /// <code lang="C#">
        /// using System;
        /// using System.IO;
        /// using EmbeddedFS;
        /// 
        /// class Program
        /// {
        ///  static void Main(string[] args)
        ///  {
        ///		EmDriveInfo drive = new EmDriveInfo(@"c:\File\drive.fs");
        ///		
        ///		EmFileInfo file = new EmFileInfo(drive, "myfile.txt");
        ///		file.Replace(@"c:\Temp\afile.txt");
        ///  }
        /// }
        /// </code>
        /// </example>        
        public System.IO.FileInfo Replace(string destinationFileName, string destinationBackupFileName = null)
        {
            Utils.CheckParameter(destinationFileName, "destinationFileName");

            CheckExists();
            if (!File.Exists(destinationFileName))
                throw new FileNotFoundException(destinationFileName);

            if (destinationBackupFileName != null && File.Exists(destinationFileName)) 
                File.Copy(destinationFileName, destinationBackupFileName);

            this.file.SaveAs(destinationFileName, true);

            return new System.IO.FileInfo(destinationFileName);
        }



        /// <summary>
        /// Creates a write-only or read-only <see cref="Stream"/>.
        /// </summary>
        /// <returns>A file opened in the specified mode, with read/write access and unshared.</returns>
        /// <exception cref="FileNotFoundException">The file described by the current <see cref="EmFileInfo"/> object could not be found.</exception>
        /// <example>
        /// The following example creates a file and adds some text to it
        /// <code lang="C#">
        /// using System;
        /// using System.IO;
        /// using System.Text;
        /// using EmbeddedFS;
        /// 
        /// class Program
        /// {
        ///  static void Main(string[] args)
        ///  {
        ///		EmDriveInfo drive = new EmDriveInfo(@"c:\File\drive.fs");
        ///		
        ///		EmFileInfo file = new EmFileInfo(drive, "myfile.txt");
        ///		using (Stream fs = file.Open(FileMode.Append, FileAccess.Write, FileShare.Write))
        ///		{
        ///		 Byte[] info =
        ///		 new UTF8Encoding(true).GetBytes("This is some text in the file.");
        ///		
        ///		 //Add some information to the file.
        ///		 fs.Write(info, 0, info.Length);
        ///		}
        ///  }
        /// }
        /// </code>
        /// </example>        
        public Stream Open(FileMode mode, FileAccess access, FileShare share)
        {
            CheckExists(); 

            if (FileAccess.Read.Equals(access))
            {
                LiteFileStream<string> stream = this.file.OpenRead();
                return stream;
            }

            switch (mode)
            {
                case FileMode.Append:
                    {
                        Stream stream = this.OpenAppend();
                        return stream;
                    }
                case FileMode.Create:
                    {
                        Stream stream = this.OpenWrite();
                        return stream;
                    }
                case FileMode.CreateNew:
                    {
                        Stream stream = this.OpenWrite();
                        return stream;
                    }
                case FileMode.Open:
                    {
                        return OpenWrite();
                    }
                case FileMode.OpenOrCreate:
                    {
                        return OpenWrite();
                    }
                case FileMode.Truncate:
                    {
                        return OpenWrite();
                    }
                default: return OpenWrite();
            }
        }

        /// <summary>
        /// Copies a file from the Embedded <see cref="EmDriveInfo"/> to a new outside file, allowing the overwriting of an existing file.
        /// </summary>
        /// <param name="destFileName">The name of the new file to copy to.</param>
        /// <returns>A new file with a fully qualified path.</returns>
        /// <exception cref="ArgumentNullException">destFileName is null.</exception>
        /// <exception cref="ArgumentException">destFileName is empty, contains only white spaces, or contains invalid characters.</exception>
        /// <exception cref="IOException">An error occurs, or the destination file already exists.</exception>
        /// <exception cref="FileNotFoundException">The file described by the current or destination <see cref="EmFileInfo"/> object could not be found.</exception>
        /// <example>
        /// The following example copies the contents this embedded file to a remote file
        /// <code lang="C#">
        /// using System;
        /// using System.IO;
        /// using EmbeddedFS;
        /// 
        /// class Program
        /// {
        ///  static void Main(string[] args)
        ///  {
        ///		EmDriveInfo drive = new EmDriveInfo(@"c:\File\drive.fs");
        ///		
        ///		EmFileInfo file = new EmFileInfo(drive, "myfile.txt");
        ///		file.CopyTo(@"c:\Temp\afile.txt");
        ///  }
        /// }
        /// </code>
        /// </example>        
        public System.IO.FileInfo CopyTo(String destFileName)
        {
            return CopyTo(destFileName, false);
        }



        /// <summary>
        /// Copies a file from the Embedded <see cref="EmDriveInfo"/> to a new outside file, allowing the overwriting of an existing file.
        /// </summary>
        /// <param name="destFileName">The name of the new file to copy to.</param>
        /// <param name="overwrite">true to allow an existing file to be overwritten; otherwise, false.</param>
        /// <returns>A new file with a fully qualified path.</returns>
        /// <exception cref="ArgumentNullException">destFileName is null.</exception>
        /// <exception cref="ArgumentException">destFileName is empty, contains only white spaces, or contains invalid characters.</exception>
        /// <exception cref="IOException">An error occurs, or the destination file already exists.</exception>
        /// <exception cref="FileNotFoundException">The file described by the current or destination <see cref="EmFileInfo"/> object could not be found.</exception>
        /// <example>
        /// The following example copies the contents this embedded file to a remote file
        /// <code lang="C#">
        /// using System;
        /// using System.IO;
        /// using EmbeddedFS;
        /// 
        /// class Program
        /// {
        ///  static void Main(string[] args)
        ///  {
        ///		EmDriveInfo drive = new EmDriveInfo(@"c:\File\drive.fs");
        ///		
        ///		EmFileInfo file = new EmFileInfo(drive, "myfile.txt");
        ///		bool overwrite = true;
        ///		file.CopyTo(@"c:\Temp\afile.txt", overwrite);
        ///  }
        /// }
        /// </code>
        /// </example>        
        public System.IO.FileInfo CopyTo(String destFileName, bool overwrite)
        {
            Utils.CheckParameter(destFileName, "destFileName");

            CheckExists();
            if (File.Exists(destFileName) && !overwrite)
                throw new IOException("Cannot overwrite existing file!");

            file.SaveAs(destFileName);

            return new System.IO.FileInfo(destFileName);
        }



        /// <summary>
        /// Moves a file from the Embedded <see cref="EmDriveInfo"/> to a new file, allowing the overwriting of an existing file.
        /// </summary>
        /// <param name="destFileName">The path to move the file to, which can specify a different file name.</param>
        /// <exception cref="ArgumentNullException">destFileName is null.</exception>
        /// <exception cref="ArgumentException">destFileName is empty, contains only white spaces, or contains invalid characters.</exception>
        /// <exception cref="IOException">An error occurs, or the destination file already exists.</exception>
        /// <exception cref="FileNotFoundException">The file described by the current <see cref="EmFileInfo"/> object could not be found.</exception>
        /// <exception cref="DirectoryNotFoundException">The destination directory doesn't exist</exception>
        /// <example>
        /// The following example moves an embedded file to a new location
        /// <code lang="C#">
        /// using System;
        /// using System.IO;
        /// using EmbeddedFS;
        /// 
        /// class Program
        /// {
        ///  static void Main(string[] args)
        ///  {
        ///		EmDriveInfo drive = new EmDriveInfo(@"c:\File\drive.fs");
        ///		
        ///		EmFileInfo file = new EmFileInfo(drive, "myfile.txt");
        ///		bool overwrite = true;
        ///		file.MoveTo(@"\Temp\afile.txt");
        ///  }
        /// }
        /// </code>
        /// </example>        
        public void MoveTo(String destFileName)
        {
            Utils.CheckParameter(destFileName, "destFileName");
            CheckExists();

            string dir = this.fs.FixDirectoryName(destFileName);
            if (!this.fs.FileExists(dir))
                throw new DirectoryNotFoundException(String.Format("Could not find a part of the path {0}", destFileName));

            string destFile = this.fs.FixFileName(destFileName);

            this.file.Metadata[EmFileSystemInfo.FILE_PATH] = destFile;
            this.fs.DB.GetCollection("files").Update(BsonMapper.Global.ToDocument(this.file));

            this.name = destFile;
            this.file = this.fs.GetFile(destFile);
        }



        /// <summary>
        /// Copies a file from the Embedded <see cref="EmDriveInfo"/> to a new file, allowing the overwriting of an existing file.
        /// </summary>
        /// <param name="destFileName">The name of the new file to copy to.</param>
        /// <param name="overwrite">true to allow an existing file to be overwritten; otherwise, false.</param>
        /// <returns>A new file with a fully qualified path.</returns>
        /// <exception cref="ArgumentNullException">destFileName is null.</exception>
        /// <exception cref="ArgumentException">destFileName is empty, contains only white spaces, or contains invalid characters.</exception>
        /// <exception cref="IOException">An error occurs, or the destination file already exists.</exception>
        /// <exception cref="FileNotFoundException">The file described by the current or destination <see cref="EmFileInfo"/> object could not be found.</exception>
        /// <example>
        /// The following example copies the contents this embedded file to a new location
        /// <code lang="C#">
        /// using System;
        /// using System.IO;
        /// using EmbeddedFS;
        /// 
        /// class Program
        /// {
        ///  static void Main(string[] args)
        ///  {
        ///		EmDriveInfo drive = new EmDriveInfo(@"c:\File\drive.fs");
        ///		
        ///		EmFileInfo file = new EmFileInfo(drive, "myfile.txt");
        ///		file.CopyToInternal(@"\Temp\afile.txt");
        ///  }
        /// }
        /// </code>
        /// </example>        
        public EmbeddedFS.EmFileInfo CopyToInternal(String destFileName)
        {
            return CopyToInternal(destFileName, false);
        }



        /// <summary>
        /// Copies a file from the Embedded <see cref="EmDriveInfo"/> to a new file, allowing the overwriting of an existing file.
        /// </summary>
        /// <param name="destFileName">The name of the new file to copy to.</param>
        /// <param name="overwrite">true to allow an existing file to be overwritten; otherwise, false.</param>
        /// <returns>A new file with a fully qualified path.</returns>
        /// <exception cref="ArgumentNullException">destFileName is null.</exception>
        /// <exception cref="ArgumentException">destFileName is empty, contains only white spaces, or contains invalid characters.</exception>
        /// <exception cref="IOException">An error occurs, or the destination file already exists.</exception>
        /// <exception cref="FileNotFoundException">The file described by the current or destination <see cref="EmFileInfo"/> object could not be found.</exception>
        /// <example>
        /// The following example copies the contents this embedded file to a new location
        /// <code lang="C#">
        /// using System;
        /// using System.IO;
        /// using EmbeddedFS;
        /// 
        /// class Program
        /// {
        ///  static void Main(string[] args)
        ///  {
        ///		EmDriveInfo drive = new EmDriveInfo(@"c:\File\drive.fs");
        ///		
        ///		EmFileInfo file = new EmFileInfo(drive, "myfile.txt");
        ///		bool overwrite = true;
        ///		file.CopyToInternal(@"c:\Temp\afile.txt", overwrite);
        ///  }
        /// }
        /// </code>
        /// </example>        
        public EmbeddedFS.EmFileInfo CopyToInternal(String destFileName, bool overwrite)
        {
            Utils.CheckParameter(destFileName, "destFileName");

            if (this.fs.FileExists(destFileName) && !overwrite)
                throw new IOException("Cannot overwrite existing file!");

            string dir = this.fs.FixDirectoryName(destFileName);
            if (!this.fs.FileExists(dir))
                throw new DirectoryNotFoundException(String.Format("Could not find a part of the path {0}", destFileName));

            EmFileInfo newFile = new EmFileInfo(this.drive, destFileName);
            using (Stream writeStream = newFile.OpenWrite())
            { 
                file.CopyTo(writeStream);
            }

            return newFile;
        }



        #region Private_helpers

        private void CheckExists()
        {
            if (this.file == null)
            {
                this.file = fs.GetFile(this.name);
                if (this.file == null)
                    throw new FileNotFoundException(String.Format("Could not find file {0}", this.name));
            }
        }

        #endregion

    }
}
