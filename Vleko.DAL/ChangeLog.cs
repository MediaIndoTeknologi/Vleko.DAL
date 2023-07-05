using System;
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
        public ChangeLogType Type { get; set; }
        public required string PrimaryKey { get; set; }
        public required string Property { get; set; }
        public string? OldValue { get; set; }
        public required string NewValue { get; set; }
        public DateTime DateChanged
        {
            get
            {
                return DateChanged;
            }
            private set
            {
                DateChanged = DateTime.UtcNow;
            }
        }
    };

}
