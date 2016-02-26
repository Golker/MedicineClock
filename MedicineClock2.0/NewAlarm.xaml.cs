using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace MedicineClock2._0
{
    public partial class NewAlarm : PhoneApplicationPage
    {
        public NewAlarm()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            
            // TODO: Actually create the alarm
            MessageBoxResult result = MessageBox.Show("The alarm has been created!");
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }
    }
}