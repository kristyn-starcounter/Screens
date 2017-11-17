using System;
using Starcounter;
using System.Linq;
using System.Collections.Specialized;
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
            RegisterHandlers();
            ScreenHandlers.Register();

            // Blending
            RegisterBlending();
        }

        private static void RegisterHandlers()
        {

            Handle.GET("/Screens/reset", (Request request) =>
            {
                MainPage mainPage = Utils.GetMainPage();
                mainPage.Cookie = null;

                Response respone = new Response();
                respone.Headers["Set-Cookie"] = Program.CookieName + "= " + "" + ";Path=/;Expires=Wed, 21 Oct 2015 07:28:00 GMTT"; 
                respone.Headers["Location"] = "/Screens";
                respone.StatusCode = (ushort)System.Net.HttpStatusCode.TemporaryRedirect;
                return respone;
            });

            Handle.GET("/Screens?{?}", (string query, Request request) =>
            {
                NameValueCollection collection = System.Web.HttpUtility.ParseQueryString(query);
                string guid = collection["setcookie"];

                Response respone = new Response();
                respone.Headers["Set-Cookie"] = Program.CookieName + "=" + guid + ";Path=/;Expires=Wed, 2 Dec 2037 00:00:00 GMT"; // TODO: Come up with a better clever way to set a cookie with no expire date
                respone.Headers["Location"] = "/Screens";
                respone.StatusCode = (ushort)System.Net.HttpStatusCode.SeeOther;
                return respone;
            });

            Handle.GET("/Screens", (Request request) =>
            {
                MainPage mainPage = Utils.GetMainPage(request);

                if (mainPage.Cookie != null)
                {
                    // Try find screen
                    Screen screen = Db.SQL<Screen>($"SELECT o FROM {typeof(Screen)} o WHERE o.{nameof(Screen.CookieValue)} = ?", mainPage.Cookie.Value).FirstOrDefault();
                    if (screen != null)
                    {
                        Db.Transact(() => { screen.LastAccess = DateTime.UtcNow; });
                        mainPage.HideMenu = true;
                        ScreenContentPage screenContentPage = new ScreenContentPage() { Data = screen };
                        screenContentPage.Init(screen);
                        mainPage.Content = screenContentPage;
                        return mainPage;
                    }
                    else
                    {
                        //Invalid cookie
                        // TODO: Clear invalid browser cookie
                    }
                }
                else
                {
                    mainPage.HideMenu = false;
                }

                WelcomePage welcomePage = new WelcomePage();
                mainPage.Content = welcomePage;
                return mainPage;
            });
        }

        private static void RegisterBlending()
        {
            Handle.GET("/Screens/screenContent/{?}", (string screenId) => { return new Json(); });
            Blender.MapUri("/Screens/screenContent/{?}", "screenContent");

            Handle.GET("/Screens/screenpluginmapping/{?}", (string screenId) => { return new Json(); });
            Blender.MapUri("/Screens/screenpluginmapping/{?}", "screen");

            Handle.GET("/Screens/menumapping", () =>
            {
                Menu menu = new Menu();
                menu.Init();
                return menu;
            });

            Blender.MapUri("/Screens/menumapping", "menu");
        }
    }
}