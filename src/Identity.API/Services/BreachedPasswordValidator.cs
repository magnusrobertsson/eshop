namespace eShop.Identity.API.Services;

/// <summary>
/// Rejects passwords that appear in a built-in list of commonly used or breached passwords.
/// The comparison ignores case and surrounding whitespace. Runs on every create, change,
/// set and reset flow handled by <see cref="UserManager{TUser}"/>.
/// </summary>
public class BreachedPasswordValidator<TUser> : IPasswordValidator<TUser> where TUser : class
{
    private static readonly HashSet<string> CommonPasswords = new(StringComparer.OrdinalIgnoreCase)
    {
        "password", "password1", "password12", "password123", "password1!", "passw0rd", "p@ssw0rd",
        "p@ssword1", "p@ssw0rd1", "password!", "password@1", "pa$$w0rd", "pa$$word1",
        "12345678", "123456789", "1234567890", "123123123", "12345678a", "1q2w3e4r", "1q2w3e4r5t",
        "qwerty123", "qwertyuiop", "qwerty12", "qwerty123!", "qwerty1!", "abc12345", "abcd1234",
        "iloveyou", "letmein1", "welcome1", "welcome123", "admin123", "admin1234", "administrator",
        "changeme", "changeme1", "changeme123", "monkey123", "dragon123", "football1", "baseball1",
        "trustno1", "sunshine1", "master123", "superman1", "princess1", "starwars1", "whatever1"
    };

    public Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user, string password)
    {
        if (!string.IsNullOrEmpty(password) && CommonPasswords.Contains(password.Trim()))
        {
            return Task.FromResult(IdentityResult.Failed(new IdentityError
            {
                Code = "BreachedPassword",
                Description = "This password is too common or has appeared in known data breaches. Choose a different password."
            }));
        }

        return Task.FromResult(IdentityResult.Success);
    }
}
