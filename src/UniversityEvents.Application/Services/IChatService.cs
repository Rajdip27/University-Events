
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UniversityEvents.Application.Helpers;
using UniversityEvents.Core.Entities.LiveChat;
using UniversityEvents.Infrastructure.Data;
using static UniversityEvents.Core.Entities.Auth.IdentityModel;

namespace UniversityEvents.Application.Services;

public interface IChatService
{
    Task SaveMessageAsync(long senderId, string message);
    Task<List<ChatMessage>> GetMessagesForUserAsync(long userId, bool isAdmin);
}
public class ChatService(UniversityDbContext _context, UserManager<User> _userManager) : IChatService
{
    public async Task SaveMessageAsync(long senderId, string message)
    {
        try
        {
            var roles = new List<string> { "Administrator", "EventManager" };
            var usersInRoles = await RoleHelperById.GetAllUserIdsByRolesAsync(_userManager, roles);
            // Map to ChatMessageReceiver objects
            var chat = new ChatMessage
            {
                SenderId = senderId,
                Message = message,
                Receivers = usersInRoles.Select(rid => new ChatMessageReceiver
                {
                    ReceiverId = rid
                }).ToList()
            };
            _context.ChatMessages.Add(chat);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    public async Task<List<ChatMessage>> GetMessagesForUserAsync(long userId, bool isAdmin)
    {
        if (isAdmin)
        {
            return await _context.ChatMessages
                .Include(c => c.Receivers)
                .OrderBy(c => c.SentAt)
                .ToListAsync();
        }

        return await _context.ChatMessages
            .Include(c => c.Receivers)
            .Where(c => c.SenderId == userId || c.Receivers.Any(r => r.ReceiverId == userId))
            .OrderBy(c => c.SentAt)
            .ToListAsync();
    }
}