using Starcounter;
using Starcounter.Templates;
using System;
using System.Linq;

namespace Screens.ViewModels
{
    partial class MainPage : Json
    {
        public Cookie Cookie { get; set; }

        public string SignInRedirectUrl;
    }


}
