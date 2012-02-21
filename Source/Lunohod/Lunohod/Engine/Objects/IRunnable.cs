using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lunohod.Objects
{
    /// <summary>
    /// Defines members of "runnable" XGAME components. "Runnable" components are those that can intrinsically be
    /// in running state and whose running state needs to be controlled. Those include, but not limited to
    /// music, sound effects, animations etc.
    /// </summary>
    public interface IRunnable
    {
        /// <summary>
        /// Specifies wheter the components is in the running state.
        /// This attribute value is false when the component has never been run or the <see cref="Stop()"/> method was called.
        /// </summary>
        bool InProgress { get; set; }
        /// <summary>
        /// Gets value identifying whether the current component is in the paused state.
        /// </summary>
        bool IsPaused { get; }

        /// <summary>
        /// Puts the component into the running state. This results in the <see cref="InProgress"/> property to be set to true.
        /// </summary>
        void Start();
        /// <summary>
        /// Stops the component. This results in the <see cref="InProgress"/> property to be set to false.
        /// </summary>
        void Stop();
        /// <summary>
        /// Pauses the component. This results in the <see cref="IsPaused"/> property to be set to true.
        /// </summary>
        void Pause();
        /// <summary>
        /// Resumes the component. Calling this method may result in three different behaviors:
        /// <list type="bullet">
        /// <item>
        /// <description>
        /// If the component is currently in the running state, it will continue to run.
        /// </description>
        /// </item>
        /// <item>
        /// <description>
        /// If the component is currently paused, it will resume running.
        /// </description>
        /// </item>
        /// <item>
        /// <description>
        /// If the component is currently stopped, it will be put into the running state.
        /// </description>
        /// </item>
        /// </list>
        /// </summary>
        void Resume();
        /// <summary>
        /// <exclude />
        /// </summary>
        /// <param name="p"></param>
		void Update(UpdateParameters p);
    }
}
