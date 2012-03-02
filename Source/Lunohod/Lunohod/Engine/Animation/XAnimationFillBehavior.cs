using System;

namespace Lunohod
{
    /// <summary>
    /// Describes the animation ending behavior.
    /// </summary>
	public enum XAnimationFillBehavior
	{
        /// <summary>
        /// When stopped, the animation will be left at the current frame.
        /// </summary>
		Hold,
        /// <summary>
        /// When stopped or ended, the animation will be set to the very first frame.
        /// </summary>
		Reset,
        /// <summary>
        /// When stopped or ended, the animation will be set to the very last frame.
        /// </summary>
		End
	}
}

