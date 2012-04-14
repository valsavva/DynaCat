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
        Stop,
        Explosion,
        CloseCurrentScreen,
        RestartLevel,
        AbandonLevel,
        EndLevel,
        LevelLoaded,
        StartNextLevel
    }
}

