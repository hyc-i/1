namespace LibrarySeatReservation.Web.Services;

public interface IStudentContext
{
    string? CurrentStudentName { get; }
    string? CurrentStudentIdentifier { get; }
    bool IsLoggedIn { get; }
    void Login(string name, string identifier);
    void Logout();
}
