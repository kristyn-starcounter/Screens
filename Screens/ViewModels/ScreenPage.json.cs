using Starcounter;
using System;
using System.Collections.Generic;
using Screens.Common;

namespace Screens.ViewModels
{
    partial class ScreenPage : Json, IBound<Screen>
    {
        public void Init()
        {
            this.PluginsContent = Self.GET("/Screens/screenpluginmapping/" + this.Data.GetObjectID());
        }

        public IEnumerable<ScreenTempCode> ScreenCodes => Db.SQL<ScreenTempCode>("SELECT o FROM Screens.Common.ScreenTempCode o WHERE o.Screen = ? ORDER BY o.Expires", this.Data);
        
        public string UrlString => string.Format("/Screens/screens/{0}", this.Data?.GetObjectID());
    
        public void Handle(Input.GenerateScreenCodeTrigger action)
        {
            Db.Transact(() =>
            {
                ScreenTempCode screenCode = new ScreenTempCode();
                screenCode.Code = GenerateRandomScreenCode();
                screenCode.Screen = this.Data;
                screenCode.Expires = DateTime.UtcNow.AddHours(1); // TODO: Expire time 1 hour?
            });
        }

        public void Handle(Input.SaveTrigger action)
        {
            this.Transaction.Commit();
            this.RedirectUrl = "/Screens/screens";
        }

        public void Handle(Input.CloseTrigger action)
        {
            this.Transaction.Rollback();
            this.RedirectUrl = "/Screens/screens";
        }


        public void Handle(Input.DeleteTrigger action)
        {

            MessageBoxButton deleteButton = new MessageBoxButton() { ID = (long)MessageBox.MessageBoxResult.Yes, Text = "Delete", CssClass = "btn btn-sm btn-danger" };
            MessageBoxButton cancelButton = new MessageBoxButton() { ID = (long)MessageBox.MessageBoxResult.Cancel, Text = "Cancel" };

            MessageBox.Show("Delete Screen", "This Screen will be deleted.", cancelButton, deleteButton, (result) =>
            {
                if (result == MessageBox.MessageBoxResult.Yes)
                {
                    // TODO: how to check that this.Data can be deleted (if it has not been commited?)
                    Db.Transact(() =>
                    {
                        this.Data.Delete();
                    });

                    this.RedirectUrl = "/Screens/screens";  // TODO: This does not work!. (maybe of some commit-hooks activity?)
                }
            });
        }

        public string GenerateRandomScreenCode()
        {
            int min = 1000;
            int max = 9999;
            Random rnd = new Random();
            return rnd.Next(min, max).ToString().PadLeft(4, '0');
        }
    }

    [ScreenPage_json.ScreenCodes]
    partial class ScreenPageScreenTempoCode : Json, IBound<ScreenTempCode>
    {

        public void Handle(Input.DeleteTrigger action)
        {
            Db.Transact(() => this.Data.Delete());
        }
    }

}
