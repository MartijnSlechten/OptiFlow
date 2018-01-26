using System;

using Xamarin.Forms;

namespace Optiflow.Views
{
    public class TabbedMenuPage : TabbedPage
    {
        public TabbedMenuPage()
        {
            Page homePage, settingsPage = null;

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    homePage = new NavigationPage(new HomePage())
                    {
                        Title = "Home"
                    };

                    settingsPage = new NavigationPage(new SettingsPage())
                    {
                        Title = "Settings"
                    };
                    homePage.Icon = "tab_feed.png";
                    settingsPage.Icon = "tab_about.png";
                    break;
                default:
                    homePage = new HomePage()
                    {
                        Title = "Home"
                    };

                    settingsPage = new SettingsPage()
                    {
                        Title = "Settings"
                    };
                    break;
            }

            Children.Add(homePage);
            Children.Add(settingsPage);

            Title = Children[0].Title;
        }

        protected override void OnCurrentPageChanged()
        {
            base.OnCurrentPageChanged();
            Title = CurrentPage?.Title ?? string.Empty;
        }
    }
}
