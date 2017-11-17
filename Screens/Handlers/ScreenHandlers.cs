using Screens.ViewModels;
using Starcounter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Screens
{
    public class ScreenHandlers
    {

        public static void Register()
        {

            #region Screen

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

                Screen screen = Db.SQL<Screen>($"SELECT o FROM {typeof(Screen)} o WHERE o.ObjectID = ?", id).FirstOrDefault();
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

                MainPage mainPage = Utils.GetMainPage(request);

                User user = UserSession.GetSignedInUser();
                if (user == null)
                {
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
        }

    }
}
