﻿//using Acr.UserDialogs;
using Realms;
using Realms.Sync;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

using Credentials = Realms.Sync.Credentials;
namespace Realm3
{
    public class ItemEntriesViewModel : INotifyPropertyChanged
    {
        private Realm _realm;
        private IEnumerable<Item> _entries;
        public IEnumerable<Item> Entries
        {
            get { return _entries; }
            private set
            {
                // Do nothing if there is no actual change.
                if (_entries == value)
                {
                    return;
                }
                _entries = value;
                // Let the UI know a change has occurred.
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Entries)));
            }
        }

        public ICommand LogOutCommand { get; private set; }

        public ICommand DeleteEntryCommand { get; private set; }

        public ICommand AddEntryCommand { get; private set; }

        private void DeleteEntry(Item entry) => _realm.Write(() => _realm.Remove(entry));

        public event PropertyChangedEventHandler PropertyChanged;
        public ItemEntriesViewModel()
        {
            LogOutCommand = new Command(() =>
            {
                if (User.Current == null)
                {
                    return;
                }
                User.Current.LogOutAsync().IgnoreResult();
                _realm = null;
                Entries = null;
                StartLoginCycle().IgnoreResult();
            });

            DeleteEntryCommand = new Command<Item>(DeleteEntry);
            AddEntryCommand = new Command(() => AddEntry().IgnoreResult());

            StartLoginCycle().IgnoreResult();
        }

        private async Task StartLoginCycle()
        {
            // Yield first in case this is called from the constructor, when not
            // everything will be loaded and the dialog may not show.
            do
            {
                await Task.Yield();
            } while (!await LogIn());
        }
        private async Task<bool> LogIn()
        {
            try
            {
                /*
                var users = User.AllLoggedIn;

                foreach (User user1 in users)
                {
                    await user1.LogOutAsync();
                }
                */
                var user = User.Current;
                
                if (user == null)
                {
                    // Not already logged in.
                    /*var loginResult = await UserDialogs.Instance.LoginAsync("Log in", "Enter a username and password");

                    if (!loginResult.Ok)
                    {
                        return false;
                    }*/

                    // Create credentials with the given username and password.
                    // Leaving the third parameter null allows a user to be registered
                    // if one does not already exist for this username.
                    //var credentials = Realms.Sync.Credentials.UsernamePassword(loginResult.LoginText, loginResult.Password);
                    await Application.Current.MainPage.DisplayAlert("Credenciales", "", "OK2");
                    var credentials = Realms.Sync.Credentials.UsernamePassword("Fabricio", "M3l1t45");



                    await Application.Current.MainPage.DisplayAlert("LoginAsync", "", "OK2");
                    user = await User.LoginAsync(credentials, new Uri(Constants.AuthUrl));
                }

                await Application.Current.MainPage.DisplayAlert("FullSyncConfiguration", "", "OK2");
                var configuration = new FullSyncConfiguration(new Uri(Constants.RealmPath, UriKind.Relative), user);

                await Application.Current.MainPage.DisplayAlert("GetInstance", "", "OK2");                
                //_realm = await Realm.GetInstanceAsync(configuration);
                _realm = Realm.GetInstance(configuration);

                /*
                RealmConfiguration.DefaultConfiguration = new FullSyncConfiguration(new Uri(Constants.AuthUrl));

                var _realm = Realm.GetInstance();

                RealmConfiguration.DefaultConfiguration = new FullSyncConfiguration(new Uri(Constants.AuthUrl));

                var realm = await Realm.GetInstanceAsync();
                */

                // Get the list of items.
                Entries = _realm.All<Item>().OrderBy(i => i.Timestamp);

                Console.WriteLine("Login successful.");

                return true;
            }
            catch (Exception ex)
            {
                // Display the error message.
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK2");
                return false;
            }
        }

        private async Task AddEntry()
        {
            /*var result = await UserDialogs.Instance.PromptAsync(new PromptConfig
            {
                Title = "New entry",
                Message = "Specify the item text",
            });

            if (result.Ok)
            {
                _realm.Write(() =>
                {
                    _realm.Add(new Item
                    {
                        Timestamp = DateTimeOffset.Now,
                        Body = result.Value,
                    });
                });
            }*/
        }


    }

}
