using System;
namespace Resteeny
{
    public interface ITimer
    {
        bool Enabled { get; }
        TimeSpan ScheduledRestTimeInterval { get; set; }
        event EventHandler Elapsed;
        void Start();
        void Stop();
    }
}
