using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lunohod.Objects
{
    public interface IRunnable
    {
        bool InProgress { get; set; }

        void Start();
        void Stop();
        void Pause();
        void Resume();
    }
}
