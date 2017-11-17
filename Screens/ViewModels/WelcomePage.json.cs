using Starcounter;
using System;
using System.Linq;

namespace Screens.ViewModels
{
    partial class WelcomePage : Json
    {

        public void Handle(Input.ScreenCode action)
        {

            this.Message = "";
        }


        public void Handle(Input.ConnectScreenTrigger action)
        {

            if( string.IsNullOrEmpty(this.ScreenCode)) {
                this.Message = "Enter a creen code";
                return;
            }

            this.Message = "";

            Screen screen = Db.SQL<Screen>($"SELECT o.{nameof(ScreenTempCode.Screen)} FROM {typeof(ScreenTempCode)} o WHERE o.{nameof(ScreenTempCode.Code)} = ? AND o.{nameof(ScreenTempCode.Expires)} >= ?", this.ScreenCode, DateTime.UtcNow).FirstOrDefault();
            if (screen == null)
            {
                // Invalid code or expired one
                this.Message = "Invalid screen code";
                return;
            }

            // Generate cookie
            string guid = Guid.NewGuid().ToString();

            Db.Transact(() =>
            {
                screen.CookieValue = guid;
                screen.LastAccess = DateTime.UtcNow;

                // Remove used code
                Db.SQL($"DELETE FROM {typeof(ScreenTempCode)} WHERE {nameof(ScreenTempCode.Code)} = ?", this.ScreenCode);
            });

            this.RedirectUrl = "/Screens?setcookie=" + guid;
        }

    }
}
