using System.Threading.Tasks;
using Zion.Reminder.Models;

namespace Zion.Reminder.Services;

public interface IMessageGenerator
{
    Task<string> GenerateMessageBodyAsync(Event @event, Notification notification);
}
