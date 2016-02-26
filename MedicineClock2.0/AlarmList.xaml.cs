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
    public partial class AlarmList : PhoneApplicationPage
    {
        public AlarmList()
        {
            InitializeComponent();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this alarm?", "Delete Alarm", MessageBoxButton.OKCancel);
            if(result == MessageBoxResult.OK)
            {
                MessageBox.Show("Alarm deleted successfully.");
                // TODO: Actually delete the alarm
            }
        }
    }
}