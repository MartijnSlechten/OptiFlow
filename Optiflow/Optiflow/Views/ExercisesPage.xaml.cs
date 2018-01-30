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
	public partial class ExercisesPage : ContentPage
	{
        public ObservableCollection<ImageCell> Items { get; set; }

        public ExercisesPage()
        {
            InitializeComponent();

            Items = new ObservableCollection<ImageCell>
            {
                new ImageCell
                {
                    ImageSource = "icon7.png",
                    Text = "Exercise 1"
                },
                new ImageCell
                {
                    ImageSource = "icon8.png",
                    Text = "Exercise 2"
                },
                new ImageCell
                {
                    ImageSource = "icon9.png",
                    Text = "Exercise 3"
                }
            };

            Exercises.ItemsSource = Items;
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            await Navigation.PushAsync(new NavigationPage(new ExercisePage()));

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }
    }
}