using Starcounter;
using System.Collections.Generic;

namespace Screens.ViewModels
{
    partial class ScreensPage : Json
    {
        public IEnumerable<Screen> Screens => Db.SQL<Screen>($"SELECT o.{nameof(UserScreenRelation.Screen)} FROM {typeof(UserScreenRelation)} o WHERE o.{nameof(UserScreenRelation.User)} = ? ORDER BY o.{nameof(UserScreenRelation.Screen)}.{nameof(Screen.Name)}", UserSession.GetSignedInUser());
    }

    [ScreensPage_json.Screens]
    partial class ScreensPageRoomItem : Json, IBound<Screen>
    {

        public string Url => string.Format("/Screens/screens/{0}", this.Data?.GetObjectID());
    }
}
