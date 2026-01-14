using UniversityEvents.Core.Entities.BaseEntities;

namespace UniversityEvents.Core.Entities.LiveChat;

public class ChatMessage:AuditableEntity
{
    public long SenderId { get; set; } 
    public string Message { get; set; } = string.Empty;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public List<ChatMessageReceiver> Receivers { get; set; } = new();
}
