using System;

namespace Domain.Shared
{
    public abstract class BaseEntity
    {
        public Guid Id { get; protected set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; protected set; }
        public bool IsDeleted { get; protected set; } = false;
        public void MarkDeleted() => IsDeleted = true;
        public void SetUpdated() => UpdatedAt = DateTime.UtcNow;
    }
}
