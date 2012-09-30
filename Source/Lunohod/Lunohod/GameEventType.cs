using System;

namespace Lunohod
{
    public enum GameEventType
    {
        Custom,
        Up,
        Down,
        Left,
        Right,
		Pause,
        Stop,
        Explosion,
        CloseCurrentScreen,
		DismissCurrentScreen,
        RestartLevel,
        AbandonLevel,
        EndLevel,
        LevelLoaded,
        StartNextLevel,
		CpsLimitExceeded,
		ScreenActivated
    }
}

