using Screens.ViewModels;
using Starcounter;
using System.Collections.Specialized;
using System;
using System.Linq;

namespace Screens
{
    public class MainHandlers
    {

        public static void Register()
        {

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
                        mainPage.Content = Utils.GetScreenContent(screen);
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

            Handle.GET("/Screens/screens", (Request request) =>
            {
                MainPage mainPage = Utils.GetMainPage(request);
                User user = UserSession.GetSignedInUser();
                if (user != null)
                {
                    mainPage.Content = new ScreensPage();
                }
                return mainPage;
            });

            Handle.GET("/Screens/screens/{?}", (string id, Request request) =>
            {
                MainPage mainPage = Utils.GetMainPage(request);
                User user = UserSession.GetSignedInUser();
                if (user == null)
                {
                    return mainPage;
                }

                Screen screen = Db.FromId<Screen>(id);
                if (screen == null)
                {
                    ErrorMessageBox.Show("Screen not found"); // TODO: Show page error instead of popup

                    mainPage.Content = new ScreensPage();
                    return mainPage;
                }

                UserScreenRelation userScreenRelation = Db.SQL<UserScreenRelation>($"SELECT o FROM {typeof(UserScreenRelation)} o WHERE o.{nameof(UserScreenRelation.Screen)} = ? AND o.{nameof(UserScreenRelation.User)} = ?", screen, user).FirstOrDefault();
                if (userScreenRelation == null)
                {
                    ErrorMessageBox.Show("User screen not found"); // TODO: Show page error instead of popup
                    mainPage.Content = new ScreensPage();
                    return mainPage;
                }

                mainPage.Content = Db.Scope<ScreenPage>(() =>
                {
                    ScreenPage screenPage = new ScreenPage
                    {
                        Data = userScreenRelation
                    };

                    screenPage.Init();

                    return screenPage;
                });
                return mainPage;
            });

            Handle.GET("/Screens/addscreen", (Request request) =>
            {
                MainPage mainPage = Utils.GetMainPage(request);
                User user = UserSession.GetSignedInUser();
                if (user == null)
                {
                    return mainPage;
                }

                mainPage.Content = Db.Scope(() =>
                {
                    ScreenPage screenPage = new ScreenPage
                    {
                        Data = new UserScreenRelation { Screen = new Screen(), User = user }
                    };

                    screenPage.Init();

                    return screenPage;
                });

                return mainPage;
            });

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

            Handle.GET("/Screens/screenpluginmapping/{?}", (string screenId) => { return new Json(); });

            Handle.GET("/Screens/menu", () =>
            {
                Menu menu = new Menu();
                menu.Init();
                return menu;
            });
        }
    }
}
