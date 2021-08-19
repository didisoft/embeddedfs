using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using LiteDB;

/// <summary>
/// Embedded file system
/// </summary>
namespace EmbeddedFS
{
    /// <summary>
    /// Provides access to information on an embedded file system.
    /// </summary>
    /// <remarks>
    /// This class models a drive and provides methods and properties to query for drive information. 
    /// Use <see cref="EmDriveInfo"/> to create embedded file systems
    /// </remarks>
    public class EmDriveInfo : IDisposable
    {
        /// <summary>
        /// Constant for the <see cref="EmDriveInfo.FullName"/> when it is located in-memory
        /// </summary>
        /// <seealso cref="DriveFormat"/>
        /// <seealso cref="InMemory"/>
        public const string MEMORY = "MEMORY";
        /// <summary>
        /// Constant for the <see cref="EmDriveInfo.FullName"/> when it is located in a file. 
        /// </summary>
        /// <seealso cref="DriveFormat"/>
        public const string FILE = "FILE";


        internal static DateTime masterBuildDate = new DateTime(2021, 4, 30);

        private EmbeddedFileSystem efs;
        private MemoryStream mem;
        private static Dictionary<string, EmbeddedFileSystem> pool = new Dictionary<string, EmbeddedFileSystem>();



        /// <summary>
        /// A property that indicates is this a production or an evaluation version of the library
        /// </summary>
        public bool TrialVersion
        {
            get
            {
#if TRIAL_VERSION
                return true;
#else
                return false;
#endif
            }
        }

        private void checkTrial()
        {
#if TRIAL_VERSION

            DateTime buildDate = masterBuildDate;
            if (DateTime.Now.AddDays(-45).CompareTo(buildDate) > 0)
            {
                throw new Exception("Your evaluation period for DidiSoft EmbeddedFS for .NET has expired. You have to purchase a production copy if you want to continue using this product.");
            }
#endif
        }

        // <summary>
        /// Gets the full path of the embedded file system.
        /// </summary>
        /// <value>A string containing the full path of the embedded file system or <see cref="EmDriveInfo.MEMORY"/></value>
        /// <remarks>
        /// If the emmbedded file system is located in-memory the constant <see cref="EmDriveInfo.MEMORY"/> is returned
        /// </remarks>
        public string FullName { get { return this.efs.Name; } }


        /// <summary>
        /// Is this embedded drive in-memory located
        /// </summary>
        /// <value>true when in-memory, false when backed by a file</value>
        /// <seealso cref="MEMORY"/>
        /// <seealso cref="DriveFormat"/>
        public bool InMemory { get { return this.mem != null; } }



        #region Constructors

        internal void Init(EmbeddedFileSystem efs)
        {
            checkTrial();

            this.efs = efs;
            this.RootDirectory = new EmDirectoryInfo(this, Convert.ToString(Path.DirectorySeparatorChar));
        }

        internal EmDriveInfo(EmbeddedFileSystem efs)
        {
            Init(efs);
        }



        /// <summary>
        /// Provides access to an embedded file system located In the RAM memory
        /// </summary>
        /// <example>
        /// The following example shows how to create an embedded file system in the RAM memory 
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
        ///             // create an embedded file system located in-memory
        ///             EmDriveInfo drive = new EmDriveInfo();
        ///             
        ///             // load from a previously persisted location
        ///             drive.LoadFrom(@"c:\Data\embedded.fs");
        ///             ...
        ///             
        ///             // persist the data if needed
        ///             drive.SaveTo(@"c:\Data\embedded.fs");
        ///         }
        ///     }
        /// }        
        /// </code>
        /// </example>
        public EmDriveInfo()
        {
            this.mem = new MemoryStream();
            Init(new EmbeddedFileSystem(this.mem));
        }



        /// <summary>
        /// Loads a previously persisted embedded file system in-memory
        /// </summary>
        /// <remarks>
        /// After this method the embedded file system remains to be in-memory
        /// </remarks>
        /// <param name="fileName">file location of a previously persisted embedded file system</param>
        /// <seealso cref="SaveTo(string)"/>
        /// <seealso cref="SaveTo(Stream)"/>
        /// <example>
        /// The following example shows how to load ab embedded file system in the RAM memory 
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
        ///             // create an embedded file system located in-memory
        ///             EmDriveInfo drive = new EmDriveInfo();
        ///             
        ///             // load from a previously persisted location
        ///             drive.LoadFrom(@"c:\Data\embedded.fs");
        ///             ...
        ///             
        ///             // persist the data if needed
        ///             drive.SaveTo(@"c:\Data\embedded.fs");
        ///         }
        ///     }
        /// }        
        /// </code>
        /// </example>
        public void LoadFrom(string fileName)
        {
            using (Stream fIn = File.OpenRead(fileName))
            {
                LoadFrom(fIn);
            }
        }



        /// <summary>
        /// Loads a previously persisted embedded file system in-memory
        /// </summary>
        /// <remarks>
        /// After this method the embedded file system remains to be in-memory
        /// </remarks>
        /// <param name="dataStream">Stream containing previously persisted embedded file system</param>
        /// <seealso cref="SaveTo(string)"/>
        /// <seealso cref="SaveTo(Stream)"/>
        /// <example>
        /// The following example shows how to load an embedded file system in the RAM memory 
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
        ///             // create an embedded file system located in-memory
        ///             EmDriveInfo drive = new EmDriveInfo();
        ///             
        ///             // load from a previously persisted location
        ///             drive.LoadFrom(File.OpenRead(@"c:\Data\embedded.fs"));
        ///             ...
        ///             
        ///             // persist the data if needed
        ///             drive.SaveTo(@"c:\Data\embedded.fs");
        ///         }
        ///     }
        /// }        
        /// </code>
        /// </example>
        public void LoadFrom(Stream dataStream)
        {
            MemoryStream mem = new MemoryStream();
            Streams.PipeAll(dataStream, mem);

            mem.Position = 0;
            this.mem = mem;

            Init(new EmbeddedFileSystem(this.mem));
        }



        /// <summary>
        /// Provides access to information on the specified embedded file system.
        /// </summary>
        /// <param name="fileName">file name where the embedded drive is located</param>
        /// <example>
        /// The following example shows how to create an embedded file system drive backed by a file 
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
        ///             EmDriveInfo drive2 = new EmDriveInfo(@"c:\Temp\drive.efs");
        ///         }
        ///     }
        /// }        
        /// </code>
        /// </example>
        public EmDriveInfo(string fileName) : this(fileName, null)
        {
        }



        /// <summary>
        /// Provides access to information on the specified embedded file system.
        /// </summary>
        /// <remarks>
        /// The embedded file system is protected with AES-256 bit encryption
        /// </remarks>
        /// <param name="fileName">file name where the embedded drive is located</param>
        /// <param name="password">Password for protecting the embedded file system</param>
        /// <exception cref="Exceptions.PasswordException">A wrong password was specified</exception>
        /// <example>
        /// The following example shows how to create an embedded file system drive backed by a file and protected by a password
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
        ///             EmDriveInfo drive2 = new EmDriveInfo(@"c:\Temp\drive.efs");
        ///         }
        ///     }
        /// }        
        /// </code>
        /// </example>
        public EmDriveInfo(string fileName, string password)
        {
            string fullPath = Path.GetFullPath(fileName);
            /*          
             *          Shared pool logic
             *            
             *            if (pool.ContainsKey(fullPath))
                        {
                            this.efs = pool[fullPath];
                            if (this.efs.IsDisposed)
                            {
                                this.efs = new EmbeddedFileSystem(fullPath, password);
                                pool[fullPath] = this.efs;
                            }
                        }
                        else
                        {
                            this.efs = new EmbeddedFileSystem(fullPath, password);
                            pool.Add(fullPath, efs);
                        }
                        */
            Init(new EmbeddedFileSystem(fullPath, password));
        }

        #endregion


        #region Password_Methods

        /// <summary>
        /// Checks if a password matches an embedded file system file's password
        /// </summary>
        /// <param name="fileName">embedded file system file to check</param>
        /// <returns>true if password is correct, otherwise false</returns>
        /// <seealso cref="IsPasswordProtected(string)"/>
        /// <seealso cref="SetPassword(string, string)"/>
        /// <seealso cref="ChangePassword(string, string, string)"/>
        /// <example>
        /// The following example checks the password of an embedded file system backed by a file
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
        ///             string password = "123";
        ///             bool isPasswordOk = new EmDriveInfo.CheckPassword(@"c:\Temp\drive.efs", password);
        ///         }
        ///     }
        /// }        
        /// </code>
        /// </example>
        public static bool CheckPassword(string fileName, string password)
        {
            try
            {
                using (EmbeddedFileSystem efs = new EmbeddedFileSystem(fileName, password, true))
                { 
                    return true;
                }
            }
            catch (LiteException e)
            {
                return false;
            }
        }



        /// <summary>
        /// Protects an embedded file system file with a password
        /// </summary>
        /// <exception cref="Exceptions.PasswordException">If the drive is password protected</exception>
        /// <seealso cref="IsPasswordProtected(string)"/>
        /// <seealso cref="ChangePassword(string, string, string)"/>
        /// <example>
        /// The following example checks the password of an embedded file system backed by a file
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
        ///             string password = "123";
        ///             new EmDriveInfo.SetPassword(@"c:\Temp\drive.efs", password);
        ///         }
        ///     }
        /// }        
        /// </code>
        /// </example>
        public static void SetPassword(string fileName, string password)
        {
            try { 
                using (EmbeddedFileSystem efs = new EmbeddedFileSystem(fileName, null, false, false))
                {
                    efs.DB.Rebuild(new LiteDB.Engine.RebuildOptions { Password = password });
                }
            }
            catch (LiteException le)
            {
                HandleException(le);
            }
        }




        private static void HandleException(LiteException le)
        {
            if (le.Message.Contains("This data file is encrypted"))
            {
                throw new Exceptions.PasswordException("This data file is encrypted");
            }
            else
            {
                throw new EmIOException(le.Message, le);
            }
        }



        /// <summary>
        /// Changes the password of a password protected embedded file system
        /// </summary>
        /// <exception cref="Exceptions.PasswordException">If the drive is password protected</exception>
        /// <seealso cref="IsPasswordProtected(string)"/>
        /// <seealso cref="SetPassword(string, string)"/>
        /// <example>
        /// The following example checks the password of an embedded file system backed by a file
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
        ///             string oldPassword = "123";
        ///             string newPassword = "1234";
        ///             new EmDriveInfo.ChangePassword(@"c:\Temp\drive.efs", oldPassword, newPassword);
        ///         }
        ///     }
        /// }        
        /// </code>
        /// </example>
        public static void ChangePassword(string fileName, string oldPassword, string newPassword)
        {
            try
            {
                using (EmbeddedFileSystem efs = new EmbeddedFileSystem(fileName, oldPassword))
                {
                    efs.DB.Rebuild(new LiteDB.Engine.RebuildOptions { Password = newPassword });
                }
            }
            catch (LiteException le)
            {
                HandleException(le);
            }
        }



        /// <summary>
        /// Checks if an embedded file system file is password protected
        /// </summary>
        /// <param name="fileName">embedded file system file to check</param>
        /// <returns>true if password protected, otherwise false</returns>
        /// <returns>true if password is correct, otherwise false</returns>
        /// <seealso cref="SetPassword(string, string)"/>
        /// <seealso cref="ChangePassword(string, string, string)"/>
        /// <seealso cref="CheckPassword(string, string)"/>
        /// <example>
        /// The following example checks is an embedded file system protected with a password
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
        ///             bool passwordProtected = new EmDriveInfo.IsPasswordProtected(@"c:\Temp\drive.efs");
        ///         }
        ///     }
        /// }        
        /// </code>
        /// </example>
        public static bool IsPasswordProtected(string fileName)
        {
            try {
                using (EmbeddedFileSystem efs = new EmbeddedFileSystem(fileName, null, true))
                { 
                    return false;
                }
            }
            catch (LiteException e)
            {
                return true;
            }
        }

        #endregion



        /// <summary>
        /// Disposes this drive and frees any resources held by it
        /// </summary>
        public void Dispose()
        {
            this.efs.Dispose();
        }



        public override string ToString()
        {
            return InMemory ? MEMORY : this.FullName;
        }



        /// <summary>
        /// Indicates the amount of available free space on a drive, in bytes.
        /// </summary>
        /// <value>The amount of free space available on the drive, in bytes.</value>
        public long AvailableFreeSpace { get { return long.MaxValue; } }



        /// <summary>
        /// Gets the type of the file system, such as <see cref="FILE"/> or <see cref="MEMORY"/>
        /// </summary>
        /// <value>Gets the type of the file system</value>
        /// <seealso cref="FILE"/>
        /// <seealso cref="MEMORY"/>
        public string DriveFormat { get { return InMemory ? MEMORY : FILE;  } }


        /// <summary>
        /// Gets a value that indicates whether a drive is ready.
        /// </summary>
        public bool IsReady { get { return true; } }


        /// <summary>
        /// Gets the root directory of a drive.
        /// </summary>
        /// <example>
        /// The following example shows how to get the root directory
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
        ///             EmDriveInfo drive = new EmDriveInfo();
        ///             EmDirectoryInfo root = drive.RootDirectory;
        /// 
        ///             Console.WriteLine(root.FullName);
        ///         }
        ///     }
        /// }        
        /// </code>
        /// </example>
        public EmDirectoryInfo RootDirectory { get; private set; }



        /// <summary>
        /// Gets the total amount of free space available on a drive, in bytes.
        /// </summary>
        /// <value>The total free space available on a drive, in bytes.</value>
        public long TotalFreeSpace { get { return long.MaxValue; } }


        /// <summary>
        /// Gets the total size of storage space on a drive, in bytes.
        /// </summary>
        /// <value>The total size of the drive, in bytes.</value>
        public long TotalSize { get; }


        /// <summary>
        /// Gets or sets the volume label of a drive.
        /// </summary>
        public string VolumeLabel { get; set; }



        /// <summary>
        /// Serialzies this embedded file system to the specified Stream
        /// </summary>
        /// <remarks>
        /// This is especially useful for im-memory based embedded file system
        /// </remarks>
        /// <param name="stream">destination Stream</param>
        /// <example>
        /// The following example shows how to persist an in-memory embedded file system
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
        ///             EmDriveInfo drive = new EmDriveInfo();
        ///             ...
        ///             using (Stream s = File.Create(@"c:\Temp\persisted.fs"))
        ///             {
        ///                drive.SaveTo(s);
        ///             }
        ///         }
        ///     }
        /// }        
        /// </code>
        /// </example>
        public void SaveTo(Stream stream)
        {
            if (this.mem != null)
            {
                this.mem.Position = 0;
                Streams.PipeAll(this.mem, stream);
            }
            else
            {
                using (Stream fIn = File.OpenRead(this.FullName))
                {
                    Streams.PipeAll(fIn, stream);
                }
            }
        }



        /// <summary>
        /// Serialzies this embedded file system to the specified Stream
        /// </summary>
        /// <param name="stream">destination Stream</param>
        /// <example>
        /// The following example shows how to persist an in-memory embedded file system
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
        ///             EmDriveInfo drive = new EmDriveInfo();
        ///             ...
        ///             drive.SaveTo(@"c:\Temp\persisted.fs");
        ///         }
        ///     }
        /// }        
        /// </code>
        /// </example>
        public void SaveTo(string fileName)
        {
            if (this.mem != null)
            {
                this.mem.Position = 0;
                using (Stream fOut = File.Create(fileName))
                {
                    Streams.PipeAll(this.mem, fOut);
                }
            }
            else
            {
                using (Stream fIn = File.OpenRead(this.FullName))
                using (Stream fOut = File.Create(fileName))
                {
                    Streams.PipeAll(fIn, fOut);
                }
            }
        }



        internal EmbeddedFileSystem EFS { get { return efs; }  }
    }
}
