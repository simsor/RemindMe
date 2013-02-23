using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using RemindMe.Resources;
using RemindMe.ViewModels;
using System.IO.IsolatedStorage;
using System.Diagnostics;
using Microsoft.Phone.Scheduler;

namespace RemindMe
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructeur
        public MainPage()
        {
            InitializeComponent();

            // Affecter l'exemple de données au contexte de données du contrôle LongListSelector
            DataContext = App.ViewModel;

            try
            {
               this.NavigationService.RemoveBackEntry();// This way, the user can't "back" to Add.xaml or DetailsPage.xaml
            }
            catch
            {
                // First run
            }

            // Exemple de code pour la localisation d'ApplicationBar
            //BuildLocalizedApplicationBar();

            /*PeriodicTask agent = (PeriodicTask)ScheduledActionService.Find("RemindAgent");
            if (agent == null)
            {
                StartPeriodicTask();
            }
            else
            {
                StopPeriodicTask();
                StartPeriodicTask();
            }*/
        }

        // Charger les données pour les éléments ViewModel
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
            NavigationService.RemoveBackEntry();
        }

        // Gérer la sélection modifiée sur LongListSelector
        private void MainLongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Si l'élément sélectionné a la valeur Null (pas de sélection), ne rien faire
            if (MainLongListSelector.SelectedItem == null)
                return;

            // Naviguer vers la nouvelle page
            NavigationService.Navigate(new Uri("/DetailsPage.xaml?selectedItem=" + (MainLongListSelector.SelectedItem as ItemViewModel).ID, UriKind.Relative));

            // Réinitialiser l'élément sélectionné sur Null (pas de sélection)
            MainLongListSelector.SelectedItem = null;
        }


        private void add_event(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Add.xaml", UriKind.Relative));
        }

        // Exemple de code pour la conception d'une ApplicationBar localisée
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Définit l'ApplicationBar de la page sur une nouvelle instance d'ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Crée un bouton et définit la valeur du texte sur la chaîne localisée issue d'AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Crée un nouvel élément de menu avec la chaîne localisée d'AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}

        // Manage the reminder task
        private void StartPeriodicTask()
        {
            Debug.WriteLine("Trying to open background agent...");
            PeriodicTask periodicTask = new PeriodicTask("RemindAgent");
            periodicTask.Description = "A task reminding the user of its events";
            try
            {
                ScheduledActionService.Add(periodicTask);
                ScheduledActionService.LaunchForTest("RemindAgent", TimeSpan.FromSeconds(3));
                Debug.WriteLine("Open the background agent success");
            }
            catch (InvalidOperationException exception)
            {
                if (exception.Message.Contains("exists already"))
                {
                    Debug.WriteLine("Since then the background agent success is already running");
                }
                if (exception.Message.Contains("BNS Error: The action is disabled"))
                {
                    Debug.WriteLine("Background processes for this application has been prohibited");
                }
                if (exception.Message.Contains("BNS Error: The maximum number of ScheduledActions of this type have already been added."))
                {
                    Debug.WriteLine("You open the daemon has exceeded the hardware limitations");
                }
                else
                    Debug.WriteLine("Launching the Agent failed: unknown InvalidOperationException occured.\n" + exception.Message);
            }
            catch (SchedulerServiceException)
            {

            }
        }
        private void StopPeriodicTask()
        {
            try
            {
                ScheduledActionService.Remove("RemindAgent");
                //Debug.WriteLine("Turn off the background agent successfully");
            }
            catch (InvalidOperationException exception)
            {
                if (exception.Message.Contains("doesn't exist"))
                {
                    Debug.WriteLine("Since then the background agent success is not running");
                }
            }
            catch (SchedulerServiceException)
            {

            }
        }
    }
}