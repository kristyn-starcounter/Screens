using System;
using Starcounter;

namespace Screens
{
    [Database]
    public class Screen
    {
        public string Name;
        public string Description;
        public string CookieValue;
        public DateTime LastAccess;

        public static void RegisterHooks()
        {
            // Cleanup
            Hook<Screen>.BeforeDelete += (sender, screen) =>
            {
                Db.SQL($"DELETE FROM {typeof(ScreenTempCode)} WHERE {nameof(ScreenTempCode.Screen)} = ?", screen);
                Db.SQL($"DELETE FROM {typeof(UserScreenRelation)} WHERE {nameof(ScreenTempCode.Screen)} = ?", screen);
            };
        }
    }
}

