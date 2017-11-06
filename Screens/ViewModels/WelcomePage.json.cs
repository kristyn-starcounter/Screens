using Starcounter;
using System;
using System.Collections.Generic;
using Screens.Common;
using System.Linq;

namespace Screens.ViewModels
{
    partial class WelcomePage : Json
    {

        public void Handle(Input.ConnectScreenTrigger action)
        {
            this.Message = "";

            Screen screen = Db.SQL<Screen>("SELECT o.Screen FROM Screens.Common.ScreenTempCode o WHERE o.Code = ? AND o.Expires >= ?", this.ScreenCode, DateTime.UtcNow).FirstOrDefault();
            if (screen == null)
            {
                // Invalid code or expired one
                this.Message = "Invalid screen code";
                return; // TODO: Show user "Invalid string"
            }

            // Generate cookie
            string guid = Guid.NewGuid().ToString();

            Db.Transact(() =>
            {
                screen.CookieValue = guid;
                screen.LastAccess = DateTime.UtcNow;

                // Remove used code
                Db.SQL("DELETE FROM Screens.Common.ScreenTempCode WHERE Code = ?", this.ScreenCode);
            });


            this.RedirectUrl = "/Screens?setcookie=" + guid;
        }

    }
}
