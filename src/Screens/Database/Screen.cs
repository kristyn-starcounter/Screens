using System;
using Starcounter;

namespace Screens
{
    [Database]
    public class Screen
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string CookieValue { get; set; }
        public DateTime LastAccess { get; set; }

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

