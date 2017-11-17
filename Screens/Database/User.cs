using Starcounter;

namespace Screens
{
    [Database]
    public class User
    {
        public string FirstName;
        public string LastName;

        public static void RegisterHooks()
        {
            // Cleanup
            Hook<User>.BeforeDelete += (sender, user) =>
            {
                Db.SQL($"DELETE FROM {typeof(UserScreenRelation)} WHERE {nameof(UserScreenRelation.User)} = ?", user);

            };
        }
    }
}
