using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;

namespace CRIApp.Repository.Seeders;

public static class UserSeeder
{
    public static Guid Execute(AppContext context)
    {
        User? user;
        if (!context.User.Any())
        {
            user = new User("Admin", DateTime.Now.ToUniversalTime(), Sex.Male, "teste@teste.com", "teste");
            user.SetAsAdmin();
            context.User.Add(user);
            context.SaveChanges();

            return user.Id;
        }
        
        user = context.User.FirstOrDefault(x => x.Name == "Admin");
        return user!.Id;
    }
}