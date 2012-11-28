using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemindAgent
{
    public class RemindEvent
    {
        public String label { get; set; }
        public TimeSpan repeatEvery { get; set; }
        public DateTime lastTime { get; set; }

        public RemindEvent()
        { // Just to stop ApplicationSettings.Save() from complaining
        }
    }
}
