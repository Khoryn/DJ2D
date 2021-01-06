using UnityEngine;
using System.Collections;

public class GameState
{
    public enum States
    {
        Idle, Running, Jumping, Falling, Dead, StartTalking, Talking, Pause, Won
    }
    public static States state = States.Idle;

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

    // Idle
    public static bool IsIdle
    {
        get
        {
            return IsState(States.Idle);
        }
    }

    // Running
    public static bool IsRunning
    {
        get
        {
            return IsState(States.Running);
        }
    }

    // Jumping
    public static bool IsJumping
    {
        get
        {
            return IsState(States.Jumping);
        }
    }

    // Falling
    public static bool IsFalling
    {
        get
        {
            return IsState(States.Falling);
        }
    }

    // Dead
    public static bool IsDead
    {
        get
        {
            return IsState(States.Dead);
        }
    }

    // Start Dialogue
    public static bool IsStartDialogue
    {
        get
        {
            return IsState(States.StartTalking);
        }
    }

    // Dialogue
    public static bool IsTalking
    {
        get
        {
            return IsState(States.Talking);
        }
    }

    public static bool IsPaused
    {
        get
        {
            return IsState(States.Pause);
        }
    }

    //// You can still do this but will need GameState.Running = true;
    //// ChangeState is more atomic...
    //public static bool Running
    //{
    //    get
    //    {
    //        return IsState(States.Pause);
    //    }
    //    set
    //    {
    //        if (value)
    //            ChangeState(States.Running);
    //    }
    //}
}