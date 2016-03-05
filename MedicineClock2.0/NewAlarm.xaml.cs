using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
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

            lpkrRecurrence.ItemsSource = RecurrenceSubItems();
        }

        private IEnumerable<string> RecurrenceSubItems()
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
            if (alarmName != null && ScheduledActionService.Find(alarmName) != null)
            {
                // if there's a name already, it means it's a pre-existent alarm being editted
                // so we delete the old one first
                ScheduledActionService.Remove(alarmName);
            }

            // generates a unique name for the new alarm
            alarmName = Guid.NewGuid().ToString();

            try
            {
                // creates the main alarm
                CreateReminderObject(alarmName, beginDatePicker1, beginTimePicker1);

                // if extra alarm times were set, creates similar alarms for each of those times
                foreach (UIElement child in spnlTimes.Children)
                {
                    StackPanel sp = (StackPanel)child;
                    DatePicker dp = (DatePicker)sp.Children[1]; // 2nd element, the DatePicker
                    TimePicker tp = (TimePicker)sp.Children[2]; // 3rd element, the TimePicker

                    string extraAlarmName = Guid.NewGuid().ToString();
                    CreateReminderObject(extraAlarmName, dp, tp, extra: true);                    
                }

                string msg = spnlTimes.Children.Count > 0 ? "The alarms have been saved!" : "The alarm has been saved!";

                MessageBoxResult result = MessageBox.Show(msg);
                NavigationService.Navigate(new Uri("/AlarmList.xaml", UriKind.Relative));
            }
            catch(Exception)
            {
                MessageBox.Show("One or more alarms could not be saved, " +
                    "please check if the info is correct.\n\n" + 
                    "If you're adding more than one alarm time for this medicine, " + 
                    "be aware that some of them might have been created before the issue was detected, so please check " + 
                    "it first to avoid duplicates. You can always create new alarms from an existing one, so don't worry " +
                    "about losing data.\n\n" + 
                    "Tip: The start date and time can't be earlier than the current time.\n",
                    "Error!", MessageBoxButton.OK);
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
            StackPanel sp = (StackPanel)(sender as Button).Parent;
            spnlTimes.Children.Remove(sp);
        }

        private void CreateReminderObject(string internalNameOrGuid, DatePicker datePicker, TimePicker timePicker, bool extra=false)
        {
            /// The 'extra' parameter indicates whether this is an additional alarm time or the main one
            
            DateTime date = (DateTime)datePicker.Value;
            DateTime time = (DateTime)timePicker.Value;
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

            // if it's an extra alarm time, add the hour to its name for ease of identification
            string title = !extra ? tbxName.Text : tbxName.Text + " [" + time.Hour.ToString() + "h]";

            Reminder reminder = new Reminder(internalNameOrGuid);
            reminder.Title = title;
            reminder.Content = tbxDetails.Text;
            reminder.BeginTime = beginTime;
            reminder.RecurrenceType = recurrence;
            
            ScheduledActionService.Add(reminder);
        }
    }    
}