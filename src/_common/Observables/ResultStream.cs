using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// GENERIC INDICATOR CACHE (STREAMING)

/* The base inheritable handler for streaming indicators. */

namespace Skender.Stock.Indicators;
internal abstract class ResultStream<TResult>
    where TResult : IReusableResult
{
    // TODO: can we use this to effectively replace 2 redundant caches
    // I think yes
}
