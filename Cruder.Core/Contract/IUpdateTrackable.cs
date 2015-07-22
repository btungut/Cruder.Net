using System;

namespace Cruder.Core.Contract
{
    public interface IUpdateTrackable
    {
        DateTime UpdatedOn { get; set; }
        int UpdatedBy { get; set; }
    }
}
