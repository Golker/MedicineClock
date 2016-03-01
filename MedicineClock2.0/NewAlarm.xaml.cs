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

namespace MedicineClock2._0
{
    public partial class NewAlarm : PhoneApplicationPage
    {
        string alarmName = null;

        public NewAlarm()
        {
            InitializeComponent();
            btnSave.IsEnabled = false;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // if the user has arrived on this page from editing an existent alarm/reminder and NOT from a 
            // DatePicker or TimePicker page...
            if (e.NavigationMode != NavigationMode.Back && NavigationContext.QueryString.TryGetValue("name", out alarmName))
            {
                try
                {
                    Reminder reminder = (Reminder)ScheduledActionService.Find(alarmName);

                    beginTimePicker.Value = (DateTime)reminder.BeginTime;

                    // 'Once' comes checked by default
                    RecurrenceInterval recurrence = reminder.RecurrenceType;
                    if (recurrence == RecurrenceInterval.Daily)
                    {
                        rbtnDaily.IsChecked = true;
                    }
                    else if (recurrence == RecurrenceInterval.Weekly)
                    {
                        rbtnWeekly.IsChecked = true;                        
                    }
                    else if (recurrence == RecurrenceInterval.Monthly)
                    {                        
                        rbtnMonthly.IsChecked = true;
                    }

                    tbxName.Text = reminder.Title;
                    tbxDetails.Text = reminder.Content;

                    btnSave.IsEnabled = true;
                }
                catch(SchedulerServiceException)
                {
                    MessageBox.Show("There was some error while loading the selected alarm. Please delete it and create a new one");
                    NavigationService.Navigate(new Uri("/AlarmList.xaml", UriKind.Relative));
                }                
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (alarmName != null && ScheduledActionService.Find(alarmName) != null)
            {
                // if there's a name already, it means it's a pre-existent alarm being editted
                // so we delete the old one first
                ScheduledActionService.Remove(alarmName);
            }

            // Generate a unique name for the new notification
            alarmName = System.Guid.NewGuid().ToString();

            DateTime date = (DateTime)beginDatePicker.Value;
            DateTime time = (DateTime)beginTimePicker.Value;
            DateTime beginTime = date + time.TimeOfDay;

            RecurrenceInterval recurrence = RecurrenceInterval.None;
            if (rbtnDaily.IsChecked == true)
            {
                recurrence = RecurrenceInterval.Daily;
            }
            else if (rbtnWeekly.IsChecked == true)
            {
                recurrence = RecurrenceInterval.Weekly;
            }
            else if (rbtnMonthly.IsChecked == true)
            {
                recurrence = RecurrenceInterval.Monthly;
            }

            Reminder reminder = new Reminder(alarmName);
            reminder.Title = tbxName.Text;
            reminder.Content = tbxDetails.Text;
            reminder.BeginTime = beginTime;
            reminder.RecurrenceType = recurrence;

            try
            {
                ScheduledActionService.Add(reminder);

                MessageBoxResult result = MessageBox.Show("The alarm has been saved!");
                NavigationService.Navigate(new Uri("/AlarmList.xaml", UriKind.Relative));
            }
            catch (Exception)
            {
                MessageBoxResult result = MessageBox.Show("The alarm could not be created, please check if the info is correct.\n" +
                    "Tip: The start date and time can't be earlier than the current time.\n");
            }
        }

        private void tbxName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbxName.Text == string.Empty)
            {
                btnSave.IsEnabled = false;
            }
            else
            {
                btnSave.IsEnabled = true;
            }
        }
    }
}