using Starcounter;
using System.Linq;

namespace Screens
{
    [Database]
    public class User
    {
        public string Email;
        public string Username;

        public static void RegisterHooks()
        {
            // Cleanup
            Hook<User>.BeforeDelete += (sender, user) =>
            {
                Db.SQL($"DELETE FROM {typeof(UserScreenRelation)} WHERE {nameof(UserScreenRelation.User)} = ?", user);
            };
        }


        private static User GetAnonymousUser()
        {
            return Db.SQL<User>($"SELECT o FROM {typeof(User)} o WHERE o.{nameof(User.Username)} = ?", "Anonymous").FirstOrDefault();

        }

        private static User AssureAnonymousUser()
        {
            User user = GetAnonymousUser();
            if (user == null)
            {
                Db.Transact(() =>
                {
                    user = new User() { Username = "Anonymous" };
                });
            }
            return user;
        }

    }
}
