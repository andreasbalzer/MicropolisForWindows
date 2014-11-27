using System;
using System.Collections.Generic;
using System.Text;

namespace Micropolis.Utils
{
    using System.Threading;

    internal static class ThreadCancellation
    {
        public static void CheckCancellation(CancellationToken cancelToken) {
            if (cancelToken.IsCancellationRequested == true)
            {
                cancelToken.ThrowIfCancellationRequested();
            }
        } 

    }
}
