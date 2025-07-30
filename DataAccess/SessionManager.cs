namespace UserManagementSystem.DataAccess;

public static class SessionManager
{
    private const string UserIdSessionKey = "UserId";
    private static IHttpContextAccessor HttpContextAccessor;


    public static void Configure(IHttpContextAccessor httpContextAccessor)
    {
        HttpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Sets the user Id in the session.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="context"></param>
    public static void SetUserIdSession(string userId)
    {
        HttpContextAccessor.HttpContext!.Session.SetString(UserIdSessionKey, userId);
    }

    /// <summary>
    /// Resets the user Id in the session by removing it.
    /// </summary>
    /// <param name="context"></param>
    public static void ResetUserIdSession()
    {
        HttpContextAccessor.HttpContext!.Session.Remove(UserIdSessionKey);
    }

    /// <summary>
    /// Checks if the user is logged in by checking if the UserId exists in the session.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static bool IsUserLoggedIn()
    {
        return HttpContextAccessor.HttpContext!.Session.GetString(UserIdSessionKey) != null;
    }

    /// <summary>
    /// Get user Id from the session.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static string GetUserId()
    {
        return HttpContextAccessor.HttpContext!.Session.GetString(UserIdSessionKey)!;
    }
}
