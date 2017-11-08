// Google Client ID: 525016583199-77s56n08s4uuoir2oppc8gs1biv5t6q9.apps.googleusercontent.com
// Google client secret: hTMyupyDfY8LFFCVnghhBPzK

using System;
using Starcounter;
using System.Linq;
using System.Collections.Specialized;
using Screens.Common;
using Screens.ViewModels;

namespace Screens
{
    class Program
    {
        static void Main()
        {

            Application.Current.Use(new HtmlFromJsonProvider());
            Application.Current.Use(new PartialToStandaloneHtmlProvider());

            Screens.Common.Utils.RegisterHooks();

            Handle.GET("/Screens/reset", (Request request) =>
            {
                MainPage mainPage = GetMainPage();
                mainPage.Cookie = null;
                mainPage.RedirectUrl = "/Screens?setcookie=";
                return mainPage;
            });

            #region App Root
            Handle.GET("/Screens?{?}", (string query, Request request) =>
            {
                NameValueCollection collection = System.Web.HttpUtility.ParseQueryString(query);

                string guid = collection["setcookie"];

                Handle.AddOutgoingCookie("screenid", guid);

                MainPage mainPage = GetMainPage();
                mainPage.RedirectUrl = "/Screens";
                return mainPage;
            });

            Handle.GET("/Screens", (Request request) =>
            {
                IncommingRequest(request);

                MainPage mainPage = GetMainPage();

                if (mainPage.Cookie != null)
                {
                    // Try find screen
                    Screen screen = Db.SQL<Screen>("SELECT o FROM Screens.Common.Screen o WHERE o.CookieValue = ?", mainPage.Cookie.Value).FirstOrDefault();
                    if (screen != null)
                    {
                        Db.Transact(() => { screen.LastAccess = DateTime.UtcNow; });
                        mainPage.HideMenu = true;


                        ScreenContentPage screenContentPage = new ScreenContentPage() { Data = screen };
                        screenContentPage.Init(screen);
                        mainPage.Content = screenContentPage;

//                        mainPage.Content = Self.GET("/Screens/screenContent/" + screen.GetObjectID());
                        return mainPage;
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

            Handle.GET("/Screens/signin", (Request request) =>
            {
                IncommingRequest(request);

                MainPage mainPage = GetMainPage();
                mainPage.Content = new SignInPage();
                return mainPage;
            });

            #endregion

            #region Screen

            Handle.GET("/Screens/screens", (Request request) =>
            {
                IncommingRequest(request);

                MainPage mainPage = GetMainPage();
                User user = UserSession.GetSignedInUser();
                if (user != null)
                {
                    mainPage.Content = new ScreensPage();   // TODO: Only show valid screens for that account
                }
                else
                {
                    mainPage.RedirectUrl = "/Screens/signin";
                    mainPage.SignInRedirectUrl = request.Uri;
                }
                return mainPage;
            });

            Handle.GET("/Screens/screens/{?}", (string id, Request request) =>
            {
                IncommingRequest(request);

                MainPage mainPage = GetMainPage();
                User user = UserSession.GetSignedInUser();

                if (user == null)
                {
                    mainPage.RedirectUrl = "/Screens/signin";
                    mainPage.SignInRedirectUrl = request.Uri;
                    return mainPage;
                }

                Screen screen = Db.SQL<Screen>("SELECT o FROM Screens.Common.Screen o WHERE o.ObjectID=?", id).FirstOrDefault();
                if (screen == null)
                {
                    ErrorMessageBox.Show("Screen not found"); // TODO: Show page error instead of popup
                    mainPage.Content = new ScreensPage();   // TODO: Only show valid screens for that account
                    return mainPage;
                }

                return Db.Scope<MainPage>(() =>
                {
                    ScreenPage screenPage = new ScreenPage();
                    screenPage.Data = screen;
                    screenPage.Init();
                    mainPage.Content = screenPage;
                    return mainPage;
                });

            });

            Handle.GET("/Screens/addscreen", (Request request) =>
            {
                IncommingRequest(request);

                MainPage mainPage = GetMainPage();

                User user = UserSession.GetSignedInUser();
                if (user == null)
                {
                    mainPage.RedirectUrl = "/Screens/signin";
                    mainPage.SignInRedirectUrl = request.Uri;
                    return mainPage;
                }


                return Db.Scope<MainPage>(() =>
                {
                    ScreenPage screenPage = new ScreenPage();
                    screenPage.Data = new Screen();
                    screenPage.Init();
                    UserScreenRelation userScreenRelation = new UserScreenRelation();
                    userScreenRelation.Screen = screenPage.Data;
                    userScreenRelation.User = user; // mainPage.User.Data;
                    mainPage.Content = screenPage;
                    return mainPage;
                });
            });

            #endregion


            #region MenuItems

            Handle.GET("/Screens/menuitem/{?}", (string screenId) =>
            {

                MenuItem menuItem = new MenuItem();
                menuItem.Name = "Screen";


                return menuItem;
            });

            Blender.MapUri("/Screens/menuitem/{?}", "menuitem");


            #endregion

            #region Mapping

            Handle.GET("/Screens/screenContent/{?}", (string screenId) =>
            {
                return new Json();
            });

            Blender.MapUri("/Screens/screenContent/{?}", "screenContent");


            Handle.GET("/Screens/screenpluginmapping/{?}", (string screenId) =>
            {
                return new Json();
            });

            Blender.MapUri("/Screens/screenpluginmapping/{?}", "screen");

            #endregion



        }

        private static void IncommingRequest(Request request)
        {
            // Get and Set Cookie

            Cookie screenCookie = GetCookie(request, "screenid");

            if (screenCookie != null && !string.IsNullOrEmpty(screenCookie.Value))
            {
                GetMainPage().Cookie = screenCookie;

            }

        }

        private static Cookie GetCookie(Request request, string name)
        {
            return request.Cookies.Select(x => new Cookie(x)).FirstOrDefault(x => x.Name == name);
        }


        public static MainPage GetMainPage()
        {
            var session = Session.Ensure();

            MainPage mainPage = session.Store[nameof(MainPage)] as MainPage;

            if (mainPage == null)
            {
                mainPage = new MainPage();
                session.Store[nameof(MainPage)] = mainPage;
            }

            return mainPage;
        }
    }
}