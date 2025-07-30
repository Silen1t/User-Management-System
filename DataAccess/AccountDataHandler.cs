using UserManagementSystem.Models;
using Firebase.Database;
using Firebase.Database.Query;
using System.Net.Mail;

namespace UserManagementSystem.DataAccess;

public static class AccountDataHandler
{
    public const string FirebaseDatabaseUrl = "FirebaseDatabaseUrl";
    private static FirebaseClient firebaseClient;
    
    public static void Initialize()
    {
        firebaseClient = new FirebaseClient(FirebaseDatabaseUrl);
    }

    /// <summary>
    /// Check if the account with the given Id exists in the database.
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    public static bool IsAccountExist(int Id)
    {
        // Check if the account exists in the database
        var isAccountExist = firebaseClient
            .Child("Users")
            .Child(Id.ToString())
            .Child("Data")
            .OnceSingleAsync<AccountModel>();

        return isAccountExist != null;
    }

    /// <summary>
    /// Check if the given email string is a vaild email format.
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public static bool IsValidEmail(string email)
    {
        try
        {
            MailAddress mail = new MailAddress(email);
            return mail.Address == email;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Check if the email already exists in the database for a different account.
    /// </summary>
    /// <param name="account"></param>
    /// <returns></returns>
    public static async Task<bool> IsEmailExist(AccountModel account)
    {
        // Retrieve All user IDs
        var UserIds = await firebaseClient
            .Child("Users")
            .OnceAsync<object>();

        foreach (var userId in UserIds)
        {
            // Retrieve account data for each user
            AccountModel newAccount = await firebaseClient
            .Child("Users")
            .Child(userId.Key)
            .Child("Data")
            .OnceSingleAsync<AccountModel>();

            // Check if the email matches the account's email
            if (newAccount != null &&
                newAccount.Email == account.Email &&
                newAccount.Id != account.Id)
            {
                return true; // Email exists for a different account
            }
        }

        return false;
    }

    /// <summary>
    /// Get the account data for the given Id.
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    public static AccountModel GetAccount(string Id)
    {
        // Retrieve the account data from the database
        var account = firebaseClient
            .Child("Users")
            .Child(Id)
            .Child("Data")
            .OnceSingleAsync<AccountModel>()
            .Result;
        return account;
    }

    /// <summary>
    /// Get the account data for the given email and password.
    /// </summary>
    /// <param name="email"></param>
    /// <param name="password"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public static async Task<AccountModel?> GetAccount(string email, string password)
    {
        // Search all users
        var users = await firebaseClient.Child("Users").OnceAsync<object>();

        foreach (var user in users)
        {
            var account = await firebaseClient
                .Child("Users")
                .Child(user.Key)
                .Child("Data")
                .OnceSingleAsync<AccountModel>();

            if (account.Email == email && account.Password == password)
            {
                // Save user info in session
                SessionManager.SetUserIdSession(account.Id);

                return account;
            }
        }

        return null;
    }

    /// <summary>
    /// Create new account and save it to the database.
    /// </summary>
    /// <param name="newAccount"></param>
    /// <returns></returns>
    public static async Task<AccountModel> CreateAccount(AccountModel newAccount)
    {
        AccountModel? existingUser = null;

        do
        {
            newAccount.Id = GetRandomUID();

            existingUser = await firebaseClient
                .Child("Users")
                .Child(newAccount.Id.ToString())
                .Child("Data")
                .OnceSingleAsync<AccountModel>();
        }
        while (existingUser != null);

        // Add the new account to the database
        await firebaseClient
            .Child("Users/" + newAccount.Id + "/Data")
            .PutAsync(newAccount);

        // Return the created account
        return newAccount;
    }

    /// <summary>
    /// Update the account data in the database.
    /// </summary>
    /// <param name="account"></param>
    /// <returns></returns>
    public static async Task UpdateAccount(AccountModel account)
    {
        if (account.UploadedImage != null)
        {
            account.ProfileIcon = await StorageHandler.UploadFile(account.Id, account.UploadedImage);
            account.UploadedImage = null;
        }

        // Update the account data in the database
        await firebaseClient
            .Child("Users/" + account.Id + "/Data")
            .PutAsync(account);
    }

    /// <summary>
    /// Delete the account data in the database.
    /// </summary>
    /// <param name="account"></param>
    /// <returns></returns>
    public static async Task DeleteAccount(AccountModel account)
    {
        await firebaseClient
            .Child("Users/" + account.Id)
            .DeleteAsync();
        SessionManager.ResetUserIdSession();
    }

    private static string GetRandomUID()
    {
        return Guid.NewGuid().ToString().Substring(0, 8);
    }
}
