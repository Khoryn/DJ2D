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
            return;
        state = stateTo;
    }

    public static bool IsState(States stateTo)
    {
        if (state == stateTo)
            return true;
        return false;
    }

    // Paused
    public static bool IsPaused
    {
        get
        {
            return IsState(States.Pause);
        }
    }

    // Playing
    public static bool IsPlaying
    {
        get
        {
            return IsState(States.Playing);
        }
    }

    // Won
    public static bool IsWon
    {
        get
        {
            return IsState(States.Won);
        }
    }

    // Lost
    public static bool IsLost
    {
        get
        {
            return IsState(States.Lost);
        }
    }

    // Lost
    public static bool TimeIsUp
    {
        get
        {
            return IsState(States.TimeIsUp);
        }
    }
}