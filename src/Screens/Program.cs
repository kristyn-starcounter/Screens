using Starcounter;
using System.Linq;
using Screens.ViewModels;

namespace Screens
{
    class Program
    {

        internal const string CookieName = "screenid";

        static void Main()
        {
            Application.Current.Use(new HtmlFromJsonProvider());
            Application.Current.Use(new PartialToStandaloneHtmlProvider());

            // Hooks
            Screen.RegisterHooks();
            User.RegisterHooks();
            UserSession.RegisterHooks();

            // Handlers
            MainHandlers.Register();

            // Blending
            BlenderMapping.Register();
        }
    }
}