using Starcounter;
using Starcounter.Templates;
using System;
using Screens.Common;

namespace Screens.ViewModels
{
    partial class MainPage : Json
    {
        public Cookie Cookie { get; set; }

        public string SignInRedirectUrl;
    }


    [MainPage_json.User]
    partial class MainPageUser : Json, IBound<User>
    {
        public bool SignedIn => this.Data != null;
    }


    [MainPage_json.GoogleUser]
    partial class GoogleUserItem : Json
    {
        public void Handle(Input.SignedIn action)
        {

            MainPage mainPage = Program.GetMainPage();

            if (action.Value)
            {
                // google user signed in

                User user = Db.SQL<User>("SELECT o FROM Screens.Common.User o WHERE o.GoogleId = ?", this.Id).First;

                if (user == null)
                {
                    // Create user
                    Db.Transact(() =>
                    {
                        user = new User();
                        user.GoogleId = this.Id;
                        user.FirstName = this.FirstName;
                        user.LastName = this.LastName;
                        user.Email = this.Email;
                    });
                }

                // Update values
                if (!string.Equals(user.Email, this.Email, StringComparison.CurrentCultureIgnoreCase))
                {
                    Db.Transact(() =>
                    {
                        user.Email = this.Email;
                    });
                }
                if (!string.Equals(user.FirstName, this.FirstName, StringComparison.CurrentCultureIgnoreCase))
                {
                    Db.Transact(() =>
                    {
                        user.FirstName = this.FirstName;
                    });
                }
                if (!string.Equals(user.LastName, this.LastName, StringComparison.CurrentCultureIgnoreCase))
                {
                    Db.Transact(() =>
                    {
                        user.LastName = this.LastName;
                    });
                }

//                mainPage.User.Data = user;

                UserSession.SignInUser(user);

                mainPage.User.Data = UserSession.GetSignedInUser();


                if (!string.IsNullOrEmpty(mainPage.SignInRedirectUrl))
                {
                    mainPage.RedirectUrl = mainPage.SignInRedirectUrl;
                }
            }
            else
            {
                UserSession.SignOutUser(mainPage.User.Data);

                // signed out or was never signed in
                mainPage.User.Data = null;
                mainPage.RedirectUrl = "/Screens";
            }
        }
    }

}
