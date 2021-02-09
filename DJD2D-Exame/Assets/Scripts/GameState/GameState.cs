using UnityEngine;
using System.Collections;

public class GameState
{
    public enum States
    {
        Pause, Playing, Won, Lost, TimeIsUp
    }
    public static States state = States.Pause;

    public static void ChangeState(States stateTo)
    {
        if (state == stateTo)
        {
            return;
        }
        state = stateTo;
    }

    public static bool IsState(States stateTo)
    {
        if (state == stateTo)
        {
            return true;
        }
        return false;
    }

    // Paused State
    public static bool IsPaused
    {
        get
        {
            return IsState(States.Pause);
        }
    }

    // Playing State
    public static bool IsPlaying
    {
        get
        {
            return IsState(States.Playing);
        }
    }

    // Won State
    public static bool IsWon
    {
        get
        {
            return IsState(States.Won);
        }
    }

    // Lost State
    public static bool IsLost
    {
        get
        {
            return IsState(States.Lost);
        }
    }

    // Lost State
    public static bool TimeIsUp
    {
        get
        {
            return IsState(States.TimeIsUp);
        }
    }
}