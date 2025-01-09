using System;
using System.Collections.Generic;
using System.Threading;
using taskThird.Models;

namespace taskThird.Processing
{
    public interface IProcessingMethod
    {
        void Process(Func<double, double> func, List<Interval> intervals, int steps, CancellationToken token);
    }
}
