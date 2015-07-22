using System;

namespace Cruder.Core.Contract
{
    public interface IDeletionTrackable
    {
        DateTime? DeletedOn { get; set; }
        int? DeletedBy { get; set; }
    }
}
