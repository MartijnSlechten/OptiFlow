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
	public partial class StatsPage : ContentPage
	{
		public StatsPage ()
		{
			InitializeComponent ();
            chart1.Source = ImageSource.FromFile("chart1.jpg");
        }
    }
}