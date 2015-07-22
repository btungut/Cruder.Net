using System;

namespace Cruder.Core.Contract
{
    public interface ICreationTrackable
    {
        DateTime CreatedOn { get; set; }
        int CreatedBy { get; set; }
    }
}
