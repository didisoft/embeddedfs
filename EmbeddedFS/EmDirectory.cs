using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using LiteDB;

namespace EmbeddedFS
{
    /// <summary>
    /// Exposes static methods for creating, moving, and enumerating through directories and subdirectories. This class cannot be inherited.
    /// </summary>
    public static class EmDirectory
    {
        /// <summary>
        /// Creates all the directories in a specified path.
        /// </summary>
        /// <param name="drive">Embedded drive</param>
        /// <param name="path">The directory to create.</param>
        /// <returns>An object that represents the directory at the specified path. This object is returned regardless of whether a directory at the specified path already exists.</returns>
        /// <example>
        /// This example creates a directory into an embedded drive
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
        ///     EmDirectory.Create(drive, "data");
        ///  }
        /// }
        /// </code>
        /// </example>        
        public static EmDirectoryInfo CreateDirectory(EmDriveInfo drive, string path)
        {
            return drive.RootDirectory.CreateSubdirectory(path);
        }



        /// <summary>
        /// Deletes a specified directory, and optionally any subdirectories.
        /// </summary>
        /// <remarks>
        /// If there is no such directory nothing happens
        /// </remarks>
        /// <param name="drive">Embedded drive</param>
        /// <param name="path">The directory to delete</param>
        /// <example>
        /// This example creates a directory into an embedded drive
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
        ///     EmDirectory.Delete(drive, "data");
        ///  }
        /// }
        /// </code>
        /// </example>        
        public static void Delete(EmDriveInfo drive, string path)
        {
            new EmDirectoryInfo(drive, path).Delete();
        }



        /// <summary>
        /// Returns an array of directory full names that meet specified criteria.
        /// </summary>
        /// <param name="drive">Embedded drive</param>
        /// <param name="path">The relative or absolute path to the directory to search. This string is not case-sensitive.</param>
        /// <returns>An array of the full names (including paths) for the directories in the directory specified by path.</returns>
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
        ///     foreach (EmFileInfo file in EmDirectory.GetFiles(drive, "data"))
        ///     {
        ///       Console.WriteLine(file.FullName);
        ///     }
        ///     
        ///     foreach (EmDirectoryInfo file in EmDirectory.GetDirectories(drive, "data"))
        ///     {
        ///       Console.WriteLine(file.FullName);
        ///     }
        ///  }
        /// }
        /// </code>
        /// </example>        
        public static string[] GetDirectories(EmDriveInfo drive, string path)
        {
            List<string> dirs = new List<string>();
            foreach (EmDirectoryInfo dir in new EmDirectoryInfo(drive, path).GetDirectories())
            {
                dirs.Add(dir.FullName);
            }
            return dirs.ToArray();
        }



        /// <summary>
        /// Returns an enumerable collection of directory full names that meet specified criteria.
        /// </summary>
        /// <param name="drive">Embedded drive</param>
        /// <param name="path">The relative or absolute path to the directory to search. This string is not case-sensitive.</param>
        /// <returns>An enumerable collection of the full names (including paths) for the directories in the directory specified by path.</returns>
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
        ///     foreach (string file in EmDirectory.EnumerateFiles(drive, "/data", "*.txt"))
        ///     {
        ///       Console.WriteLine(file);
        ///     }
        ///     
        ///     foreach (string file in EmDirectory.EnumerateDirectories(drive, "data"))
        ///     {
        ///       Console.WriteLine(file);
        ///     }
        ///  }
        /// }
        /// </code>
        /// </example>        
        public static System.Collections.Generic.IEnumerable<string> EnumerateDirectories(EmDriveInfo drive, string path)
        {
            foreach(EmDirectoryInfo dir in new EmDirectoryInfo(drive, path).GetDirectories())
            {
                yield return dir.FullName;
            }
        }



        /// <summary>
        /// Returns an enumerable collection of full file names that match a search pattern in a specified path.
        /// </summary>
        /// <param name="drive">Embedded drive</param>
        /// <param name="path">The relative or absolute path to the directory to search. This string is not case-sensitive.</param>
        /// <returns>An enumerable collection of the full names (including paths) for the files in the directory specified by path and that match the specified search pattern.</returns>
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
        ///     foreach (string file in EmDirectory.EnumerateFiles(drive, "/data"))
        ///     {
        ///       Console.WriteLine(file);
        ///     }
        ///     
        ///     foreach (string file in EmDirectory.EnumerateDirectories(drive, "data"))
        ///     {
        ///       Console.WriteLine(file);
        ///     }
        ///  }
        /// }
        /// </code>
        /// </example>        
        public static System.Collections.Generic.IEnumerable<string> EnumerateFiles(EmDriveInfo drive, string path)
        {
            return EnumerateFiles(drive, path, "*.*");
        }



        /// <summary>
        /// Returns an enumerable collection of full file names that match a search pattern in a specified path.
        /// </summary>
        /// <param name="drive">Embedded drive</param>
        /// <param name="path">The relative or absolute path to the directory to search. This string is not case-sensitive.</param>
        /// <param name="searchPattern">The search string to match against the names of files in path. This parameter can contain a combination of valid literal path and wildcard (* and ?) characters, but it doesn't support regular expressions.</param>
        /// <returns>An enumerable collection of the full names (including paths) for the files in the directory specified by path and that match the specified search pattern.</returns>
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
        ///     foreach (string file in EmDirectory.EnumerateFiles(drive, "/data", "*.txt"))
        ///     {
        ///       Console.WriteLine(file);
        ///     }
        ///     
        ///     foreach (string file in EmDirectory.EnumerateDirectories(drive, "data"))
        ///     {
        ///       Console.WriteLine(file);
        ///     }
        ///  }
        /// }
        /// </code>
        /// </example>        
        public static System.Collections.Generic.IEnumerable<string> EnumerateFiles(EmDriveInfo drive, string path, string searchPattern)
        {
            foreach (EmFileInfo dir in new EmDirectoryInfo(drive, path).GetFiles())
            {
                Regex regex = FindFilesPatternToRegex.Convert(searchPattern);
                if (regex.IsMatch(dir.FullName))
                {
                    yield return dir.FullName;
                }
            }
        }



        /// <summary>
        /// Returns an arrays of full file names in a specified path.
        /// </summary>
        /// <param name="drive">Embedded drive</param>
        /// <param name="path">The relative or absolute path to the directory to search. This string is not case-sensitive.</param>
        /// <returns>An arrays of the full names (including paths) for the files in the directory specified by path.</returns>
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
        ///     foreach (string file in EmDirectory.GetFiles(drive, "data"))
        ///     {
        ///       Console.WriteLine(file);
        ///     }
        ///     
        ///     foreach (string file in EmDirectory.GetDirectories(drive, "data"))
        ///     {
        ///       Console.WriteLine(file);
        ///     }
        ///  }
        /// }
        /// </code>
        /// </example>        
        public static string[] GetFiles(EmDriveInfo drive, string path)
        {
            return GetFiles(drive, path, "*.*");
        }



        /// <summary>
        /// Returns an arrays of full file names that match a search pattern in a specified path.
        /// </summary>
        /// <param name="drive">Embedded drive</param>
        /// <param name="path">The relative or absolute path to the directory to search. This string is not case-sensitive.</param>
        /// <param name="searchPattern">The search string to match against the names of files in path. This parameter can contain a combination of valid literal path and wildcard (* and ?) characters, but it doesn't support regular expressions.</param>
        /// <returns>An arrays of the full names (including paths) for the files in the directory specified by path and that match the specified search pattern.</returns>
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
        ///     foreach (string file in EmDirectory.GetFiles(drive, "/data", "*.txt"))
        ///     {
        ///       Console.WriteLine(file);
        ///     }
        ///  }
        /// }
        /// </code>
        /// </example>        
        public static string[] GetFiles(EmDriveInfo drive, string path, string searchPattern)
        {
            List<string> files = new List<string>();
            foreach (EmFileInfo dir in new EmDirectoryInfo(drive, path).GetFiles())
            {
                Regex regex = FindFilesPatternToRegex.Convert(searchPattern);
                if (regex.IsMatch(dir.FullName))
                {
                    files.Add(dir.FullName);
                }
            }

            return files.ToArray();
        }



        /// <summary>
        /// Determines whether the given path refers to an existing directory
        /// </summary>
        /// <param name="drive"></param>
        /// <param name="path">The path to test.</param>
        /// <returns>true if path refers to an existing directory; false if the directory does not exist or an error occurs when trying to determine if the specified directory exists.</returns>
        /// <example>
        /// This example creates a directory into an embedded drive
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
        ///     If (EmDirectory.Exists(drive, "data"))
        ///     {
        ///         Console.WriteLine("Directory data exists!");
        ///     }
        ///  }
        /// }
        /// </code>
        /// </example>        
        public static bool Exists(EmDriveInfo drive, string path)
        {
            return new EmDirectoryInfo(drive, path).Exists;
        }



        /// <summary>
        /// Returns the last access date and time of the specified directory or directory.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">path to the directory</param>
        /// <returns>last access date and time of the specified directory in local Time</returns>
        public static DateTime GetLastAccessTime(EmDriveInfo drive, string path)
        {
            return new EmDirectoryInfo(drive, path).LastAccessTime;
        }



        /// <summary>
        /// Returns the last access date and time in UTC of the specified directory or directory.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">path to the directory</param>
        /// <returns>last access date and time of the specified directory in UTC</returns>
        public static DateTime GetLastAccessTimeUtc(EmDriveInfo drive, string path)
        {
            return new EmDirectoryInfo(drive, path).LastAccessTimeUtc;
        }



        /// <summary>
        /// Returns the last write date and time of the specified directory or directory.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">path to the directory</param>
        /// <returns>last write date and time of the specified directory in local Time</returns>
        public static DateTime GetLastWriteTime(EmDriveInfo drive, string path)
        {
            return new EmDirectoryInfo(drive, path).LastWriteTime;
        }



        /// <summary>
        /// Returns the last write date and time in UTC of the specified directory or directory.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">path to the directory</param>
        /// <returns>last write date and time of the specified directory in UTC</returns>
        public static DateTime GetLastWriteTimeUtc(EmDriveInfo drive, string path)
        {
            return new EmDirectoryInfo(drive, path).LastWriteTimeUtc;
        }



        /// <summary>
        /// Retrieves the parent directory of the specified path, including both absolute and relative paths.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">The path for which to retrieve the parent directory.</param>
        /// <returns>The parent directory, or null if path is the root directory</returns>
        public static EmDirectoryInfo GetParent(EmDriveInfo drive, string path)
        {
            return new EmDirectoryInfo(drive, path).Parent;
        }



        /// <summary>
        /// Moves a specified directory to a new location
        /// </summary>
        /// <param name="drive">Embedded file system contining the directory</param>
        /// <param name="sourceDirName">The name of the directory to move. Can include a relative or absolute path.</param>
        /// <param name="destDirName">The new path and name</param>
        public static void Move(EmDriveInfo drive, string sourceDirName, string destDirName)
        {
            new EmDirectoryInfo(drive, sourceDirName).MoveTo(destDirName);
        }



        /// <summary>
        /// Sets the date and time the directory was created.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">The directory for which to set the creation date and time information.</param>
        /// <param name="creationTime">A <see cref="DateTime"/> containing the value to set for the creation date and time of path. This value is expressed in local time.</param>
        public static void SetCreationTime(EmDriveInfo drive, string path, DateTime creationTime)
        {
            LiteFileInfo<string> file = drive.EFS.GetFile(path);

            if (file != null)
            {
                file.Metadata[EmFileSystemInfo.CREATION_TIME] = creationTime;
            }
        }


        /// <summary>
        /// Sets the date and time the directory was created.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">The directory for which to set the creation date and time information.</param>
        /// <param name="creationTime">A <see cref="DateTime"/> containing the value to set for the creation date and time of path. This value is expressed in UTC</param>
        public static void SetCreationTimeUtc(EmDriveInfo drive, string path, DateTime creationTime)
        {
            LiteFileInfo<string> file = drive.EFS.GetFile(path);

            if (file != null)
            {
                file.Metadata[EmFileSystemInfo.CREATION_TIME] = creationTime.ToLocalTime();
            }
        }



        /// <summary>
        /// Sets the date and time the directory was created.
        /// </summary>
        /// <param name="drive">Embedded file system contining the file</param>
        /// <param name="path">The directory for which to set the creation date and time information.</param>
        /// <param name="accessTime">A <see cref="DateTime"/> containing the value to set for the last access date and time of path. This value is expressed in local time.</param>
        public static void SetLastAccessTime(EmDriveInfo drive, string path, DateTime accessTime)
        {
            LiteFileInfo<string> file = drive.EFS.GetFile(path);

            if (file != null)
            {
                file.Metadata[EmFileSystemInfo.ACCESS_TIME] = accessTime;
            }
        }


        /// <summary>
        /// Sets the date and time the directory was created.
        /// </summary>
        /// <param name="drive">Embedded file system contining the directory</param>
        /// <param name="path">The directory for which to set the creation date and time information.</param>
        /// <param name="accessTime">A <see cref="DateTime"/> containing the value to set for the last access date and time of path. This value is expressed in UTC</param>
        public static void SetLastAccessTimeUtc(EmDriveInfo drive, string path, DateTime accessTime)
        {
            LiteFileInfo<string> file = drive.EFS.GetFile(path);

            if (file != null)
            {
                file.Metadata[EmFileSystemInfo.ACCESS_TIME] = accessTime.ToLocalTime();
            }
        }
    }
}
