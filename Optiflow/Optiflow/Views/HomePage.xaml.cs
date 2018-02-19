using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Optiflow.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class HomePage : ContentPage
	{
        public ObservableCollection<string> HintItems { get; set; }
        public ObservableCollection<string> UpdateItems { get; set; }

        public HomePage ()
		{
			InitializeComponent ();
            chart1.Source = ImageSource.FromFile("chart1.jpg");

            HintItems = new ObservableCollection<string>
            {
                "Hint 1",
                "Hint 2",
                "Hint 3"
            };

            Hints.ItemsSource = this.HintItems;

            UpdateItems = new ObservableCollection<string>
            {
                "Update 1",
                "Update 2",
                "Update 3"
            };

            Updates.ItemsSource = UpdateItems;
        }
	}
}