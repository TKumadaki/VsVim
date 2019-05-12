using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VimApp.ScriptTest
{
    public class EditorUtil
    {

    }
    public class EditorCoreUtil
    {
        public static bool IsEndPoint(SnapshotPoint point)
        {
            return point.Position == point.Snapshot.Length;
        }
    }
}
