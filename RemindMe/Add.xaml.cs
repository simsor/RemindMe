using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Scheduler;

namespace RemindMe
{
    public partial class Add : PhoneApplicationPage
    {
        private AddContext contexte;
        public Add()
        {
            InitializeComponent();

            contexte = new AddContext { eventName = "Name", repeat = "1" };
            DataContext = contexte;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        { // We clicked the "add" button
            if (String.IsNullOrEmpty(contexte.eventName) || String.IsNullOrEmpty(contexte.repeat))
            {
                MessageBox.Show("All fields are required");
            }

            PivotItem currentItem = (PivotItem)pivotRepeat.SelectedItem;
            String repeatName = (String)currentItem.Header;

            TimeSpan repeatTime = TimeSpan.FromHours(24);
            try
            {
                if (repeatName == "hours")
                    repeatTime = TimeSpan.FromHours(int.Parse(contexte.repeat));
                else if (repeatName == "days")
                    repeatTime = TimeSpan.FromDays(int.Parse(contexte.repeat));
                else if (repeatName == "months")
                    repeatTime = TimeSpan.FromDays(int.Parse(contexte.repeat) * 30);
                else if (repeatName == "years")
                    repeatTime = TimeSpan.FromDays(int.Parse(contexte.repeat) * 365.15);
            }
            catch (System.FormatException)
            {
                MessageBox.Show("Only integers are allowed");
            }

            RemindEvent newEvent = new RemindEvent();
            newEvent.label = contexte.eventName;
            newEvent.repeatEvery = repeatTime;
            newEvent.lastTime = DateTime.Now;

            List<RemindEvent> evenements = (List<RemindEvent>)IsolatedStorageSettings.ApplicationSettings["events"];
            evenements.Add(newEvent);

            IsolatedStorageSettings.ApplicationSettings.Save();

            App.ViewModel.LoadData();
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative)); // We added the event, going back to the "list" page
            
        }
    }
}