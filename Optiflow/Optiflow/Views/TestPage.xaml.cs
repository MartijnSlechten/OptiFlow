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
	public partial class TestPage : ContentPage
	{
		public TestPage ()
		{
			InitializeComponent ();
            fan.Source = ImageSource.FromFile("fan.png");
        }

        async void Test_Clicked(object sender, EventArgs e)
        {
            await fan.RotateTo(10000,10000, Easing.SinIn);
        }
    }
}