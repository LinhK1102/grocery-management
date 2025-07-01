using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Common
{
    public class Convention
    {
        public static string ToUpperOrNA(string? value) => string.IsNullOrWhiteSpace(value) ? "N/A" : value.ToUpper();
    }
}
