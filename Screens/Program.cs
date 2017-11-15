// Google Client ID: 525016583199-77s56n08s4uuoir2oppc8gs1biv5t6q9.apps.googleusercontent.com
// Google client secret: hTMyupyDfY8LFFCVnghhBPzK

using System;
using Starcounter;
using System.Linq;
using System.Collections.Specialized;
using Screens.Common;
using Screens.ViewModels;
using System.Reflection;

namespace Screens
{
    class Program
    {
        static void Main()
        {

            Application.Current.Use(new HtmlFromJsonProvider());
            Application.Current.Use(new PartialToStandaloneHtmlProvider());

            //#region Hack to get default template and insert custom tag in <head> section
            //string relative_wwwroot_path_to_manifest = "/screens/manifest.json";
            //PartialToStandaloneHtmlProvider p = new PartialToStandaloneHtmlProvider();
            //FieldInfo field = typeof(PartialToStandaloneHtmlProvider).GetField("template", BindingFlags.NonPublic | BindingFlags.Instance);
            //string htmlTemplate = field.GetValue(p) as string;
            //int headPos = htmlTemplate.IndexOf("<head>");
            //if (headPos != -1)
            //{
            //    // Insert
            //    string linkManifest = string.Format(Environment.NewLine + "<link rel = \"manifest\" href = \"{0}\">" + Environment.NewLine, relative_wwwroot_path_to_manifest);
            //    htmlTemplate = htmlTemplate.Insert(headPos + 6, linkManifest);
            //}
            //Application.Current.Use(new PartialToStandaloneHtmlProvider(htmlTemplate));
            //#endregion


            Screens.Common.Utils.RegisterHooks();

            Handle.GET("/Screens/reset", (Request request) =>
            {
                MainPage mainPage = GetMainPage();
                mainPage.Cookie = null;

                Response respone = new Response();
                respone.Headers["Set-Cookie"] = "screenid=" + "" + ";Path=/;Expires=Wed, 21 Oct 2015 07:28:00 GMTT"; // TODO: Come up with a more clever way to set a cookie with no expire date
                respone.Headers["Location"] = "/Screens";
                respone.StatusCode = (ushort)System.Net.HttpStatusCode.TemporaryRedirect;
                return respone;
            });

            #region App Root
            Handle.GET("/Screens?{?}", (string query, Request request) =>
            {
                NameValueCollection collection = System.Web.HttpUtility.ParseQueryString(query);
                string guid = collection["setcookie"];

                Response respone = new Response();
                respone.Headers["Set-Cookie"] = "screenid=" + guid + ";Path=/;Expires=Wed, 2 Dec 2037 00:00:00 GMT"; // TODO: Come up with a more clever way to set a cookie with no expire date
                respone.Headers["Location"] = "/Screens";
                respone.StatusCode = (ushort)System.Net.HttpStatusCode.SeeOther;
                return respone;
            });

            Handle.GET("/Screens", (Request request) =>
            {
                MainPage mainPage = GetMainPage(request);

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

            Handle.GET("/Screens/signin", (Request request) =>
            {

                MainPage mainPage = GetMainPage(request);
                mainPage.Content = new SignInPage();
                return mainPage;
            });

            #endregion

            #region Screen

            Handle.GET("/Screens/screens", (Request request) =>
            {

                MainPage mainPage = GetMainPage(request);
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

                MainPage mainPage = GetMainPage(request);
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

                MainPage mainPage = GetMainPage(request);

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

            #region Blending

            Handle.GET("/Screens/screenContent/{?}", (string screenId) => { return new Json(); });
            Blender.MapUri("/Screens/screenContent/{?}", "screenContent");

            Handle.GET("/Screens/screenpluginmapping/{?}", (string screenId) => { return new Json(); });
            Blender.MapUri("/Screens/screenpluginmapping/{?}", "screen");

            #endregion
        }



        private static Cookie GetCookie(Request request, string name)
        {
            return request.Cookies.Select(x => new Cookie(x)).FirstOrDefault(x => x.Name == name);
        }


        public static MainPage GetMainPage(Request request = null)
        {
            var session = Session.Ensure();

            MainPage mainPage = session.Store[nameof(MainPage)] as MainPage;
            if (mainPage == null)
            {
                mainPage = new MainPage();
                session.Store[nameof(MainPage)] = mainPage;
            }

            // Set the cookie from request on the mainPage if any
            if (request != null)
            {
                Cookie screenCookie = GetCookie(request, "screenid");
                if (screenCookie != null && !string.IsNullOrEmpty(screenCookie.Value))
                {
                    mainPage.Cookie = screenCookie;
                }
            }

            return mainPage;
        }
    }
}