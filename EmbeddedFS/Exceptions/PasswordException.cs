using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Exceptions related to the embedded file system
/// </summary>
namespace EmbeddedFS.Exceptions
{
    /// <summary>
    /// Indicates a wrong password for opening an embedded file system
    /// </summary>
    public class PasswordException : EmIOException
    {
        public PasswordException(string message) : base(message) { }
    }
}
