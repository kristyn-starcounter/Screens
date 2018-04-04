using Starcounter;
using System;
using System.Linq;

namespace Screens
{
    [Database]
    public class UserSession
    {
        public User User { get; set; }
        public string SessionId { get; set; }
        public DateTime ExpiresAt { get; set; }

        public static User GetSignedInUser()
        {
            User user = Db.SQL<User>($"SELECT o FROM {typeof(User)} o").FirstOrDefault();

            if (user == null)
            {
                Db.Transact(() =>
                {
                    user = new User() { Username = "Anonymous", Email = "" };
                });
            }
            return user;
        }

        public static void RegisterHooks()
        {
            Hook<User>.BeforeDelete += (sender, user) =>
            {
                Db.SQL($"DELETE FROM {typeof(UserSession)} WHERE {nameof(UserSession.User)} = ?", user);
            };
        }
    }
}
