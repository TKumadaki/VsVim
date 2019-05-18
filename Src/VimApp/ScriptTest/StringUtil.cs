using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VimApp.ScriptTest
{
    public class StringUtil
    {
        public static int Length(string str)
        {
            if (str == null)
                return 0;

            return str.Length;
        }
    }
}
