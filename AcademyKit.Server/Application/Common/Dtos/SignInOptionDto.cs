namespace AcademyKit.Application.Common.Dtos;

public class SignInOptionDto
{
    public SignInType SignIn { get; set; }
    public bool IsAllowed { get; set; }
}

public enum SignInType
{
    Email = 1,
    Google = 2,
    Microsoft = 3
}
