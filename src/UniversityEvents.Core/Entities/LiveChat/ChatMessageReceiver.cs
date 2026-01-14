using UniversityEvents.Core.Entities.BaseEntities;

namespace UniversityEvents.Core.Entities.LiveChat;

public class ChatMessageReceiver:AuditableEntity
{
    public long ChatMessageId { get; set; }
    public ChatMessage ChatMessage { get; set; }
    public long ReceiverId { get; set; } 
}
