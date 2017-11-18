using Screens.ViewModels;
using Starcounter;
using System.Linq;

namespace Screens
{
    public class ScreenHandlers
    {

        public static void Register()
        {

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

                    mainPage.Content = new ScreensPage();
                    return mainPage;
                }

                UserScreenRelation userScreenRelation = Db.SQL<UserScreenRelation>($"SELECT o FROM {typeof(UserScreenRelation)} o WHERE o.{nameof(UserScreenRelation.Screen)} = ? AND o.{nameof(UserScreenRelation.User)} = ?", screen,user).FirstOrDefault();
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

                mainPage.Content= Db.Scope<ScreenPage>(() =>
                {
                    UserScreenRelation userScreenRelation = new UserScreenRelation
                    {
                        Screen = new Screen(),
                        User = user
                    };

                    ScreenPage screenPage = new ScreenPage
                    {
                        Data = userScreenRelation
                    };

                    screenPage.Init();

                    return screenPage;
                });

                return mainPage;
            });
        }
    }
}
