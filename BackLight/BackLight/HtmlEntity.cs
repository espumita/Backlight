namespace Backlight {
    public class HtmlEntity {
        public string EntityType { get; }
        public bool CanCreate { get; }
        public bool CanRead { get; }
        public bool CanUpdate { get; }
        public bool CanDelete { get; }

        public HtmlEntity(string entityType, bool canCreate, bool canRead, bool canUpdate, bool canDelete) {
            EntityType = entityType;
            CanCreate = canCreate;
            CanRead = canRead;
            CanUpdate = canUpdate;
            CanDelete = canDelete;
        }

    }
}