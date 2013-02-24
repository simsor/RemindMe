using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemindMe
{
    public class AddContext : INotifyPropertyChanged
    {
        private String eventName_Private;
        public string eventName {
            get
            {
                return eventName_Private;
            }
            set
            {
                if (value == eventName_Private)
                    return;
                eventName_Private = value;
                NotifyPropertyChanged("eventName");
            }
        }

        private String repeat_Private;
        public string repeat {
            get
            {
                return repeat_Private;
            }
            set
            {
                if (value == repeat_Private)
                    return;
                repeat_Private = value;
                NotifyPropertyChanged("repeat");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string nomPropriete)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(nomPropriete));
        }
    }
}
