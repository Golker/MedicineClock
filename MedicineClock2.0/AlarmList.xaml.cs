using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Scheduler;
using System.Collections.ObjectModel;

namespace MedicineClock2._0
{
    public partial class AlarmList : PhoneApplicationPage
    {
        ObservableCollection<Alarm> alarms;
        IEnumerable<ScheduledNotification> notifications;
        string selectedAlarmName;

        public AlarmList()
        {
            InitializeComponent();
            btnEdit.IsEnabled = false;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (NavigationService.BackStack.ElementAt(0).Source.OriginalString.Contains("NewAlarm"))
            {
                NavigationService.RemoveBackEntry();
            }
            selectedAlarmName = string.Empty;
            //Reset the ReminderListBox items when the page is navigated to.
            ResetItemsList();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/NewAlarm.xaml?name=" + selectedAlarmName, UriKind.Relative));
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/NewAlarm.xaml", UriKind.Relative));
        }

        private void ResetItemsList()
        {
            // Use GetActions to retrieve all of the scheduled actions
            // stored for this application.
            notifications = ScheduledActionService.GetActions<ScheduledNotification>();

            if (notifications.Count<ScheduledNotification>() > 0)
            {
                EmptyTextBlock.Visibility = Visibility.Collapsed;
            }
            else
            {
                EmptyTextBlock.Visibility = Visibility.Visible;
                btnEdit.IsEnabled = false;
            }

            // Update the ReminderListBox with the list of reminders.
            NotificationListBox.ItemsSource = notifications;
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete the " + (string)((Button)sender).Name + " alarm?", "Delete Alarm", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                // The scheduled action name is stored in the Tag property
                string name = (string)((Button)sender).Tag;

                ScheduledActionService.Remove(name);
                ResetItemsList();
                MessageBox.Show("Alarm deleted successfully.");
            }            
        }

        private void NotificationListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NotificationListBox.SelectedItem != null)
            {
                selectedAlarmName = (NotificationListBox.SelectedItem as Reminder).Name;
                btnEdit.IsEnabled = true;
            }
        }
    }
}