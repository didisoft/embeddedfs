using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmbeddedFS
{
    /// <summary>
    /// <see cref="IOException"/> subclass indicating error inside the Embedded file system
    /// </summary>
    public class EmIOException : IOException
    {
        public EmIOException(string message) : base(message) { }

        public EmIOException(string message, Exception e) : base(message, e) { }
    }
}
