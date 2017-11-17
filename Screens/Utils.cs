using Screens.ViewModels;
using Starcounter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Screens
{
    public class Utils
    {
        /// <summary>
        /// Get app main page
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static MainPage GetMainPage(Request request = null)
        {
            var session = Session.Ensure();

            MainPage mainPage = session.Store[nameof(MainPage)] as MainPage;
            if (mainPage == null)
            {
                mainPage = new MainPage();

                // Menu blending point
                mainPage.Menu = Self.GET("/Screens/menumapping");

                session.Store[nameof(MainPage)] = mainPage;
            }

            // Set the cookie from request on the mainPage if any
            if (request != null)
            {
                Cookie screenCookie = GetCookie(request, Program.CookieName);
                if (screenCookie != null && !string.IsNullOrEmpty(screenCookie.Value))
                {
                    mainPage.Cookie = screenCookie;
                }
            }

            return mainPage;
        }

        /// <summary>
        /// Retrive cookie from request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static Cookie GetCookie(Request request, string name)
        {
            return request.Cookies.Select(x => new Cookie(x)).FirstOrDefault(x => x.Name == name);
        }

    }
}
