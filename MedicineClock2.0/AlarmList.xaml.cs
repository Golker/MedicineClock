using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Scheduler;
using System.ComponentModel;

namespace MedicineClock
{
    public partial class AlarmList : PhoneApplicationPage
    {
        IEnumerable<ScheduledNotification> alarms;
        string selectedAlarmName;

        ApplicationBarIconButton btnNew = new ApplicationBarIconButton();
        ApplicationBarIconButton btnEdit = new ApplicationBarIconButton();
        ApplicationBarIconButton btnInfo = new ApplicationBarIconButton();

        public AlarmList()
        {
            InitializeComponent();
            btnEdit.IsEnabled = false;

            AddAppBar();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.RemoveBackEntry();
            }

            selectedAlarmName = string.Empty;
            //Reset the ReminderListBox items when the page is navigated to.
            ResetItemsList();
        }

        private void ResetItemsList()
        {            
            alarms = ScheduledActionService.GetActions<ScheduledNotification>();

            if (alarms.Count<ScheduledNotification>() > 0)
            {
                tblkNoAlarms.Visibility = Visibility.Collapsed;
            }
            else
            {
                tblkNoAlarms.Visibility = Visibility.Visible;
                btnEdit.IsEnabled = false;
            }

            // Update the lbxAlarms with the list of reminders.
            lbxAlarms.ItemsSource = alarms;
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete the " + (string)((Button)sender).Name + " alarm?", "Delete Alarm", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                // The scheduled action name is stored in the Tag property
                string name = (string)((Button)sender).Tag;

                if (ScheduledActionService.Find(name) != null)
                {
                    ScheduledActionService.Remove(name);
                }
                ResetItemsList();
                MessageBox.Show("Alarm deleted successfully.");
            }            
        }

        private void lbxAlarms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbxAlarms.SelectedItem != null)
            {
                selectedAlarmName = (lbxAlarms.SelectedItem as Reminder).Name;
                btnEdit.IsEnabled = true;
            }
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            if (NavigationService.CanGoBack && NavigationService.BackStack.ElementAt(0).Source.OriginalString.Contains("AlarmList"))
            {
                NavigationService.RemoveBackEntry();
            }
            base.OnBackKeyPress(e);
        }

        private void AddAppBar()
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.Opacity = 1.0;
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = true;

            string bgTheme = GetPhoneBackgroundTheme();            

            btnNew.IconUri = new Uri("/Assets/AppBar/" + bgTheme + "/add.png", UriKind.Relative);
            btnNew.Text = "New Alarm";
            btnNew.Click += new EventHandler(btnNew_Click);
            ApplicationBar.Buttons.Add(btnNew);            
            
            btnEdit.IconUri = new Uri("/Assets/AppBar/" + bgTheme + "/edit.png", UriKind.Relative);
            btnEdit.Text = "Edit Alarm";                
            btnEdit.Click += new EventHandler(btnEdit_Click);
            ApplicationBar.Buttons.Add(btnEdit);

            btnInfo.IconUri = new Uri("/Assets/AppBar/" + bgTheme + "/questionmark.png", UriKind.Relative);
            btnInfo.Text = "Info";
            btnInfo.Click += new EventHandler(btnInfo_Click);
            ApplicationBar.Buttons.Add(btnInfo);
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/NewAlarm.xaml", UriKind.Relative));
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/NewAlarm.xaml?name=" + selectedAlarmName, UriKind.Relative));
        }

        private void btnInfo_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Thank you for using MedicineClock!\n\nThis app was developed by Luca Bezerra. " +
            "If you have any questions, complaints and/or suggestions, please feel free to mail me at " +
            "lucabezerra@gmail.com, I'd love to hear from you!\n\n" +

            "PS: Windows Phone doesn't allow an alarm's recurrence interval to be shorter than once a day, " +
            "so I've allowed multiple alarms to be created at once by simply adding more times when " +
            "creating/editting an alarm, hope it helps :)", "MedicineClock 1.2", MessageBoxButton.OK);
        }

        public string GetPhoneBackgroundTheme()
        {
            bool isDarkTheme = (double)Application.Current.Resources["PhoneDarkThemeOpacity"] > 0;
            if (isDarkTheme)
            {
                return "Light";
            }
            else
            {
                return "Dark";
            }
        }
    }
}