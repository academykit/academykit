namespace AcademyKit.Application.Common.Dtos;

public class SignInOptionDto
{
    public required SignInType SignIn { get; set; }
    public required bool IsAllowed { get; set; }
}

public enum SignInType
{
    Email = 1,
    Google = 2,
    Microsoft = 3
}
