using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UniversityEvents.Application.Services;
using static UniversityEvents.Core.Entities.Auth.IdentityModel;

namespace UniversityEvents.Web.Controllers;

[Authorize]
public class ChatController : Controller
{
    private readonly IChatService _chatService;
    private readonly UserManager<User> _userManager;

    public ChatController(IChatService chatService, UserManager<User> userManager)
    {
        _chatService = chatService;
        _userManager = userManager;
    }

    // User view: shows only messages for the logged-in user
    public async Task<IActionResult> UserChat()
    {
        var user = await _userManager.GetUserAsync(User);
        bool isAdmin = await _userManager.IsInRoleAsync(user, "Student");

        var messages = await _chatService.GetMessagesForUserAsync(user.Id, isAdmin);
        return View(messages);
    }

    // Admin view: shows all messages and user list
    public async Task<IActionResult> AdminChat()
    {
        var users = _userManager.Users.ToList();
        ViewBag.Users = users;
        var messages = await _chatService.GetMessagesForUserAsync(0, true); // Admin sees all messages
        return View(messages);
    }

    // API: get messages for selected user (Admin)
    [HttpGet]
    public async Task<IActionResult> GetMessagesForUser(long userId)
    {
        var messages = await _chatService.GetMessagesForUserAsync(userId, false);
        return Json(messages.Select(m => new
        {
            m.Id,
            m.SenderId,
            m.Message,
            Receivers = m.Receivers.Select(r => r.ReceiverId)
        }));
    }

    //// Send message to specific users (Admin)
    //[HttpPost]
    //public async Task<IActionResult> SendMessage(string message, List<long> receiverIds)
    //{
    //    if (string.IsNullOrWhiteSpace(message) || receiverIds == null || !receiverIds.Any())
    //        return BadRequest("Message or receivers are empty.");

    //    var sender = await _userManager.GetUserAsync(User);
    //    await _chatService.SaveMessageAsync(sender.Id, message, receiverIds);

    //    return Ok(new { success = true });
    //}

   
}
