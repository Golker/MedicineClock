using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MedicineClock2._0.Resources;

namespace MedicineClock2._0
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        private void btnNewAlarm_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/NewAlarm.xaml", UriKind.Relative));
        }

        private void btnSeeAlarms_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/AlarmList.xaml", UriKind.Relative));
        }

        private void btnInfo_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Thank you for using MedicineClock!\n\nThis app was developed by Luca Bezerra. " +
                "If you have any questions, complaints and/or suggestions, please feel free to mail me at " + 
                "lucabezerra@gmail.com, I'd love to hear from you!", "MedicineClock 1.2", MessageBoxButton.OK);
        }



        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}