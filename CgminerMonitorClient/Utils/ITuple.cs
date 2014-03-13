using System.Collections;
using System.Text;

namespace CgminerMonitorClient.Utils
{
    internal interface ITuple
    {
        int Size
        {
            get;
        }
        string ToString(StringBuilder sb);
        int GetHashCode(IEqualityComparer comparer);
    }
}
