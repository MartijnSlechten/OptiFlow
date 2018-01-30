using System;
using System.Collections.Generic;
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
		public ExercisesPage ()
		{
			InitializeComponent ();
            star.Source = ImageSource.FromFile("star.png");
        }

        async void Test_Clicked(object sender, EventArgs e)
        {
            await fan.RotateTo(10000, 10000, Easing.SinIn);
        }
    }
}