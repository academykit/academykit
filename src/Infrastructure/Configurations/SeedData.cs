namespace Lingtren.Infrastructure.Configurations
{
    public class SeedData
    {
        // public static async Task SeedUsers(WebApplication app)
        // {
        //     //Seed Super Admin
        //     using (var scope = app.Services.CreateScope())
        //     {
        //         var services = scope.ServiceProvider;
        //         var userService = services.GetRequiredService<IUserService>();
        //         var zoomSettingService = services.GetRequiredService<IZoomSettingService>();
        //         var smtpSettingService = services.GetRequiredService<ISMTPSettingService>();

        //         var currentTimeStamp = DateTime.UtcNow;

        //         var adminUser = await userRepository.GetByIdAsync(1).ConfigureAwait(false);
        //         if (adminUser == null)
        //         {
        //             var hashAdminPassword = userTokenService.HashPassword("Momo@12345");

        //             //Super Admin
        //             users.Add(new User
        //             {
        //                 Id = 1,
        //                 UserName = "SuperAdmin",
        //                 Password = hashAdminPassword,
        //                 FirstName = "Super",
        //                 LastName = "Admin",
        //                 Email = "",
        //                 Phone = "01-4443720",
        //                 UserStatus = UserStatusEnum.Active,
        //                 UserRole = RoleEnum.SuperAdmin,
        //                 CreatedOn = currentTimeStamp,
        //             });
        //         }

        //         var defaultUser = await userRepository.GetByIdAsync(2).ConfigureAwait(false);
        //         if (defaultUser == null)
        //         {
        //             // Unknown user
        //             users.Add(new User
        //             {
        //                 Id = 2,
        //                 UserName = "Someone",
        //                 Password = "",
        //                 FirstName = "Someone",
        //                 LastName = "",
        //                 Email = null,
        //                 Phone = null,
        //                 UserStatus = UserStatusEnum.Active,
        //                 UserRole = RoleEnum.User,
        //                 CreatedOn = currentTimeStamp,
        //             });
        //         }
        //         if (users.Count > 0)
        //         {
        //             await userRepository.AddAsync(users).ConfigureAwait(false);
        //         }
        //     }
        //}
    }
}
