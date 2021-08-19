using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using LiteDB;

namespace EmbeddedFS
{
    /// <summary>
    /// Exposes instance methods for creating, moving, and enumerating through directories and subdirectories. This class cannot be inherited.
    /// </summary>
    /// <remarks>
    /// Use the DirectoryInfo class for typical operations such as copying, moving, renaming, creating, and deleting directories.
    ///
    /// If you are going to reuse an object several times, consider using the instance method of DirectoryInfo instead of the corresponding static methods of the Directory class
    ///
    /// Tha path of the root folder is "\\" in C#, or "\" in Visual Basic.
    /// 
    /// In members that accept a path, the path can refer to a file or just a directory. The specified path can also refer to a relative path. For example, all the following are acceptable paths:
    /// 
    /// \\MyDir\\MyFile.txt" in C#, or "\MyDir\MyFile.txt" in Visual Basic.
    /// 
    /// "MyDir\\MySubdir" in C#, or "MyDir\MySubDir" in Visual Basic.
    /// </remarks>
    /// <example>
    /// The following example checks whether a specified directory exists, creates the directory if it does not exist, and deletes the directory.
    /// <code lang="C#">
    /// using System;
    /// using EmbeddedFS;
    /// 
    /// class Test
    /// {
    ///  public static void Main()
    ///  {
    ///    // file based embedded file system 
    ///    EmDriveInfo drive = new EmDriveInfo(@"c:\Temp\drive2.efs");
    ///    //  Specify the directories you want to manipulate.
    ///    EmDirectoryInfo di = new EmDirectoryInfo(drive, @"\MyDir");
    ///    try
    ///    {
    ///        // Determine whether the directory exists.
    ///        if (di.Exists)
    ///        {
    ///                     // Indicate that the directory already exists.
    ///                     Console.WriteLine("That path exists already.");
    ///                     return;
    ///         }
    /// 
    ///         // Try to create the directory.
    ///         di.Create();
    ///         Console.WriteLine("The directory was created successfully.");
    /// 
    ///         // Delete the directory.
    ///         di.Delete();
    ///         Console.WriteLine("The directory was deleted successfully.");
    ///      }
    ///      catch (Exception e)
    ///      {
    ///          Console.WriteLine("The process failed: {0}", e.ToString());
    ///      }
    ///      finally { }
    ///  }
    /// }
    /// </code>
    /// </example>
    public sealed class EmDirectoryInfo : EmFileSystemInfo
    {
        private EmDriveInfo drive;
        private EmbeddedFileSystem fs;

        /// <summary>
        /// Initializes a new instance of the EmDirectoryInfo class on the specified path.
        /// </summary>
        /// <param name="drive">embedded drive container</param>
        /// <param name="path">A string specifying the path on which to create the EmDirectoryInfo.</param>
        /// <exception cref="ArgumentException">path contains invalid characters such as ", <, >, or |.</exception>
        /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length.</exception>
        public EmDirectoryInfo(EmDriveInfo drive, string path)
        {
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentException(String.Format("Directory name cannot be empty"));

            this.drive = drive;
            this.fs = drive.EFS;
            this.name = fs.FixDirectoryName(path);
            this.attributes |= FileAttributes.Directory;

            this.file = fs.GetFile(this.name);
            if (file != null)
            {
                this.LastWriteTime = this.file.UploadDate;

                BsonValue date;
                if (this.file.Metadata.TryGetValue(ACCESS_TIME, out date))
                { 
                    this.LastAccessTime = date.AsDateTime;
                }
                else
                {
                    this.LastAccessTime = this.LastWriteTime;
                }

                if (this.file.Metadata.TryGetValue(ACCESS_TIME, out date))
                {
                    this.CreationTime = date.AsDateTime;
                }
                else
                {
                    this.LastAccessTime = this.LastWriteTime;
                }

            }
            else
            {
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
        /// The name part of this directory
        /// </summary>
        /// <remarks>
        /// For the root folder this will return <see cref="Path.PathSeparator"/>
        /// </remarks>
        /// <value>the name part of this directory</value>
        public override string Name
        {
            get
            {
                string s = Path.GetFileName(Path.GetDirectoryName(this.name));
                if (String.IsNullOrEmpty(s))
                {
                    return Convert.ToString(Path.DirectorySeparatorChar);
                } 
                else
                { 
                    return s;
                }
            }
        }



        public override string ToString()
        {
            return FullName;
        }



        /// <summary>
        /// Creates all missing directories in a long path
        /// </summary>
        /// <param name="path"></param>
        private void CreatePath(string path)
        {
            String[] parts = path.Split(EmbeddedFileSystem.DirectorySeparator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            StringBuilder sb = new StringBuilder();
            sb.Append(EmbeddedFileSystem.DirectorySeparator);
            foreach (string part in parts)
            {
                sb.Append(part).Append(EmbeddedFileSystem.DirectorySeparator);
                if (!this.fs.FileExists(sb.ToString()))
                {
                    this.fs.CreateDirectory(sb.ToString());
                }
            }
        }



        /// <summary>
        /// Creates a directory.
        /// </summary>
        /// <remarks>If the directory already exists, this method does nothing.</remarks>
        /// <exception cref="IOException">The directory cannot be created.</exception>
        /// <example>
        /// The following example checks whether a specified directory exists, creates the directory if it does not exist, and deletes the directory.
        /// <code lang="C#">
        /// using System;
        /// using EmbeddedFS;
        /// 
        /// class Test
        /// {
        ///  public static void Main()
        ///  {
        ///    // file based embedded file system 
        ///    EmDriveInfo drive = new EmDriveInfo(@"c:\Temp\drive2.efs");
        ///    //  Specify the directories you want to manipulate.
        ///    EmDirectoryInfo di = new EmDirectoryInfo(drive, @"\MyDir");
        ///    try
        ///    {
        ///        // Determine whether the directory exists.
        ///        if (di.Exists)
        ///        {
        ///                     // Indicate that the directory already exists.
        ///                     Console.WriteLine("That path exists already.");
        ///                     return;
        ///         }
        /// 
        ///         // Try to create the directory.
        ///         di.Create();
        ///         Console.WriteLine("The directory was created successfully.");
        /// 
        ///         // Delete the directory.
        ///         di.Delete();
        ///         Console.WriteLine("The directory was deleted successfully.");
        ///      }
        ///      catch (Exception e)
        ///      {
        ///          Console.WriteLine("The process failed: {0}", e.ToString());
        ///      }
        ///      finally { }
        ///  }
        /// }
        /// </code>
        /// </example>
        public void Create()
        {
            // Create file if not exists
            if (!this.fs.FileExists(this.name))
            {
                this.CreatePath(this.name);
                this.file = fs.GetFile(this.name);
            }
        }




        /// <summary>
        /// Creates a subdirectory or subdirectories on the specified path. The specified path can be relative 
        /// to this instance of the <see cref="EmDirectoryInfo"/>class.
        /// </summary>
        /// <param name="path">The specified path</param>
        /// <returns>The last directory specified in path</returns>
        /// <remarks>
        /// Any and all directories specified in path are created, unless some part of path is invalid. 
        /// The path parameter specifies a directory path, not a file path. If the subdirectory already exists, this method does nothing.
        /// </remarks>
        /// <example>
        /// The following example demonstrates creating a subdirectory. In this example, the created directories are removed once created. Therefore, to test this sample, comment out the delete lines in the code.
        /// <code lang="C#">
        /// using System;
        /// using EmbeddedFS;
        /// 
        /// public class CreateSubTest
        /// {
        ///     public static void Main()
        ///     {
        ///  	    // in-memory file system
        ///  	    EmDriveInfo drive = new EmDriveInfo();
        ///         // Create a reference to a directory.
        ///         EmDirectoryInfo di = new EmDirectoryInfo(drive, "TempDir");
        /// 
        ///         // Create the directory only if it does not already exist.
        ///         if (di.Exists == false)
        ///             di.Create();
        /// 
        ///         // Create a subdirectory in the directory just created.
        ///         DirectoryInfo dis = di.CreateSubdirectory("SubDir");
        /// 
        ///         // Process that directory as required.
        ///         // ...
        /// 
        ///         // Delete the subdirectory.
        ///         dis.Delete(true);
        /// 
        ///         // Delete the directory.
        ///         di.Delete(true);
        ///     }
        /// }        
        /// </code>
        /// </example>
        public EmDirectoryInfo CreateSubdirectory(string path)
        {
            string subDir = this.fs.FixDirectoryName(Path.Combine(this.name, path));
            // Create file if not exists
            if (!this.fs.FileExists(subDir))
            {
                this.CreatePath(subDir);
            }

            return new EmDirectoryInfo(this.drive, subDir);
        }



        /// <summary>
        /// Deletes a <see cref="EmDirectoryInfo"/> and its contents from a path.
        /// </summary>
        /// <exception cref="IOException">The directory is not empty.
        /// 
        /// -or-
        /// 
        /// The directory is the root directory of the embedded file system
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">The directory described by this EmDirectoryInfo object does not exist or could not be found.</exception>
        /// <example>
        /// The following example throws an exception if you attempt to delete a directory that is not empty.
        /// <code lang="C#">
        /// using System;
        /// using EmbeddedFS;
        /// 
        /// class Test
        /// {
        ///     public static void Main()
        ///     {
        /// 	    // in-memory file system
        /// 	    EmDriveInfo drive = new EmDriveInfo();
        ///         // Specify the directories you want to manipulate.
        ///         EmDirectoryInfo di1 = new EmDirectoryInfo(drive, @"\MyDir");
        /// 
        ///         try
        ///         {
        ///             // Create the directories.
        ///             di1.Create();
        ///             di1.CreateSubdirectory("temp");
        /// 
        ///             //This operation will not be allowed because there are subdirectories.
        ///             Console.WriteLine("I am about to attempt to delete {0}", di1.Name);
        ///             di1.Delete();
        ///             Console.WriteLine("The Delete operation was successful, which was unexpected.");
        ///         }
        ///         catch (Exception)
        ///         {
        ///             Console.WriteLine("The Delete operation failed as expected.");
        ///         }
        ///         finally {}
        ///     }
        /// }
        /// </code>
        /// </example>
        public void Delete()
        {
            if (this.file == null)
            {
                this.file = fs.GetFile(this.name);
                if (this.file == null)
                    throw new DirectoryNotFoundException(String.Format("Directory {0} doesn't exist.", this.name));
            }

            if (this.drive.RootDirectory.FullName.Equals(this.name))
                throw new IOException("Root directory cannot be deleted");

            if (this.GetDirectories().Length > 0 || this.GetFiles().Length > 0)
                throw new IOException(String.Format("Directory {0} is not empty.", this.name));


            this.fs.DeleteFiles(this.name);
            this.file = null;
        }



        /// <summary>
        /// Moves a <see cref="EmDirectoryInfo"/> instance and its contents to a new path.
        /// </summary>
        /// <remarks>
        /// After the operation the <see cref="EmDirectoryInfo"/> properties will be reflecting the change.
        /// </remarks>
        /// <param name="destPath">The name and path to which to move this directory. The destination cannot be another file system or a directory with the identical name. It can be an existing directory to which you want to add this directory as a subdirectory.</param>
        /// <exception cref="ArgumentException">destPath is null or an empty string (''")</exception>
        /// <exception cref="IOException">An attempt was made to move the root folder</exception>
        /// <exception cref="DirectoryNotFoundException">The destination directory cannot be found.</exception>
        /// <example>
        /// The following example throws an exception if you attempt to delete a directory that is not empty.
        /// <code lang="C#">
        /// using System;
        /// using EmbeddedFS;
        /// 
        /// class Test
        /// {
        ///     public static void Main()
        ///     {
        ///             EmDriveInfo drive = new EmDriveInfo();
        ///             EmDirectoryInfo root = drive2.RootDirectory;
        /// 
        ///             EmDirectoryInfo dir = new EmDirectoryInfo(drive, "/data");
        ///             Assert.IsFalse(dir.Exists);
        ///             dir.Create();
        /// 
        ///             dir.MoveTo("/data2/x1");
        /// 
        ///             // the move operation will be reflected in FullName
        ///             Console.WriteLine(dir.FullName);
        ///         }
        /// }
        /// </code>
        /// </example>
        public void MoveTo(String destPath)
        {
            if (this.file == null)
                throw new DirectoryNotFoundException(this.FullName);

            if (String.IsNullOrEmpty(destPath))
                throw new ArgumentException("Parameter destPath cannot be null or empty");

            if (drive.RootDirectory.FullName.Equals(this.name))
                throw new IOException("Cannot move root directory");

            string destinationDirectoryName = this.fs.FixDirectoryName(destPath);

            EmDirectoryInfo destDir = new EmDirectoryInfo(drive, destinationDirectoryName);
            if (!destDir.Parent.Exists)
                throw new DirectoryNotFoundException(String.Format("Target directory {0} not found", destDir.Parent.FullName));

            if (drive.EFS.FileExists(destinationDirectoryName) || drive.EFS.DirectoryExists(destinationDirectoryName))
                throw new IOException("Cannot create a file when that file already exists.");

            CreatePath(destinationDirectoryName);

            IEnumerator<LiteFileInfo<string>> files = 
                (this.fs.FS as LiteStorage<string>).RawFiles.Find(
                                        s => s.Metadata[EmFileSystemInfo.FILE_PATH].AsString.StartsWith(this.name))
                .GetEnumerator();

            while (files.MoveNext())
            {
                LiteFileInfo<string> file = files.Current;

                file.Metadata[EmFileSystemInfo.FILE_PATH] =
                    file.Metadata[EmFileSystemInfo.FILE_PATH].AsString.Replace(this.name, destPath);
            }

            this.name = destinationDirectoryName;
            this.file = this.fs.GetFile(destinationDirectoryName);
        }



        /// <summary>
        /// Returns a directory list from the current directory.
        /// </summary>
        /// <returns>An array of strongly typed <see cref="EmDirectoryInfo"/> objects</returns>
        /// <exception cref="DirectoryNotFoundException">This directory doesn't exist</exception>
        /// <example>
        /// The following example retrieves all the directories in the root directory and displays the directory names.
        /// <code lang="C#">
        /// using System;
        /// using EmbeddedFS;
        /// 
        /// 
        /// public class GetDirectoriesTest
        /// {
        ///     public static void Main()
        ///     {
        ///         // in-memory file system
        ///         EmDriveInfo drive2 = new EmDriveInfo();
        ///         // Make a reference to the root directory.
        ///         EmDirectoryInfo di = new EmDirectoryInfo(drive, "\\");
        /// 
        ///         // Get a reference to each directory in that directory.
        ///         EmDirectoryInfo[] diArr = di.GetDirectories();
        /// 
        ///         // Display the names of the directories.
        ///         foreach (EmDirectoryInfo dri in diArr)
        ///             Console.WriteLine(dri.Name);
        ///     }
        /// }        
        /// </code>
        /// </example>
        public EmDirectoryInfo[] GetDirectories()
        {
            if (this.file == null)
                throw new DirectoryNotFoundException(this.FullName);

            int separatorsCount = this.file.Metadata[EmFileSystemInfo.FILE_PATH].AsString.Count(f => f == EmbeddedFileSystem.DirectorySeparatorChar);

            IEnumerator<LiteFileInfo<string>> files =
              this.fs.FS.Find(file =>
                (file.Metadata[EmFileSystemInfo.FILE_PATH].AsString.StartsWith(this.name)
                && file.Metadata[EmFileSystemInfo.FILE_PATH].AsString.EndsWith(EmbeddedFileSystem.DirectorySeparator)))
                //&& file.Metadata[EmFileSystemInfo.FILE_PATH].AsString.Count(f => f == EmbeddedFileSystem.DirectorySeparatorChar) == separatorsCount))
              .GetEnumerator();

            LinkedList<EmDirectoryInfo> list = new LinkedList<EmDirectoryInfo>();
            while (files.MoveNext())
            { 
                if (files.Current.Metadata[EmFileSystemInfo.FILE_PATH].AsString.Count(f => f == EmbeddedFileSystem.DirectorySeparatorChar) == separatorsCount+1)
                    list.AddLast(new EmDirectoryInfo(this.drive, files.Current.Metadata[EmFileSystemInfo.FILE_PATH].AsString));
            }

            return list.ToArray<EmDirectoryInfo>();
        }



        /// <summary>
        /// Returns a file list from the current directory.
        /// </summary>
        /// <returns>An array of strongly typed <see cref="EmFileInfo"/> objects</returns>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <example>
        /// The following example shows how to get a list of files from a directory by using different search options. 
        /// <code lang="C#">
        /// using System;
        /// using EmbeddedFS;
        /// 
        /// namespace ConsoleApplication1
        /// {
        ///     class Program
        ///     {
        ///         static void Main(string[] args)
        ///         {
        ///             // in-memory file system
        ///             EmDriveInfo drive2 = new EmDriveInfo();
        ///             EmDirectoryInfo di = new EmDirectoryInfo(drive, @"\");
        ///             Console.WriteLine("No search pattern returns:");
        ///             foreach (var fi in di.GetFiles())
        ///             {
        ///                 Console.WriteLine(fi.Name);
        ///             }
        ///             Console.WriteLine();
        /// 
        ///             Console.WriteLine("Search pattern *2* returns:");
        ///             foreach (var fi in di.GetFiles("*2*"))
        ///             {
        ///                 Console.WriteLine(fi.Name);
        ///             }
        ///             Console.WriteLine();
        /// 
        ///             Console.WriteLine("Search pattern test?.txt returns:");
        ///             foreach (var fi in di.GetFiles("test?.txt"))
        ///             {
        ///                 Console.WriteLine(fi.Name);
        ///             }
        ///         }
        ///     }
        /// }        
        /// </code>
        /// </example>
        public EmFileInfo[] GetFiles()
        {
            if (this.file == null)
                throw new DirectoryNotFoundException(this.FullName);

            int separatorsCount = this.name.Count(f => f == EmbeddedFileSystem.DirectorySeparatorChar);

            IEnumerator<LiteFileInfo<string>> files =
              this.fs.FS.Find(file =>
                (file.Metadata[EmFileSystemInfo.FILE_PATH].AsString.StartsWith(this.name))
                &&
                !file.Metadata[EmFileSystemInfo.FILE_PATH].AsString.EndsWith(EmbeddedFileSystem.DirectorySeparator))
              .GetEnumerator();

            LinkedList<EmFileInfo> list = new LinkedList<EmFileInfo>();
            while (files.MoveNext()) {
                if (files.Current.Metadata[EmFileSystemInfo.FILE_PATH].AsString.Count(f => f == EmbeddedFileSystem.DirectorySeparatorChar) == separatorsCount)
                    list.AddLast(new EmFileInfo(this.drive, files.Current.Metadata[EmFileSystemInfo.FILE_PATH].AsString));
            }

            return list.ToArray<EmFileInfo>();
        }



        /// <summary>
        /// Returns a file list from the current directory.
        /// </summary>
        /// <param name="searchPattern">The search string to match against the names of files. This parameter can contain a combination of valid literal path and wildcard (* and ?) characters, but it doesn't support regular expressions.</param>
        /// <returns>An array of strongly typed <see cref="EmFileInfo"/> objects</returns>
        /// <example>
        /// The following example shows how to get a list of files from a directory by using different search options. 
        /// <code lang="C#">
        /// using System;
        /// using EmbeddedFS;
        /// 
        /// namespace ConsoleApplication1
        /// {
        ///     class Program
        ///     {
        ///         static void Main(string[] args)
        ///         {
        ///             // in-memory file system
        ///             EmDriveInfo drive2 = new EmDriveInfo();
        ///             EmDirectoryInfo di = new EmDirectoryInfo(drive, @"\");
        /// 
        ///             Console.WriteLine("Search pattern *2* returns:");
        ///             foreach (var fi in di.GetFiles("*2*"))
        ///             {
        ///                 Console.WriteLine(fi.Name);
        ///             }
        ///             Console.WriteLine();
        /// 
        ///             Console.WriteLine("Search pattern test?.txt returns:");
        ///             foreach (var fi in di.GetFiles("test?.txt"))
        ///             {
        ///                 Console.WriteLine(fi.Name);
        ///             }
        ///         }
        ///     }
        /// }        
        /// </code>
        /// </example>
        public EmFileInfo[] GetFiles(string searchPattern)
        {
            int separatorsCount = this.name.Count(f => f == EmbeddedFileSystem.DirectorySeparatorChar);

            IEnumerator<LiteFileInfo<string>> files =
              this.fs.FS.Find(file =>
                (file.Metadata[EmFileSystemInfo.FILE_PATH].AsString.StartsWith(this.name))
                &&
                !file.Metadata[EmFileSystemInfo.FILE_PATH].AsString.EndsWith(EmbeddedFileSystem.DirectorySeparator))
              .GetEnumerator();

            System.Text.RegularExpressions.Regex regex = FindFilesPatternToRegex.Convert(searchPattern);

            LinkedList<EmFileInfo> list = new LinkedList<EmFileInfo>();
            while (files.MoveNext())
            {
                if (files.Current.Metadata[EmFileSystemInfo.FILE_PATH].AsString.Count(f => f == EmbeddedFileSystem.DirectorySeparatorChar) == separatorsCount
                    && regex.IsMatch(files.Current.Metadata[EmFileSystemInfo.FILE_PATH].AsString))
                { 
                    list.AddLast(new EmFileInfo(this.drive, files.Current.Metadata[EmFileSystemInfo.FILE_PATH].AsString));
                }
            }

            return list.ToArray<EmFileInfo>();
        }



        /// <summary>
        /// Returns an enumerable collection of file information in the current directory.
        /// </summary>
        /// <returns>An enumerable collection of the files in the current directory.</returns>
        public System.Collections.Generic.IEnumerable<EmFileInfo> EnumerateFiles()
        {
            foreach(EmFileInfo file in GetFiles())
            {
                yield return file;
            }
        }


        /// <summary>
        /// Returns an enumerable collection of directory information in the current directory.
        /// </summary>
        /// <returns>An enumerable collection of directories</returns>
        public System.Collections.Generic.IEnumerable<EmDirectoryInfo> EnumerateDirectories()
        {
            foreach (EmDirectoryInfo file in GetDirectories())
            {
                yield return file;
            }
        }




        /// <summary>
        /// Gets the root directory of the embedded file system.
        /// </summary>
        public EmDirectoryInfo Root
        {
            get
            {
                return this.drive.RootDirectory;
            }
        }



        /// <summary>
        /// Gets the parent directory of a specified subdirectory.
        /// </summary>
        /// <value>The parent directory, or null if the path is null or if the file path denotes a root (such as \ or /).</value>
        public EmDirectoryInfo Parent
        {
            get
            {
                // Root
                if (Path.GetPathRoot(this.name).Equals(this.name))
                {
                    return null;
                }

                int pos = this.name.LastIndexOf(EmbeddedFileSystem.DirectorySeparator);
                pos = name.LastIndexOf(EmbeddedFileSystem.DirectorySeparator, pos - 1);
                if (pos == 0)
                {
                    return new EmDirectoryInfo(this.drive, this.name.Substring(0, 1));
                }
                else
                {
                    return new EmDirectoryInfo(this.drive, this.name.Substring(0, pos));
                }
            }
        }
    }
}
