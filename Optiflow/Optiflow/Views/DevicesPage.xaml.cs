using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Optiflow.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DevicesPage : ContentPage
    {
        public ObservableCollection<string> Items { get; set; }

        public DevicesPage()
        {
            InitializeComponent();

            Items = new ObservableCollection<string>
            {
                "Device 1",
                "Device 2",
                "Device 3",
                "Device 4",
                "Device 5"
            };
			
			MyListView.ItemsSource = Items;
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            await DisplayAlert(e.Item.ToString(), "Loaded", "OK");

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }
    }
}
