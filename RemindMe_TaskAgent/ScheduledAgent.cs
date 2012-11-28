using System;
using System.Diagnostics;
using System.Windows;
using Microsoft.Phone.Scheduler;
using System.IO.IsolatedStorage;
using System.Threading;
using System.Collections.Generic;
using RemindAgent;
using Microsoft.Phone.Shell;

namespace RemindMe_TaskAgent
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        /// <remarks>
        /// Le constructeur ScheduledAgent initialise le gestionnaire UnhandledException
        /// </remarks>
        static ScheduledAgent()
        {
            // S'abonner au gestionnaire d'exceptions prises en charge
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                Application.Current.UnhandledException += UnhandledException;
            });
        }

        /// Code à exécuter sur les exceptions non gérées
        private static void UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // Une exception non gérée s'est produite ; arrêt dans le débogueur
                Debugger.Break();
            }
        }

        /// <summary>
        /// Agent qui exécute une tâche planifiée
        /// </summary>
        /// <param name="task">
        /// La tâche appelée
        /// </param>
        /// <remarks>
        /// Cette méthode est appelée lorsqu'une tâche périodique ou  resource intensive est appelée
        /// </remarks>
        protected override void OnInvoke(ScheduledTask task)
        {
            if (task.Name.Equals("RemindAgent", StringComparison.OrdinalIgnoreCase))
            {
                Debug.WriteLine("MESSAGE FROM AGENT: Called!");



               Mutex mutex = new Mutex(true, "RemindAgentData");
                mutex.WaitOne();
                IsolatedStorageSettings setting = IsolatedStorageSettings.ApplicationSettings;
                List<RemindEvent> events = (List<RemindEvent>)setting["events"];
                mutex.ReleaseMutex(); 

                int i = 0;
                bool Alert = false;
                while (i < events.Count)
                {
                    DateTime lastTime = events[i].lastTime;
                    TimeSpan repeatEvery = events[i].repeatEvery;
                    double repeatEverySeconds = repeatEvery.TotalSeconds;
                    TimeSpan timeLeft = DateTime.Now - (lastTime.Add(repeatEvery));
                    double timeLeftSeconds = timeLeft.TotalSeconds;
                    double eventComingSoon = (10 / 100) * repeatEverySeconds;

                    if (timeLeft.TotalSeconds <= 0)
                        Alert = true; // In this case, the user missed the event :(
                    if (timeLeftSeconds < eventComingSoon)
                        Alert = true; // If we are 10% away from the next "milestone", we warn the user

                    i++;
                }

                if (Alert)
                {
                    ShellToast toast = new ShellToast();
                    toast.Title = "REMINDME";
                    toast.Content = "You have events coming soon!";
                    toast.NavigationUri = new Uri("/MainPage.xaml", UriKind.Relative);
                    toast.Show();
                    Debug.WriteLine("EVENTS COMING");
                } 
             }

            ScheduledActionService.LaunchForTest(task.Name, TimeSpan.FromSeconds(3));
            NotifyComplete();
        }
    }
}