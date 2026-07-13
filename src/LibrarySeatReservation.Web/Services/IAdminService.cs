using LibrarySeatReservation.Web.Models.ViewModels;

namespace LibrarySeatReservation.Web.Services;

public interface IAdminService
{
    Task<bool> ValidateLoginAsync(string username, string password);
    Task<AdminStatisticsViewModel> GetStatisticsAsync();
}
