using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System.IO;

using LiteDB;

namespace EmbeddedFS
{
    /// <summary>
    /// Provides static methods for the creation, copying, deletion, moving, and opening of a single file
    /// </summary>
    public static class EmFile
    {
        /// <summary>
        /// Appends lines to a file, and then closes the file. If the specified file does not exist, this method creates a file, writes the specified lines to the file, and then closes the file.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">The file to append the lines to. The file is created if it doesn't already exist.</param>
        /// <param name="contents">The lines to append to the file.</param>
        /// <example>
        /// The following example demonstrates the AppendAllLines method
        /// <code lang="C#">
        /// using System;
        /// using System.IO;
        /// using EmbeddedFS;
        /// 
        /// class Program
        /// {
        ///  static void Main(string[] args)
        ///  {
        ///     IEnumerable&lt;string&gt; contents = ...
        ///     
        ///		EmDriveInfo drive = new EmDriveInfo(@"c:\File\drive.fs");
        ///		
        ///		EmFile.AppendAllLines(drive, "myfile.txt", contents);
        ///  }
        /// }
        /// </code>
        /// </example>        
        public static void AppendAllLines(EmDriveInfo drive, string path, System.Collections.Generic.IEnumerable<string> contents)
        {
            Utils.CheckParameter(path, "path");

            EmFileInfo file = new EmFileInfo(drive, path);
            using (StreamWriter writer = file.AppendText())
            {
                IEnumerator<string> enumerator = contents.GetEnumerator();
                while (enumerator.MoveNext())
                    writer.WriteLine(enumerator.Current);
            }
        }



        /// <summary>
        /// Appends lines to a file, and then closes the file. If the specified file does not exist, this method creates a file, writes the specified lines to the file, and then closes the file.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">The file to append the lines to. The file is created if it doesn't already exist.</param>
        /// <param name="contents">The lines to append to the file.</param>
        /// <param name="enc">Encoding of the <see cref="StreamWriter"/> that will append the data</param>
        /// <example>
        /// The following example demonstrates the AppendAllLines method
        /// <code lang="C#">
        /// using System;
        /// using System.IO;
        /// using EmbeddedFS;
        /// 
        /// class Program
        /// {
        ///  static void Main(string[] args)
        ///  {
        ///     IEnumerable&lt;string&gt; contents = ...
        ///     
        ///		EmDriveInfo drive = new EmDriveInfo(@"c:\File\drive.fs");
        ///		
        ///		EmFile.AppendAllLines(drive, "myfile.txt", contents, Encoding.UTF8.UTF8Encoding);
        ///  }
        /// }
        /// </code>
        /// </example>        
        public static void AppendAllLines(EmDriveInfo drive, string path, System.Collections.Generic.IEnumerable<string> contents, Encoding enc)
        {
            Utils.CheckParameter(path, "path");

            EmFileInfo file = new EmFileInfo(drive, path);
            using (Stream stream = file.OpenAppend())
            using (StreamWriter writer = new StreamWriter(stream, enc))
            {
                IEnumerator<string> enumerator = contents.GetEnumerator();
                while (enumerator.MoveNext())
                    writer.WriteLine(enumerator.Current);
            }
        }


        /// <summary>
        /// Asynchronously appends lines to a file, and then closes the file. If the specified file does not exist, this method creates a file, writes the specified lines to the file, and then closes the file.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">The file to append the lines to. The file is created if it doesn't already exist.</param>
        /// <param name="contents">The lines to append to the file.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <returns>A task that represents the asynchronous append operation.</returns>
        /// <example>
        /// The following example demonstrates the AppendAllLinesAsync method
        /// <code lang="C#">
        /// using System;
        /// using System.IO;
        /// using EmbeddedFS;
        /// 
        /// class Program
        /// {
        ///  static void Main(string[] args)
        ///  {
        ///     IEnumerable&lt;string&gt; contents = ...
        ///     
        ///		EmDriveInfo drive = new EmDriveInfo(@"c:\File\drive.fs");
        ///		
        ///		await EmFile.AppendAllLinesAsync(drive, "myfile.txt", contents, Encoding.UTF8.UTF8Encoding);
        ///  }
        /// }
        /// </code>
        /// </example>        
        public static System.Threading.Tasks.Task AppendAllLinesAsync(EmDriveInfo drive, string path, System.Collections.Generic.IEnumerable<string> contents, System.Threading.CancellationToken cancellationToken)
        {
            return new Task(() => AppendAllLines(drive, path, contents), cancellationToken);
        }



        /// <summary>
        /// Opens a file, appends the specified string to the file, and then closes the file. If the file does not exist, this method creates a file, writes the specified string to the file, then closes the file.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">The file to append the lines to. The file is created if it doesn't already exist.</param>
        /// <param name="contents">The text to append to the file.</param>
        /// <example>
        /// The following example demonstrates the AppendAllText method
        /// <code lang="C#">
        /// using System;
        /// using System.IO;
        /// using EmbeddedFS;
        /// 
        /// class Program
        /// {
        ///  static void Main(string[] args)
        ///  {
        ///     IEnumerable&lt;string&gt; contents = ...
        ///     
        ///		EmDriveInfo drive = new EmDriveInfo(@"c:\File\drive.fs");
        ///		
        ///		EmFile.AppendAllText(drive, "myfile.txt", "Hello world");
        ///  }
        /// }
        /// </code>
        /// </example>        
        public static void AppendAllText(EmDriveInfo drive, string path, string contents)
        {
            Utils.CheckParameter(path, "path");

            EmFileInfo file = new EmFileInfo(drive, path);
            using (StreamWriter writer = file.AppendText())
            {
                writer.Write(contents);
            }
        }



        /// <summary>
        /// Opens a file, appends the specified string to the file, and then closes the file. If the file does not exist, this method creates a file, writes the specified string to the file, then closes the file.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">The file to append the lines to. The file is created if it doesn't already exist.</param>
        /// <param name="contents">The text to append to the file.</param>
        /// <param name="encoding">Encoding of the <see cref="StreamWriter"/> that will append the data</param>
        /// <example>
        /// The following example demonstrates the AppendAllText method
        /// <code lang="C#">
        /// using System;
        /// using System.IO;
        /// using EmbeddedFS;
        /// 
        /// class Program
        /// {
        ///  static void Main(string[] args)
        ///  {
        ///     IEnumerable&lt;string&gt; contents = ...
        ///     
        ///		EmDriveInfo drive = new EmDriveInfo(@"c:\File\drive.fs");
        ///		
        ///		EmFile.AppendAllText(drive, "myfile.txt", "Hello world", Encoding.UTF8.UTF8Encoding);
        ///  }
        /// }
        /// </code>
        /// </example>        
        public static void AppendAllText(EmDriveInfo drive, string path, string contents, Encoding encoding)
        {
            Utils.CheckParameter(path, "path");

            EmFileInfo file = new EmFileInfo(drive, path);
            using (Stream stream = file.OpenAppend())
            using (StreamWriter writer = new StreamWriter(stream, encoding))
            {
                writer.Write(contents);
            }
        }



        /// <summary>
        /// Asynchronously opens a file or creates a file if it does not already exist, appends the specified string to the file, and then closes the file.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">The file to append the lines to. The file is created if it doesn't already exist.</param>
        /// <param name="contents">The text to append to the file.</param>
        /// <returns>A task that represents the asynchronous append operation.</returns>
        /// <example>
        /// The following example demonstrates the AppendAllTextAsync method
        /// <code lang="C#">
        /// using System;
        /// using System.IO;
        /// using EmbeddedFS;
        /// 
        /// class Program
        /// {
        ///  static void Main(string[] args)
        ///  {
        ///     IEnumerable&lt;string&gt; contents = ...
        ///     
        ///		EmDriveInfo drive = new EmDriveInfo(@"c:\File\drive.fs");
        ///		
        ///		await EmFile.AppendAllTextAsync(drive, "myfile.txt", "Hello world", Encoding.UTF8.UTF8Encoding);
        ///  }
        /// }
        /// </code>
        /// </example>        
        public static System.Threading.Tasks.Task AppendAllTextAsync(EmDriveInfo drive, string path, string contents)
        {
            return AppendAllTextAsync(drive, path, contents, System.Threading.CancellationToken.None);
        }



        /// <summary>
        /// Asynchronously opens a file or creates a file if it does not already exist, appends the specified string to the file, and then closes the file.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">The file to append the lines to. The file is created if it doesn't already exist.</param>
        /// <param name="contents">The text to append to the file.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <returns>A task that represents the asynchronous append operation.</returns>
        /// <example>
        /// The following example demonstrates the AppendAllTextAsync method
        /// <code lang="C#">
        /// using System;
        /// using System.IO;
        /// using EmbeddedFS;
        /// 
        /// class Program
        /// {
        ///  static void Main(string[] args)
        ///  {
        ///     IEnumerable&lt;string&gt; contents = ...
        ///     
        ///		EmDriveInfo drive = new EmDriveInfo(@"c:\File\drive.fs");
        ///		
        ///		await EmFile.AppendAllTextAsync(drive, "myfile.txt", "Hello world", Encoding.UTF8.UTF8Encoding);
        ///  }
        /// }
        /// </code>
        /// </example>        
        public static System.Threading.Tasks.Task AppendAllTextAsync(EmDriveInfo drive, string path, string contents, System.Threading.CancellationToken cancellationToken)
        {
             return new Task(() => AppendAllText(drive, path, contents), cancellationToken);
        }



        /// <summary>
        /// Creates a <see cref="StreamWriter"/> that appends UTF-8 encoded text to an existing file, or to a new file if the specified file does not exist.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">The file to append the lines to. The file is created if it doesn't already exist.</param>
        /// <returns>A stream writer that appends UTF-8 encoded text to the specified file or to a new file.</returns>
        /// <example>
        /// The following example demonstrates the AppendAllTextAsync method
        /// <code lang="C#">
        /// using System;
        /// using System.IO;
        /// using EmbeddedFS;
        /// 
        /// class Program
        /// {
        ///  static void Main(string[] args)
        ///  {
        ///     IEnumerable&lt;string&gt; contents = ...
        ///     
        ///		EmDriveInfo drive = new EmDriveInfo(@"c:\File\drive.fs");
        ///		
        ///		using (StreamWriter fs = EmFile.AppendText(drive, "myfile.txt"))
        ///		{
        ///		 fs.Write("This is an extra line.");
        ///		}
        ///  }
        /// }
        /// </code>
        /// </example>        
        public static System.IO.StreamWriter AppendText(EmDriveInfo drive, string path)
        {
            Utils.CheckParameter(path, "path");

            return new StreamWriter(new EmFileInfo(drive, path).OpenAppend());
        }



        /// <summary>
        /// Copies an existing file from an <see cref="EmDriveInfo"/> to a new file also in the <see cref="EmDriveInfo"/>.
        /// </summary>
        /// <param name="drive">Embedded file system contining the source file</param>
        /// <param name="sourceFileName">The file to copy.</param>
        /// <param name="destFileName">The name of the destination file. This cannot be a directory or an existing file.</param>
        /// <param name="overwrite">Optional. If true the target file will be overwriten.</param>
        /// <example>
        /// The following example copies the contents this embedded file to another file
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
        ///		EmFile.CopyInternal(drive, "myfile.txt", @"\folder1\afile.txt");
        ///  }
        /// }
        /// </code>
        /// </example>        
        public static void CopyInternal(EmDriveInfo drive, string sourceFileName, string destFileName, bool overwrite = false)
        {
            Utils.CheckParameter(destFileName, "destFileName");
            Utils.CheckParameter(sourceFileName, "sourceFileName");

            if (drive.EFS.FileExists(destFileName) && !overwrite)
                throw new IOException(String.Format("File {0} exists", destFileName));

            using (Stream output = new EmFileInfo(drive, destFileName).OpenWrite())
            using (Stream input = new EmFileInfo(drive, sourceFileName).OpenRead())
            {
                Streams.PipeAll(input, output);
            }
        }



        /// <summary>
        /// Copies an existing file from an <see cref="EmDriveInfo"/> to a new outside file.
        /// </summary>
        /// <param name="drive">Embedded file system contining the source file</param>
        /// <param name="sourceFileName">The file to copy.</param>
        /// <param name="destFileName">The name of the destination file. This cannot be a directory or an existing file.</param>
        /// <param name="overwrite">Optional. If true the target file will be overwriten.</param>
        /// <exception cref="ArgumentNullException">path is null.</exception>
        /// <exception cref="ArgumentException">path is empty, contains only white spaces, or contains invalid characters.</exception>
        /// <example>
        /// The following example copies the contents this embedded file to another file
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
        ///		EmFile.Copy(drive, "myfile.txt", @"c:\folder1\afile.txt");
        ///  }
        /// }
        /// </code>
        /// </example>        
        public static void Copy(EmDriveInfo drive, string sourceFileName, string destFileName, bool overwrite = false)
        {
            Utils.CheckParameter(destFileName, "destFileName");
            Utils.CheckParameter(sourceFileName, "sourceFileName");

            if (File.Exists(destFileName) && !overwrite)
                throw new IOException(String.Format("File {0} exists", destFileName));

            using (Stream output = File.Create(destFileName))
            using (Stream input = new EmFileInfo(drive, sourceFileName).OpenRead())
            {
                Streams.PipeAll(input, output);
            }
        }



        /// <summary>
        /// Deletes the specified file.
        /// </summary>
        /// <param name="drive"></param>
        /// <param name="path">The name of the file to be deleted. Wildcard characters are not supported.</param>
        /// <exception cref="ArgumentNullException">path is null.</exception>
        /// <exception cref="ArgumentException">path is empty, contains only white spaces, or contains invalid characters.</exception>
        /// <remarks>
        /// If the file doesn't exist, nothing happens
        /// </remarks>
        /// <exception cref=""
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
        ///     EmFile.Delete(drive, @"\folder1\file.txt");
        ///  }
        /// }
        /// </code>
        /// </example>        
        public static void Delete(EmDriveInfo drive, string path)
        {
            Utils.CheckParameter(path, "path");

            new EmFileInfo(drive, path).Delete();
        }



        /// <summary>
        /// Creates or overwrites a file in the specified path.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">The path and name of the file to create.</param>
        /// <returns>A <see cref="Stream"/> that provides write access to the file specified in <paramref name="path"/></returns>
        /// <exception cref="ArgumentNullException">path is null.</exception>
        /// <exception cref="ArgumentException">path is empty, contains only white spaces, or contains invalid characters.</exception>
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
        ///		using (Stream fs = EmFile.Create(drive, "myfile.txt"))
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
        public static Stream Create(EmDriveInfo drive, string path)
        {
            return new EmFileInfo(drive, path).OpenWrite();
        }



        /// <summary>
        /// Creates or opens a file for writing UTF-8 encoded text. If the file already exists, its contents are overwritten.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">The file to be opened for writing.</param>
        /// <returns>A <see cref="StreamWriter"/> that writes to the specified file using UTF-8 encoding.</returns>
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
        ///		using (StreamWriter fs = EmFile.CreateText(drive, "myfile.txt"))
        ///		{
        ///		 fs.WriteLine("This is some text in the file.");
        ///		}
        ///  }
        /// }
        /// </code>
        /// </example>        
        public static System.IO.StreamWriter CreateText(EmDriveInfo drive, string path)
        {
            return new StreamWriter(Create(drive, path));
        }



        /// <summary>
        /// Determines whether the specified file exists.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">The file to check.</param>
        /// <returns>true if path contains the name of an existing file; otherwise, false. This method also returns false if path is null, an invalid path, or a zero-length string.</returns>
        /// <example>
        /// The following example demonstrates the Exists method
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
        ///		if (EmFile.Exists(drive, "myfile.txt"))
        ///		{
        ///		 Console.WriteLine("file xists");
        ///		}
        ///  }
        /// }
        /// </code>
        /// </example>        
        public static bool Exists(EmDriveInfo drive, string path)
        {
            return drive.EFS.FileExists(path);
        }



        /// <summary>
        /// Gets the <see cref="FileAttributes"/> of the file on the path.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">The path to the file.</param>
        /// <returns>The <see cref="FileAttributes"/> of the file on the path.</returns>
        /// <exception cref="FileNotFoundException"><paramref name="path"/> path represents a file and is invalid, such as being on an unmapped drive, or the file cannot be found.</exception>
        public static System.IO.FileAttributes GetAttributes(EmDriveInfo drive, string path)
        {
            Utils.CheckParameter(path, "path");

            return drive.EFS.GetAttributes(path);            
        }



        /// <summary>
        /// Returns the creation date and time of the specified file or directory.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">path to the file</param>
        /// <returns>creation date and time of the specified file in local Time</returns>
        /// <example>
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
        ///		Console.WriteLine(EmFile.GetCreationTime(drive, "myfile.txt"));
        ///  }
        /// }
        /// </code>
        /// </example>        
        public static DateTime GetCreationTime(EmDriveInfo drive, string path)
        {
            Utils.CheckParameter(path, "path");

            return new EmFileInfo(drive, path).CreationTime;
        }



        /// <summary>
        /// Returns the creation date and time in UTC of the specified file or directory.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">path to the file</param>
        /// <returns>creation date and time of the specified file in UTC</returns>
        /// <example>
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
        ///		Console.WriteLine(EmFile.GetCreationTimeUtc(drive, "myfile.txt"));
        ///  }
        /// }
        /// </code>
        /// </example>        
        public static DateTime GetCreationTimeUtc(EmDriveInfo drive, string path)
        {
            Utils.CheckParameter(path, "path");

            return new EmFileInfo(drive, path).CreationTimeUtc;
        }



        /// <summary>
        /// Returns the last access date and time of the specified file or directory.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">path to the file</param>
        /// <returns>last access date and time of the specified file in local Time</returns>
        /// <example>
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
        ///		Console.WriteLine(EmFile.GetLastAccessTime(drive, "myfile.txt"));
        ///  }
        /// }
        /// </code>
        /// </example>        
        public static DateTime GetLastAccessTime(EmDriveInfo drive, string path)
        {
            Utils.CheckParameter(path, "path");

            return new EmFileInfo(drive, path).LastAccessTime;
        }



        /// <summary>
        /// Returns the last access date and time in UTC of the specified file or directory.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">path to the file</param>
        /// <returns>last access date and time of the specified file in UTC</returns>
        /// <example>
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
        ///		Console.WriteLine(EmFile.GetLastAccessTimeUtc(drive, "myfile.txt"));
        ///  }
        /// }
        /// </code>
        /// </example>        
        public static DateTime GetLastAccessTimeUtc(EmDriveInfo drive, string path)
        {
            Utils.CheckParameter(path, "path");

            return new EmFileInfo(drive, path).LastAccessTimeUtc;
        }



        /// <summary>
        /// Returns the last write date and time of the specified file or directory.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">path to the file</param>
        /// <returns>last write date and time of the specified file in local Time</returns>
        /// <example>
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
        ///		Console.WriteLine(EmFile.GetLastWriteTime(drive, "myfile.txt"));
        ///  }
        /// }
        /// </code>
        /// </example>        
        public static DateTime GetLastWriteTime(EmDriveInfo drive, string path)
        {
            Utils.CheckParameter(path, "path");

            return new EmFileInfo(drive, path).LastWriteTime;
        }



        /// <summary>
        /// Returns the last write date and time in UTC of the specified file or directory.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">path to the file</param>
        /// <returns>last write date and time of the specified file in UTC</returns>
        /// <example>
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
        ///		Console.WriteLine(EmFile.GetLastWriteTimeUtc(drive, "myfile.txt"));
        ///  }
        /// }
        /// </code>
        /// </example>        
        public static DateTime GetLastWriteTimeUtc(EmDriveInfo drive, string path)
        {
            Utils.CheckParameter(path, "path");

            return new EmFileInfo(drive, path).LastWriteTimeUtc;
        }



        /// <summary>
        /// Moves a specified file to a new location, providing the option to specify a new file name.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="sourceFileName">The name of the file to move. Can include a relative or absolute path.</param>
        /// <param name="destFileName">The new path and name for the file.</param>
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
        ///		EmFile.MoveTo(drive, "myfile.txt", @"\Temp\afile.txt");
        ///  }
        /// }
        /// </code>
        /// </example>        
        public static void Move(EmDriveInfo drive, string sourceFileName, string destFileName)
        {
            Utils.CheckParameter(destFileName, "destFileName");
            Utils.CheckParameter(sourceFileName, "sourceFileName");

            Copy(drive, sourceFileName, destFileName);
            drive.EFS.Delete(sourceFileName);
        }


        /// <summary>
        /// Opens an existing file for reading.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">The file to be opened for reading.</param>
        /// <returns>A read-only <see cref="Stream"/> on the specified path.</returns>
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
        ///		using (Stream fs = EmFile.OpenRead(drive, "myfile.txt"))
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
        public static Stream OpenRead(EmDriveInfo drive, string path)
        {
            return new EmFileInfo(drive, path).OpenRead();
        }



        /// <summary>
        /// Opens an existing UTF-8 encoded text file for reading.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">The file to be opened for reading.</param>
        /// <returns>A read-only <see cref="StreamReader"/> on the specified path.</returns>
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
        ///		using (StreamReader reader = EmFile.OpenText(drive, "myfile.txt"))
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
        public static StreamReader OpenText(EmDriveInfo drive, string path)
        {
            return new StreamReader(new EmFileInfo(drive, path).OpenRead());
        }



        /// <summary>
        /// Opens an existing file or creates a new file for writing.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">The file to be opened for writing.</param>
        /// <returns>A write-only <see cref="Stream"/> on the specified path.</returns>
        /// <example>
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
        ///		using (Stream fs = EmFile.OpenWrite(drive, "myfile.txt"))
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
        public static Stream OpenWrite(EmDriveInfo drive, string path)
        {
            return new EmFileInfo(drive, path).OpenWrite();
        }



        /// <summary>
        /// Opens a binary file, reads the contents of the file into a byte array, and then closes the file.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">The file to be opened</param>
        /// <returns>A byte array containing the contents of the file.</returns>
        /// <example>
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
        ///		 Byte[] info = EmFile.ReadAllBytes(drive, "myfile.txt")
        ///  }
        /// }
        /// </code>
        /// </example>        
        public static byte[] ReadAllBytes(EmDriveInfo drive, string path)
        {
            Utils.CheckParameter(path, "path");

            using (Stream s = new EmFileInfo(drive, path).OpenRead())
            {
                return Streams.ReadAll(s);
            }
        }



        /// <summary>
        /// Asynchronously opens a binary file, reads the contents of the file into a byte array, and then closes the file.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">The file to be opened</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <returns>A task that represents the asynchronous read operation, which wraps the byte array containing the contents of the file.</returns>
        /// <example>
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
        ///		 Byte[] info = await EmFile.ReadAllBytesAsync(drive, "myfile.txt")
        ///  }
        /// }
        /// </code>
        /// </example>        
        public static System.Threading.Tasks.Task<byte[]> ReadAllBytesAsync(EmDriveInfo drive, string path, System.Threading.CancellationToken cancellationToken)
        {
            return new Task<byte[]>(() => ReadAllBytes(drive, path), cancellationToken);
        }



        /// <summary>
        /// Opens a text file, reads all lines of the file into a string array, and then closes the file.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">The file to be opened</param>
        /// <returns>A string array containing all lines of the file.</returns>
        /// <example>
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
        ///		 string[] info = EmFile.ReadAllLines(drive, "myfile.txt");
        ///  }
        /// }
        /// </code>
        /// </example>        
        public static string[] ReadAllLines(EmDriveInfo drive, string path)
        {
            Utils.CheckParameter(path, "path");

            List<string> lines = new List<string>();

            using (Stream s = new EmFileInfo(drive, path).OpenRead())
            using (StreamReader sr = new StreamReader(s))
            {
                string line = null;
                while ((line = sr.ReadLine()) != null)
                {
                    lines.Add(line);
                }
            }

            return lines.ToArray();
        }



        /// <summary>
        /// Opens a text file, reads all lines of the file into a string array, and then closes the file.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">The file to be opened</param>
        /// <param name="encoding"><see cref="Encoding"/> of the file</param>
        /// <returns>A string array containing all lines of the file.</returns>
        /// <example>
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
        ///		 string[] info = EmFile.ReadAllLines(drive, "myfile.txt");
        ///  }
        /// }
        /// </code>
        /// </example>        
        public static string[] ReadAllLines(EmDriveInfo drive, string path, Encoding encoding)
        {
            Utils.CheckParameter(path, "path");

            List<string> lines = new List<string>();

            using (Stream s = new EmFileInfo(drive, path).OpenRead())
            using (StreamReader sr = new StreamReader(s, encoding))
            {
                string line = null;
                while ((line = sr.ReadLine()) != null)
                {
                    lines.Add(line);
                }
            }

            return lines.ToArray();
        }



        /// <summary>
        /// Asynchronously opens a text file, reads all lines of the file, and then closes the file.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">The file to be opened</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <returns>A task that represents the asynchronous read operation, which wraps the string array containing all lines of the file.</returns>
        /// <example>
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
        ///		 string[] info = await EmFile.ReadAllLinesAsync(drive, "myfile.txt")
        ///  }
        /// }
        /// </code>
        /// </example>        
        public static System.Threading.Tasks.Task<string[]> ReadAllLinesAsync(EmDriveInfo drive, string path, System.Threading.CancellationToken cancellationToken)
        {
            return new Task<string[]>(() => ReadAllLines(drive, path), cancellationToken);
        }



        /// <summary>
        /// Asynchronously opens a text file, reads all lines of the file, and then closes the file.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">The file to be opened</param>
        /// <param name="encoding">The encoding applied to the contents of the file.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <returns>A task that represents the asynchronous read operation, which wraps the string array containing all lines of the file.</returns>
        /// <example>
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
        ///		 string[] info = await EmFile.ReadAllLinesAsync(drive, "myfile.txt")
        ///  }
        /// }
        /// </code>
        /// </example>        
        public static System.Threading.Tasks.Task<string[]> ReadAllLinesAsync(EmDriveInfo drive, string path, Encoding encoding, System.Threading.CancellationToken cancellationToken)
        {
            return new Task<string[]>(() => ReadAllLines(drive, path, encoding), cancellationToken);
        }



        /// <summary>
        /// Opens a text file, reads all the text in the file into a string, and then closes the file.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">The file to be opened</param>
        /// <param name="encoding">The encoding applied to the contents of the file.</param>
        /// <returns>A string containing all text in the file.</returns>
        /// <example>
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
        ///		 string info = EmFile.ReadAllText(drive, "myfile.txt")
        ///  }
        /// }
        /// </code>
        /// </example>        
        public static string ReadAllText(EmDriveInfo drive, string path, System.Text.Encoding encoding)
        {
            Utils.CheckParameter(path, "path");

            using (Stream s = new EmFileInfo(drive, path).OpenRead())
            using (StreamReader sr = new StreamReader(s, encoding))
            {
                return sr.ReadToEnd();
            }
        }


        /// <summary>
        /// Opens a text file, reads all the text in the file into a string, and then closes the file.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">The file to be opened</param>
        /// <returns>A string containing all text in the file.</returns>
        /// <example>
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
        ///		 string info = EmFile.ReadAllText(drive, "myfile.txt")
        ///  }
        /// }
        /// </code>
        /// </example>        
        public static string ReadAllText(EmDriveInfo drive, string path)
        {
            return ReadAllText(drive, path, Encoding.UTF8);
        }



        /// <summary>
        /// Asynchronously opens a text file, reads all the text in the file, and then closes the file.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">The file to be opened</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <returns>A task that represents the asynchronous read operation, which wraps the string containing all text in the file.</returns>
        /// <example>
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
        ///		 string info = await EmFile.ReadAllTextAsync(drive, "myfile.txt")
        ///  }
        /// }
        /// </code>
        /// </example>        
        public static System.Threading.Tasks.Task<string> ReadAllTextAsync(EmDriveInfo drive, string path, System.Threading.CancellationToken cancellationToken)
        {
            return new Task<string>(() => ReadAllText(drive, path), cancellationToken);
        }



        /// <summary>
        /// Asynchronously opens a text file, reads all the text in the file, and then closes the file.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">The file to be opened</param>
        /// <param name="encoding">The encoding applied to the contents of the file.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <returns>A task that represents the asynchronous read operation, which wraps the string containing all text in the file.</returns>
        public static System.Threading.Tasks.Task<string> ReadAllTextAsync(EmDriveInfo drive, string path, Encoding encoding, System.Threading.CancellationToken cancellationToken)
        {
            return new Task<string>(() => ReadAllText(drive, path, encoding), cancellationToken);
        }



        /// <summary>
        /// Reads the lines of a file.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">The file to be opened</param>
        /// <returns>All the lines of the file, or the lines that are the result of a query.</returns>
        public static System.Collections.Generic.IEnumerable<string> ReadLines(EmDriveInfo drive, string path)
        {
            Utils.CheckParameter(path, "path");

            using (Stream s = new EmFileInfo(drive, path).OpenRead())
            using (StreamReader sr = new StreamReader(s))
            {
                string line = null;
                while ((line = sr.ReadLine()) != null)
                {
                    yield return line;
                }
            }

        }



        /// <summary>
        /// Reads the lines of a file.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">The file to be opened</param>
        /// <param name="encoding">The encoding that is applied to the contents of the file</param>
        /// <returns>All the lines of the file, or the lines that are the result of a query.</returns>
        public static System.Collections.Generic.IEnumerable<string> ReadLines(EmDriveInfo drive, string path, Encoding encoding)
        {
            Utils.CheckParameter(path, "path");

            using (Stream s = new EmFileInfo(drive, path).OpenRead())
            using (StreamReader sr = new StreamReader(s, encoding))
            {
                string line = null;
                while ((line = sr.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }



        /// <summary>
        /// Replaces the contents of a specified file with the contents of another file, deleting the original file, and creating a backup of the replaced file.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="sourceFileName">The name of a file that replaces the file specified by destinationFileName.</param>
        /// <param name="destinationFileName">The name of the file being replaced.</param>
        /// <param name="destinationBackupFileName">The name of the backup file.</param>
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
        ///	    EmFile.Replace(drive, "myfile.txt", @"c:\Temp\afile.txt");
        ///  }
        /// }
        /// </code>
        /// </example>        
        public static void Replace(EmDriveInfo drive, string sourceFileName, string destinationFileName, string destinationBackupFileName = null)
        {
            Move(drive, sourceFileName, destinationFileName);

            if (destinationBackupFileName != null)
            {
                Copy(drive, destinationFileName, destinationBackupFileName);
            }
        }



        /// <summary>
        /// Sets the date and time the file was created.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">The file for which to set the creation date and time information.</param>
        /// <param name="creationTime">A <see cref="DateTime"/> containing the value to set for the creation date and time of path. This value is expressed in local time.</param>
        /// <example>
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
        ///	    EmFile.SetCreationTime(drive, "myfile.txt", DateTime.Now);
        ///  }
        /// }
        /// </code>
        /// </example>        
        public static void SetCreationTime(EmDriveInfo drive, string path, DateTime creationTime)
        {
            Utils.CheckParameter(path, "path");

            LiteFileInfo<string> file = drive.EFS.GetFile(path);

            if (file != null)
            {
                file.Metadata[EmFileSystemInfo.CREATION_TIME] = creationTime;
            }
        }



        /// <summary>
        /// Sets the date and time the file was created.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">The file for which to set the creation date and time information.</param>
        /// <param name="creationTime">A <see cref="DateTime"/> containing the value to set for the creation date and time of path. This value is expressed in UTC</param>
        /// <example>
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
        ///	    EmFile.SetCreationTimeUtc(drive, "myfile.txt", DateTime.UtcNow);
        ///  }
        /// }
        /// </code>
        /// </example>        
        public static void SetCreationTimeUtc(EmDriveInfo drive, string path, DateTime creationTime)
        {
            Utils.CheckParameter(path, "path");

            LiteFileInfo<string> file = drive.EFS.GetFile(path);

            if (file != null)
            {
                file.Metadata[EmFileSystemInfo.CREATION_TIME] = creationTime.ToLocalTime();
            }
        }



        /// <summary>
        /// Sets the date and time the file was created.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">The file for which to set the creation date and time information.</param>
        /// <param name="accessTime">A <see cref="DateTime"/> containing the value to set for the last access date and time of path. This value is expressed in local time.</param>
        /// <example>
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
        ///	    EmFile.SetLastAccessTime(drive, "myfile.txt", DateTime.Now);
        ///  }
        /// }
        /// </code>
        /// </example>        
        public static void SetLastAccessTime(EmDriveInfo drive, string path, DateTime accessTime)
        {
            Utils.CheckParameter(path, "path");

            LiteFileInfo<string> file = drive.EFS.GetFile(path);

            if (file != null)
            {
                file.Metadata[EmFileSystemInfo.ACCESS_TIME] = accessTime;
            }
        }


        /// <summary>
        /// Sets the date and time the file was created.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">The file for which to set the creation date and time information.</param>
        /// <param name="accessTime">A <see cref="DateTime"/> containing the value to set for the last access date and time of path. This value is expressed in UTC</param>
        /// <example>
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
        ///	    EmFile.SetLastAccessTime(drive, "myfile.txt", DateTime.UtcNow);
        ///  }
        /// }
        /// </code>
        /// </example>        
        public static void SetLastAccessTimeUtc(EmDriveInfo drive, string path, DateTime accessTime)
        {
            Utils.CheckParameter(path, "path");

            LiteFileInfo<string> file = drive.EFS.GetFile(path);

            if (file != null)
            {
                file.Metadata[EmFileSystemInfo.ACCESS_TIME] = accessTime.ToLocalTime();
            }
        }


        /// <summary>
        /// Creates a new file, writes the specified byte array to the file, and then closes the file. If the target file already exists, it is overwritten.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">The file to write to.</param>
        /// <param name="bytes">The bytes to write to the file.</param>
        /// <example>
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
        ///	    EmFile.WriteAllBytes(drive, "myfile.txt", System.Text.UTF8Endcoding.GetByest("Hello world"));
        ///  }
        /// }
        /// </code>
        /// </example>        
        public static void WriteAllBytes(EmDriveInfo drive, string path, byte[] bytes)
        {
            Utils.CheckParameter(path, "path");

            using (Stream s = new EmFileInfo(drive, path).OpenWrite())
            {
                s.Write(bytes, 0, bytes.Length);
            }
        }



        /// <summary>
        /// Asynchronously creates a new file, writes the specified byte array to the file, and then closes the file. If the target file already exists, it is overwritten.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">The file to write to.</param>
        /// <param name="bytes">The bytes to write to the file.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.  </param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        /// <example>
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
        ///	    await EmFile.WriteAllBytesAsync(drive, "myfile.txt", System.Text.UTF8Endcoding.GetByest("Hello world"));
        ///  }
        /// }
        /// </code>
        /// </example>        
        public static System.Threading.Tasks.Task WriteAllBytesAsync(EmDriveInfo drive, string path, byte[] bytes, System.Threading.CancellationToken cancellationToken)
        {
            return new Task(() => WriteAllBytes(drive, path, bytes), cancellationToken);
        }



        /// <summary>
        /// Creates a new file, writes the specified string array to the file by using the specified encoding, and then closes the file.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">The file to write to.</param>
        /// <param name="contents">The string array to write to the file.</param>
        /// <param name="encoding">An <see cref="Encoding"/> object that represents the character encoding applied to the string array.</param>
        /// <example>
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
        ///	    EmFile.WriteAllLines(drive, "myfile.txt", new string[] {"Hello world"});
        ///  }
        /// }
        /// </code>
        /// </example>        
        public static void WriteAllLines(EmDriveInfo drive, string path, string[] contents, System.Text.Encoding encoding)
        {
            Utils.CheckParameter(path, "path");

            using (Stream s = new EmFileInfo(drive, path).OpenWrite())
            using (StreamWriter writer = new StreamWriter(s, encoding))
            {
                foreach (string line in contents)
                {
                    writer.WriteLine(line);
                }
            }
        }



        /// <summary>
        /// Creates a new file, writes the specified strings to the file by using the specified encoding, and then closes the file.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">The file to write to.</param>
        /// <param name="contents">The strings to write to the file.</param>
        /// <param name="encoding">An <see cref="Encoding"/> object that represents the character encoding applied to the string array.</param>
        /// <example>
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
        ///	    EmFile.WriteAllLines(drive, "myfile.txt", new string[] {"Hello world"});
        ///  }
        /// }
        /// </code>
        /// </example>        
        public static void WriteAllLines(EmDriveInfo drive, string path, IEnumerable<string> contents, System.Text.Encoding encoding)
        {
            Utils.CheckParameter(path, "path");

            using (Stream s = new EmFileInfo(drive, path).OpenWrite())
            using (StreamWriter writer = new StreamWriter(s, encoding))
            {
                foreach (string line in contents)
                {
                    writer.WriteLine(line);
                }
            }
        }



        /// <summary>
        /// Creates a new file, writes the specified strings to the file by using the specified encoding, and then closes the file.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">The file to write to.</param>
        /// <param name="contents">The strings to write to the file.</param>
        public static void WriteAllLines(EmDriveInfo drive, string path, IEnumerable<string> contents)
        {
            WriteAllLines(drive, path, contents, Encoding.UTF8);
        }



        /// <summary>
        /// Creates a new file, writes the specified string array to the file by using the specified encoding, and then closes the file.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">The file to write to.</param>
        /// <param name="contents">The string array to write to the file.</param>
        /// <example>
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
        ///	    EmFile.WriteAllLines(drive, "myfile.txt", new string[] {"Hello world"});
        ///  }
        /// }
        /// </code>
        /// </example>        
        public static void WriteAllLines(EmDriveInfo drive, string path, string[] contents)
        {
            Utils.CheckParameter(path, "path");

            WriteAllLines(drive, path, contents, Encoding.UTF8);
        }



        /// <summary>
        /// Asynchronously creates a new file, writes the specified lines to the file, and then closes the file.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">The file to write to.</param>
        /// <param name="contents">The lines to write to the file.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        public static System.Threading.Tasks.Task WriteAllLinesAsync(EmDriveInfo drive, string path, System.Collections.Generic.IEnumerable<string> contents, System.Threading.CancellationToken cancellationToken)
        {
            return new Task(() => WriteAllLines(drive, path, contents), cancellationToken);
        }



        /// <summary>
        /// Asynchronously creates a new file, writes the specified lines to the file, and then closes the file.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">The file to write to.</param>
        /// <param name="contents">The lines to write to the file.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        public static System.Threading.Tasks.Task WriteAllLinesAsync(EmDriveInfo drive, string path, System.Collections.Generic.IEnumerable<string> contents, Encoding encoding, System.Threading.CancellationToken cancellationToken)
        {
            return new Task(() => WriteAllLines(drive, path, contents, encoding), cancellationToken);
        }



        /// <summary>
        /// Creates a new file, write the contents to the file, and then closes the file. If the target file already exists, it is overwritten.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">The file to write to.</param>
        /// <param name="contents">The lines to write to the file.</param>
        /// <example>
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
        ///	    EmFile.WriteAllText(drive, "myfile.txt", "Hello world");
        ///  }
        /// }
        /// </code>
        /// </example>        
        public static void WriteAllText(EmDriveInfo drive, string path, string contents)
        {
            WriteAllText(drive, path, contents, Encoding.UTF8);
        }


        /// <summary>
        /// Creates a new file, write the contents to the file, and then closes the file. If the target file already exists, it is overwritten.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">The file to write to.</param>
        /// <param name="contents">The lines to write to the file.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <example>
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
        ///	    EmFile.WriteAllText(drive, "myfile.txt", "Hello world");
        ///  }
        /// }
        /// </code>
        /// </example>        
        public static void WriteAllText(EmDriveInfo drive, string path, string contents, Encoding encoding)
        {
            Utils.CheckParameter(path, "path");

            using (Stream s = new EmFileInfo(drive, path).OpenWrite())
            using (StreamWriter writer = new StreamWriter(s, encoding))
            {
                writer.Write(contents);
            }
        }



        /// <summary>
        /// Asynchronously creates a new file, writes the specified string to the file, and then closes the file. If the target file already exists, it is overwritten.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">The file to write to.</param>
        /// <param name="contents">The lines to write to the file.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        /// <example>
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
        ///	    await EmFile.WriteAllTextAsync(drive, "myfile.txt", "Hello world");
        ///  }
        /// }
        /// </code>
        /// </example>        
        public static System.Threading.Tasks.Task WriteAllTextAsync(EmDriveInfo drive, string path, string contents, System.Threading.CancellationToken cancellationToken)
        {
            return new Task(() => WriteAllText(drive, path, contents), cancellationToken);
        }



        /// <summary>
        /// Asynchronously creates a new file, writes the specified string to the file, and then closes the file. If the target file already exists, it is overwritten.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">The file to write to.</param>
        /// <param name="contents">The lines to write to the file.</param>
        /// <param name="encoding">The encoding to apply to the string.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        /// <example>
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
        ///	    await EmFile.WriteAllTextAsync(drive, "myfile.txt", "Hello world");
        ///  }
        /// }
        /// </code>
        /// </example>        
        public static System.Threading.Tasks.Task WriteAllTextAsync(EmDriveInfo drive, string path, string contents, Encoding encoding, System.Threading.CancellationToken cancellationToken)
        {
            return new Task(() => WriteAllText(drive, path, contents, encoding), cancellationToken);
        }
    }
}
