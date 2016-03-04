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
using System.Windows.Media;

namespace MedicineClock
{
    public partial class NewAlarm : PhoneApplicationPage
    {
        string alarmName = null;

        public NewAlarm()
        {
            InitializeComponent();
            btnSave.IsEnabled = false;

            lpkrRecurrence.ItemsSource = SubItems();
        }

        private IEnumerable<string> SubItems()
        {
            yield return "Once";
            yield return "Daily";
            yield return "Weekly";
            yield return "Monthly";
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

                    beginTimePicker1.Value = (DateTime)reminder.BeginTime;

                    // 'Once' comes checked by default
                    RecurrenceInterval recurrence = reminder.RecurrenceType;
                    if (recurrence == RecurrenceInterval.Daily)
                    {
                        lpkrRecurrence.SelectedItem = "Daily";
                    }
                    else if (recurrence == RecurrenceInterval.Weekly)
                    {
                        lpkrRecurrence.SelectedItem = "Weekly";
                    }
                    else if (recurrence == RecurrenceInterval.Monthly)
                    {
                        lpkrRecurrence.SelectedItem = "Monthly";
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
            for (int i = 0; i < spnlTimes.Children.Count + 1; i++)
            {
                if (alarmName != null && ScheduledActionService.Find(alarmName) != null)
                {
                    // if there's a name already, it means it's a pre-existent alarm being editted
                    // so we delete the old one first
                    ScheduledActionService.Remove(alarmName);
                }

                // Generate a unique name for the new notification
                alarmName = System.Guid.NewGuid().ToString();

                DateTime date = (DateTime)beginDatePicker1.Value;
                DateTime time = (DateTime)beginTimePicker1.Value;
                DateTime beginTime = date + time.TimeOfDay;

                RecurrenceInterval recurrence = RecurrenceInterval.None;
                if ((lpkrRecurrence.SelectedItem as string) == "Daily")
                {
                    recurrence = RecurrenceInterval.Daily;
                }
                else if ((lpkrRecurrence.SelectedItem as string) == "Weekly")
                {
                    recurrence = RecurrenceInterval.Weekly;
                }
                else if ((lpkrRecurrence.SelectedItem as string) == "Monthly")
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
                    MessageBoxResult result = MessageBox.Show("The alarm could not be created, " +
                        "please check if the info is correct.\nTip: The start date and time " +
                        "can't be earlier than the current time.\n");
                }
            }
        }

        private void tbxName_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnSave.IsEnabled = tbxName.Text == string.Empty ? false : true;
        }

        private void btnAddTime_Click(object sender, RoutedEventArgs e)
        {
            StackPanel panel = new StackPanel();
            panel.Orientation = System.Windows.Controls.Orientation.Horizontal;            

            Button btnDelete = new Button();
            btnDelete.Width = 60;
            btnDelete.Content = "(X)";
            btnDelete.BorderThickness = new Thickness(0);
            Color currentAccentColorHex = (Color)Application.Current.Resources["PhoneAccentColor"];
            btnDelete.BorderBrush = new SolidColorBrush(currentAccentColorHex);            
            btnDelete.Padding = new Thickness(0);
            btnDelete.Click += btnDeleteExtraTime_Click;

            DatePicker dp = new DatePicker();
            dp.Width = 176;
            dp.HorizontalAlignment = HorizontalAlignment.Left;
            dp.Value = DateTime.Today;

            TimePicker tp = new TimePicker();
            tp.Width = 153;
            tp.HorizontalAlignment = HorizontalAlignment.Right;
            tp.Value = DateTime.Now;

            panel.Children.Add(btnDelete);
            panel.Children.Add(dp);
            panel.Children.Add(tp);

            spnlTimes.Children.Add(panel);
        }

        private void btnDeleteExtraTime_Click(object sender, RoutedEventArgs e)
        {
            spnlTimes.Children.Remove((StackPanel)(sender as Button).Parent);
        }
    }    
}