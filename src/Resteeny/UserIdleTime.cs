using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;
using static Resteeny.SafeNativeMethods;

namespace Resteeny
{
    /// <summary>
    /// Represents the current user session's idle time (no input events).
    /// </summary>
    public class UserIdleTime : IUserIdleTime
    {
        private LastInputInfo _lastInputInfo;

        public UserIdleTime()
        {
            _lastInputInfo = new LastInputInfo();
            _lastInputInfo.CbSize = Marshal.SizeOf(_lastInputInfo);
        }

        /// <summary>
        /// Retrieves the time since the last input event. This is for the current user's session only.
        /// </summary>
        /// <returns>TimeSpan with amount of time that has elapsed since the last input event.</returns>
        public TimeSpan Get()
        {
            if (!GetLastInputInfo(out _lastInputInfo))
                throw new Win32Exception("GetLastInputInfo function in user32.dll returned false.");

            return TimeSpan.FromTicks(Environment.TickCount - _lastInputInfo.DwTime);
        }
    }

    [SuppressUnmanagedCodeSecurity]
    internal static class SafeNativeMethods
    {
        /// <summary>
        /// Retrieves the number of ticks (since the system was started) at which the last input event occured.
        /// </summary>
        /// <remarks>
        /// This function answers the question "Since the system was started, when was the last time an input event occured?"
        /// It does not answer the question "For how long there is no input events (for how long the user is idle)?"
        /// 
        /// 
        ///     System started      Last input event occured     Now
        ///     |                   |                            |
        ///     *===================*============================*> (time axis)
        ///     |___________________|____________________________|
        ///        GetLastInputInfo          user is idle
        /// 
        ///     |________________________________________________|
        ///                System.Environment.TickCount
        /// 
        /// 
        /// </remarks>
        /// <param name="plii">An instance of <see cref="LastInputInfo"/> that receives the time of the last input event.</param>
        /// <returns>True if the function succeeds, otherwise false.</returns>
        [DllImport("user32.dll")]
        internal static extern bool GetLastInputInfo(out LastInputInfo plii);

        internal struct LastInputInfo
        {
            public int CbSize;
            public int DwTime;
        }
    }
}
