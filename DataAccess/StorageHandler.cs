using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace UserManagementSystem.DataAccess;

public static class StorageHandler
{
    private static Cloudinary Cloudinary;

    public static void Initialize()
    {
        Cloudinary = new Cloudinary("cloudinary://<API Key>:<API Secret>@cloudname");
        Cloudinary.Api.Secure = true;
    }

    /// <summary>
    /// Upload a file to Cloudinary under the user's folder.
    /// </summary>
    /// <param name="userID"></param>
    /// <param name="file"></param>
    /// <returns></returns>
    public static async Task<string> UploadFile(string userID, IFormFile file)
    {
        var imageUpload = new ImageUploadParams()
        {
            File = new FileDescription(userID, file.OpenReadStream()),
            Folder = "Users/" + userID,
            UseFilename = true,
            UniqueFilename = false,
            Overwrite = true
        };

        // Upload the file to Cloudinary
        var uploadResult = await Cloudinary.UploadAsync(imageUpload);

        return uploadResult.SecureUrl.ToString();
    }

    /// <summary>
    /// Delete a user folder from Cloudinary.
    /// </summary>
    /// <param name="userID"></param>
    /// <returns></returns>
    public static async Task DeleteUserFolder(string userID)
    {
        // Remove user folder to Cloudinary
        await Cloudinary.DeleteFolderAsync("Users/" + userID);
    }
}
