using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmbeddedFS
{
    internal class Utils
    {
        internal static void CheckParameter(string param, string name)
        {
            if (param == null)
                throw new ArgumentNullException(String.Format("Argument {0} cannot be null", name));

            if (String.IsNullOrWhiteSpace(param))
                throw new ArgumentException(String.Format("Argument {0} cannot be empty", name));

        }
    }
}
