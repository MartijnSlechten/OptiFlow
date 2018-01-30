using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace Optiflow.Views
{
	public class TabbedMenuPage : TabbedPage
	{
		public TabbedMenuPage ()
		{
            Page homePage, devicesPage, statsPage, exercisesPage, testPage, settingsPage = null;

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    homePage = new NavigationPage(new HomePage())
                    {
                        Title = "Home",
                        Icon = "icon1.png"
                    };

                    devicesPage = new NavigationPage(new DevicesPage())
                    {
                        Title = "Devices",
                        Icon = "icon2.png"

                    };

                    statsPage = new NavigationPage(new StatsPage())
                    {
                        Title = "Stats",
                        Icon = "icon3.png"
                    };

                    exercisesPage = new NavigationPage(new ExercisesPage())
                    {
                        Title = "Exercises",
                        Icon = "icon10.png"
                    };

                    testPage = new NavigationPage(new TestPage())
                    {
                        Title = "Test",
                        Icon = "icon4.png"
                    };

                    settingsPage = new NavigationPage(new SettingsPage())
                    {
                        Title = "Settings",
                        Icon = "icon5.png"
                    };

                    break;
                default:
                    homePage = new HomePage()
                    {
                        Title = "Home",
                        Icon = "icon1.png"
                    };

                    devicesPage = new DevicesPage()
                    {
                        Title = "Devices",
                        Icon = "icon2.png"
                    };

                    statsPage = new StatsPage()
                    {
                        Title = "Stats",
                        Icon = "icon3.png"
                    };

                    exercisesPage = new NavigationPage(new ExercisesPage())
                    {
                        Title = "Exercises",
                        Icon = "icon10.png"
                    };

                    testPage = new TestPage()
                    {
                        Title = "Test",
                        Icon = "icon4.png"
                    };

                    settingsPage = new SettingsPage()
                    {
                        Title = "Settings",
                        Icon = "icon5.png"
                    };

                    break;
            }

            Children.Add(homePage);
            Children.Add(devicesPage);
            Children.Add(statsPage);
            Children.Add(exercisesPage);
            Children.Add(testPage);
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