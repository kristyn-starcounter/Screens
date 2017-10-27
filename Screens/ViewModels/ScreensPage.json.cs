using Starcounter;
using System.Collections.Generic;
using Screens.Common;

namespace Screens.ViewModels
{
    partial class ScreensPage : Json
    {
        public IEnumerable<Screen> Screens => Db.SQL<Screen>("SELECT o.Screen FROM Screens.Common.UserScreenRelation o WHERE o.User = ? ORDER BY o.Screen.Name", UserSession.GetSignedInUser());
    }

    [ScreensPage_json.Screens]
    partial class ScreensPageRoomItem : Json, IBound<Screen>
    {

        public string Url => string.Format("/Screens/screens/{0}", this.Data?.GetObjectID());

        public void Handle(Input.DeleteTrigger action)
        {

            MessageBoxButton deleteButton = new MessageBoxButton() { ID = (long)MessageBox.MessageBoxResult.Yes, Text = "Delete", CssClass = "btn btn-sm btn-danger" };
            MessageBoxButton cancelButton = new MessageBoxButton() { ID = (long)MessageBox.MessageBoxResult.Cancel, Text = "Cancel" };

            MessageBox.Show("Remove Screen", "This Screen will be removed.", cancelButton, deleteButton, (result) => {

                if (result == MessageBox.MessageBoxResult.Yes)
                {
                    Db.Transact(() => {
                        this.Data.Delete();
                    });
                }
            });
        }
    }
}
