using System;

namespace Husky.GridQuery
{
    [Flags]
    public enum GridColumnAggregates
    {
        None,
        Sum,
        Average,
        Min,
        Max
    }
}
