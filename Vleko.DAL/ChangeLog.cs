using System;
using System.Collections.Generic;

namespace Vleko.DAL
{
    public enum ChangeLogType
    {
        ADD,
        EDIT,
        DELETE
    }

    public class ChangeLog
    {
        public required string Entity { get; set; }
        public required string PrimaryKey { get; set; }
        public ChangeLogType Type { get; set; }
        public List<ChangeLogProperties>? Property { get; set; }
        public DateTime DateChanged { get; set; }
    }
    public class ChangeLogProperties
    {
        public required string Property { get; set; }
        public string? OldValue { get; set; }
        public required string NewValue { get; set; }
    }

}
