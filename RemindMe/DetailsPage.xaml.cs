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
using System.IO.IsolatedStorage;
using Microsoft.Phone.Tasks;

namespace RemindMe
{
    public partial class DetailsPage : PhoneApplicationPage
    {
        // Constructeur
        public DetailsPage()
        {
            InitializeComponent();

            // Exemple de code pour la localisation d'ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        // Lors de l'accès à la page, affectez l'élément sélectionné dans la liste au contexte de données
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (DataContext == null)
            {
                string selectedIndex = "";
                if (NavigationContext.QueryString.TryGetValue("selectedItem", out selectedIndex))
                {
                    int index = int.Parse(selectedIndex);
                    DataContext = App.ViewModel.Items[index];
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        { // The user clicked "I did it again"
            List<RemindEvent> evenements = (List<RemindEvent>)IsolatedStorageSettings.ApplicationSettings["events"];
            TextBlock id_block = this.FindName("page_ID") as TextBlock;
            int pageIndex = int.Parse(id_block.Text);

            evenements[pageIndex].lastTime = DateTime.Now;
            App.ViewModel.LoadData();
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void delete_action(object sender, EventArgs e)
        {
            List<RemindEvent> evenements = (List<RemindEvent>)IsolatedStorageSettings.ApplicationSettings["events"];
            TextBlock id_block = this.FindName("page_ID") as TextBlock;
            int pageIndex = int.Parse(id_block.Text);

            MessageBoxResult m = MessageBox.Show("Are you sure you want to delete this event?", "Event deletion", MessageBoxButton.OKCancel);

            if (m == MessageBoxResult.OK)
            {
                evenements.RemoveAt(pageIndex);
                App.ViewModel.LoadData();
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
        }

        private void add_calendar(object sender, EventArgs e)
        {
            MessageBoxResult m = MessageBox.Show("This will add the next occurence of this event to your calendar. Are you sure you want to continue?", "Calendar", MessageBoxButton.OKCancel);

            if (m == MessageBoxResult.OK)
            {
                List<RemindEvent> evenements = (List<RemindEvent>)IsolatedStorageSettings.ApplicationSettings["events"];
                TextBlock id_block = this.FindName("page_ID") as TextBlock;
                int pageIndex = int.Parse(id_block.Text);

                SaveAppointmentTask sat = new SaveAppointmentTask();
                sat.StartTime = evenements[pageIndex].lastTime.Add(evenements[pageIndex].repeatEvery);
                sat.Details = "Created by RemindMe";
                sat.Subject = evenements[pageIndex].label;
                //sat.EndTime = sat.StartTime + TimeSpan.FromHours(2);
                sat.Show();
            }
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
    }
}