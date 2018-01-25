using Starcounter;
using System;
using System.Linq;

namespace Screens
{
    [Database]
    public class UserSession
    {
        public User User;
        public string SessionId;
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// Attatch current session to a user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static UserSession SignIn(User user)
        {
            if (user == null) return null;

            string sessionId = Session.Current?.SessionId;
            if (sessionId == null) return null;

            UserSession userSession = GetOne(sessionId);

            if (userSession == null)
            {
                // Create user session
                return Db.Transact(() =>
                {
                    return new UserSession() { User = user, SessionId = sessionId };
                });
            }

            if (userSession.User == user)
            {
                // Already signed in
                return userSession;
            }

            // Signout current user
            SignOut(userSession);

            // Create new user session
            return Db.Transact(() =>
            {
                return new UserSession() { User = user, SessionId = sessionId };
            });
        }

        private static UserSession GetOne(string sessionId)
        {
            return Db.SQL<UserSession>($"SELECT o FROM {typeof(UserSession)} o WHERE o.{nameof(UserSession.SessionId)} = ?", sessionId).FirstOrDefault();
        }

        public static User GetSignedInUser()
        {
            string sessionId = Session.Current?.SessionId;
            if (sessionId == null) return null;

            UserSession userSession = UserSession.GetOne(sessionId);
            if (userSession == null)
            {
                return null;
            }

            return userSession.User;
        }

        public static void SignOut()
        {
            string sessionId = Session.Current?.SessionId;
            if (sessionId == null) return;
            SignOut(UserSession.GetOne(sessionId));
        }

        private static void SignOut(UserSession userSession)
        {
            if (userSession == null) return;

            Db.Transact(() =>
            {
                userSession.Delete();
            });
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
