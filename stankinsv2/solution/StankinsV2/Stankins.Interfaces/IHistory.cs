﻿using System;

namespace Stankins.Interfaces
{
    public interface IHistory : IMetadataRow
    {
        DateTimeOffset ModifiedDate { get; set; }
        string ModifiedOwner { get; set; }
        string ObjectName  { get; set; } 
        string ObjectId { get; set; }
        string Details { get; set; }
    }
}
