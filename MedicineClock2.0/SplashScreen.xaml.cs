using System;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using System.Threading;
using System.Threading.Tasks;

namespace MedicineClock
{
    public partial class SplashScreen : PhoneApplicationPage
    {
        public SplashScreen()
        {
            InitializeComponent();
            LoadNextScreen();
        }

        async Task LoadNextScreen()
        {            
            await Task.Delay(1300);
            NavigationService.Navigate(new Uri("/AlarmList.xaml", UriKind.Relative));
        }
    }
}