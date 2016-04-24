using System;
using System.Threading;

namespace Resteeny
{
    /// <summary>
    /// Provides a mechanism for executing a method when the current user is inactive for specified intervals.
    /// </summary>
    public class UserInactivityTimer : ITimer, IDisposable
    {
        private bool _disposed;
        private Timer _timer;
        private readonly TimerCallback _callback;
        private object _cookie;
        private readonly IUserIdleTime _userIdleTime;

        public UserInactivityTimer(TimeSpan scheduledRestTimeInterval, IUserIdleTime userIdleTime)
        {
            _callback = new TimerCallback(Callback);
            _userIdleTime = userIdleTime;
            ScheduledRestTimeInterval = scheduledRestTimeInterval;
        }

        private void Callback(object state)
        {
            // The condition 'state == _cookie' is needed because
            // System.Threading.Timer does not clear the work queue when it is stopped,
            // so a callback that arrived after it is stopped should not be handled.
            if (state == _cookie && _userIdleTime.Get() >= ScheduledRestTimeInterval)
            {
                OnElapsed(EventArgs.Empty);
            }
        }

        public TimeSpan ScheduledRestTimeInterval { get; set; }

        public bool Enabled { get; private set; }

        public event EventHandler Elapsed;

        protected virtual void OnElapsed(EventArgs e)
        {
            var handler = Elapsed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void Start()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(UserInactivityTimer));

            _cookie = new object();
            _timer = new Timer(_callback, _cookie, 500, 500);
            Enabled = true;
        }

        public void Stop()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(UserInactivityTimer));

            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }
            _cookie = null;
            Enabled = false;
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            Stop();
            _disposed = true;
        }
    }
}
