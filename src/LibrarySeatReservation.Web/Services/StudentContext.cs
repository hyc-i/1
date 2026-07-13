namespace LibrarySeatReservation.Web.Services;

public class StudentContext : IStudentContext
{
    private const string NameKey = "StudentName";
    private const string IdKey = "StudentIdentifier";

    private readonly IHttpContextAccessor _httpContextAccessor;

    public StudentContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ISession Session => _httpContextAccessor.HttpContext!.Session;

    public string? CurrentStudentName => Session.GetString(NameKey);
    public string? CurrentStudentIdentifier => Session.GetString(IdKey);
    public bool IsLoggedIn => CurrentStudentIdentifier != null;

    public void Login(string name, string identifier)
    {
        Session.SetString(NameKey, name);
        Session.SetString(IdKey, identifier);
    }

    public void Logout()
    {
        Session.Clear();
    }
}
