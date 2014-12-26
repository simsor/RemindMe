using System;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Collections.ObjectModel;
using System.ComponentModel;
using RemindMe.Resources;
using System.Collections.Generic;
using System.Threading;

namespace RemindMe.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            this.Items = new ObservableCollection<ItemViewModel>();
        }

        /// <summary>
        /// Collection pour les objets ItemViewModel.
        /// </summary>
        public ObservableCollection<ItemViewModel> Items { get; private set; }

        private string _sampleProperty = "Sample Runtime Property Value";
        /// <summary>
        /// Exemple de propriété ViewModel ; cette propriété est utilisée dans la vue pour afficher sa valeur à l'aide d'une liaison
        /// </summary>
        /// <returns></returns>
        public string SampleProperty
        {
            get
            {
                return _sampleProperty;
            }
            set
            {
                if (value != _sampleProperty)
                {
                    _sampleProperty = value;
                    NotifyPropertyChanged("SampleProperty");
                }
            }
        }

        /// <summary>
        /// Exemple de propriété qui retourne une chaîne localisée
        /// </summary>
        public string LocalizedSampleProperty
        {
            get
            {
                return AppResources.SampleProperty;
            }
        }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        /// <summary>
        /// Crée et ajoute quelques objets ItemViewModel dans la collection Items.
        /// </summary>
        public void LoadData()
        {
            List<ItemViewModel> temporary_model = new List<ItemViewModel>();
            this.Items.Clear();
            Mutex mutex = new Mutex(false, "RemindAgentData");
            mutex.WaitOne();

            if (!IsolatedStorageSettings.ApplicationSettings.Contains("events"))
            {
                IsolatedStorageSettings.ApplicationSettings["events"] = new List<RemindEvent>();
                List<RemindEvent> evenements_ = (List<RemindEvent>)IsolatedStorageSettings.ApplicationSettings["events"];
                evenements_.Add(new RemindEvent());
                evenements_[0].label = "Example event";
                evenements_[0].lastTime = DateTime.Now;
                evenements_[0].repeatEvery = TimeSpan.FromHours(24);
                //IsolatedStorageSettings.ApplicationSettings.Save();
            }

            List<RemindEvent> evenements = (List<RemindEvent>)IsolatedStorageSettings.ApplicationSettings["events"];
            mutex.ReleaseMutex();
            //this.Items.Add(new ItemViewModel() { ID = "0", LineOne = "runtime one", LineTwo = "Maecenas praesent accumsan bibendum", LineThree = "Facilisi faucibus habitant inceptos interdum lobortis nascetur pharetra placerat pulvinar sagittis senectus sociosqu" });
            for (int i = 0; i < evenements.Count; i++)
            {
                TimeSpan timeDiff = DateTime.Now.Subtract(evenements[i].lastTime);
                String finalLastTime = "";
                String shortLastTime = "";
                String finalRepeatEvery = "";
                if (timeDiff.TotalDays < 1)
                {
                    // TIMEDIFF CALCULATION
                    if (timeDiff.TotalHours < 1)
                    {
                        if (timeDiff.TotalSeconds < 60) {
                            finalLastTime = evenements[i].lastTime.ToString() + "\n" + "This was " + Math.Round(timeDiff.TotalSeconds) + "s ago";
                            shortLastTime = Math.Round(timeDiff.TotalSeconds) + " s";
                        }
                        else {
                            finalLastTime = evenements[i].lastTime.ToString() + "\n" + "This was " + Math.Round(timeDiff.TotalSeconds / 60) + "min " + Math.Round(timeDiff.TotalSeconds - Math.Round(timeDiff.TotalSeconds / 60) * 60) + "s ago";
                            shortLastTime = Math.Round(timeDiff.TotalSeconds / 60) + " min";
                        }
                    }
                    else {
                        finalLastTime = evenements[i].lastTime.ToString() + "\n" + "This was " + Math.Round(timeDiff.TotalHours, 1) + "h " + (Math.Round(timeDiff.TotalSeconds / 60) - Math.Round(timeDiff.TotalHours, 1) * 60) + "min " + Math.Round(timeDiff.TotalSeconds - Math.Round(timeDiff.TotalSeconds / 60) * 60) + "s ago";
                        shortLastTime = Math.Round(timeDiff.TotalHours, 1) + " h";
                    }
                    // REPEATEVERY CALCULATION
                    if (evenements[i].repeatEvery.TotalDays < 1)
                        finalRepeatEvery = evenements[i].repeatEvery.TotalHours.ToString() + "h";
                    else
                    {
                        if (evenements[i].repeatEvery.TotalDays < 30)
                            finalRepeatEvery = evenements[i].repeatEvery.TotalDays.ToString() + " days";
                        else
                        {
                            if (evenements[i].repeatEvery.TotalDays < 365)finalRepeatEvery = Math.Round(evenements[i].repeatEvery.TotalDays / 30, 1).ToString() + " months";
                            else
                                finalRepeatEvery = Math.Round(evenements[i].repeatEvery.TotalDays / 365, 1).ToString() + " years";
                        }
                    }
                }
                else
                    finalLastTime = evenements[i].lastTime.ToString() + "\n" + "This was " + timeDiff.TotalDays + " days ago";

                temporary_model.Add(new ItemViewModel() { ID = i.ToString(), LineOne = evenements[i].label, LineTwo = shortLastTime + " ago, every " + finalRepeatEvery, LineThree = finalLastTime, LastTime = evenements[i].lastTime, RepeatEvery = evenements[i].repeatEvery });
            }

            while (temporary_model.Count > 0)
            {
                long closestTime = 999999999999999999;
                int closestTimeId = 0;
                for (int j = 0; j < temporary_model.Count; j++)
                {
                    if ((temporary_model[j].LastTime.Ticks + temporary_model[j].RepeatEvery.Ticks) < closestTime)
                    {
                        closestTime = temporary_model[j].LastTime.Ticks + temporary_model[j].RepeatEvery.Ticks;
                        closestTimeId = j;
                    }
                }
                this.Items.Add(temporary_model[closestTimeId]);
                temporary_model.RemoveAt(closestTimeId);
            }

            this.IsDataLoaded = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}