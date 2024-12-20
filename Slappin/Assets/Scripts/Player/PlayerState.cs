using System;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PossibleStates
{
    Z_Null = -1,

    DefaultState = 0,

    SlapState = 100,

    FlickState = 500,
    
    AbsorbState = 750,

    SquishState = 1000,

    WildcardState = 2000,

    DamagedState = 3000, //This is if your hand gets hurt while attacking

    DeathState = 4000
}

public abstract class PlayerState
{
    public PossibleStates state; //If I want to put up a debug notification of what state I'm in, I can use this value
    protected Player thisPlayer;

    protected PlayerState(Player player)
    {
        thisPlayer = player;
    }

    public abstract void Enter(PlayerState fromState);

    public abstract void Exit(PlayerState toState);

    public abstract void Update(float deltaTime);

    public abstract void FixedUpdate(float fixedDeltaTime);
}

/// <summary> =========================================================================
/// EMPTY STATE
/// </summary> =========================================================================
public class StateEmpty : PlayerState
{
    public StateEmpty(Player player) : base(player)
    {
        state = PossibleStates.Z_Null;
    }

    public override void Enter(PlayerState fromState)
    {
    }

    public override void Exit(PlayerState toState)
    {
    }

    public override void Update(float deltaTime)
    {
        //Enjoy the void
    }

    public override void FixedUpdate(float fixedDeltaTime)
    {
    }
}

/// <summary> =========================================================================
/// The basic state when you're deciding what to do next and moving the shadow around
/// </summary> =========================================================================
public class StateDefault : PlayerState
{
    //Handle shadow movement and such here - but the main code for movement should be in the movement class
    //You want to use this to check for state, essentially - what's possible and then run the functions
    //In the movement class
    private HandMovement _handMovement;

    public StateDefault(Player player) : base(player)
    {
        state = PossibleStates.DefaultState;
    }

    public override void Enter(PlayerState fromState)
    {
        thisPlayer.EnableMovement();
        thisPlayer.CurrentAttackType = null;
    }

    public override void Exit(PlayerState toState)
    {
        thisPlayer.handMovement._rigidbody.velocity = Vector3.zero;
    }

    public override void Update(float deltaTime)
    {
        if (!StateGame.PlayerInGameControlsEnabled) return;

        //NOTE: IF I decided to handle movement here, we wouldn't have the fun
        //sliding around on the way up - unless I converted all that to sub-states here

        /*
         *  A=0
            B=1
            X=2
            Y=3
            LBUMP=4
            RBUMP=5
            LTRIG=6
            RTRIG=9
            SELECT=8
            START=4
            MIDDLE=10
            LSTICK=11
            RSTICK=12
         */

        if (thisPlayer._inputSystem.Player.Slap.IsPressed())
        {
            thisPlayer.SetState(new StateSlapState(thisPlayer));
        }
        else if (thisPlayer._inputSystem.Player.Absorb.IsPressed())
        {
            thisPlayer.SetState(new StateAbsorbState(thisPlayer));
        }
        else if (thisPlayer._inputSystem.Player.Flick.IsPressed())
        {
            thisPlayer.SetState(new StateFlickState(thisPlayer));
        }
        else if (thisPlayer._inputSystem.Player.Squish.IsPressed())
        {
            thisPlayer.SetState(new SquishState(thisPlayer));
        }
    }

    public override void FixedUpdate(float fixedDeltaTime)
    {
        //Handle the moving around here?
    }
}

/// <summary> =========================================================================
/// The state when the slap is heading down or up
/// </summary> =========================================================================
public class StateSlapState : PlayerState
{
    public StateSlapState(Player player) : base(player)
    {
        state = PossibleStates.SlapState;
        thisPlayer = player;
    }

    public override void Enter(PlayerState fromState)
    {
        //TODO:: Make this into an enum and function that happens on the player
        thisPlayer.CurrentAttackType = thisPlayer.slapAttack;
        thisPlayer.CurrentAttackType.CleanupThatsOnlyCalledFromStateMachine();
        thisPlayer.CurrentAttackType.gameObject.SetActive(true);
        thisPlayer.CurrentAttackType.InitiateAttack();
        thisPlayer.DisableMovement();
    }

    public override void Exit(PlayerState toState)
    {
    }

    public override void Update(float deltaTime)
    {
    }

    public override void FixedUpdate(float fixedDeltaTime)
    {
    }
}

/// <summary> =========================================================================
/// The state when the flick is in action
/// </summary> =========================================================================
public class StateFlickState : PlayerState
{
    private FlickAttack _flickAttack;
    
    public StateFlickState(Player player) : base(player)
    {
        state = PossibleStates.FlickState;
        thisPlayer = player;
    }

    public override void Enter(PlayerState fromState)
    {
        thisPlayer.CurrentAttackType = thisPlayer.flickAttack;
        // _flickAttack = (FlickAttack) thisPlayer.CurrentAttackType;
        thisPlayer.CurrentAttackType.CleanupThatsOnlyCalledFromStateMachine();
        thisPlayer.CurrentAttackType.gameObject.SetActive(true);
        thisPlayer.CurrentAttackType.InitiateAttack();
        // _flickAttack.InitiateAttack();
    }

    public override void Exit(PlayerState toState)
    {
    }

    public override void Update(float deltaTime)
    {

    }

    public override void FixedUpdate(float fixedDeltaTime)
    {
    }
}

/// <summary> =========================================================================
/// The state when the squish is in action
/// </summary> =========================================================================
public class SquishState : PlayerState
{
    private SquishState _squishAttack;
    
    public SquishState(Player player) : base(player)
    {
        state = PossibleStates.SquishState;
        thisPlayer = player;
    }

    public override void Enter(PlayerState fromState)
    {
        thisPlayer.CurrentAttackType = thisPlayer.squishAttack;
        thisPlayer.CurrentAttackType.CleanupThatsOnlyCalledFromStateMachine();
        thisPlayer.CurrentAttackType.gameObject.SetActive(true);
        thisPlayer.CurrentAttackType.InitiateAttack();
    }

    public override void Exit(PlayerState toState)
    {
    }

    public override void Update(float deltaTime)
    {
        // if (Input.GetButtonUp("Fire4"))
        // {
        //     thisPlayer.squishAttack.InitiateTravelBackUp();
        // }
        
    }

    public override void FixedUpdate(float fixedDeltaTime)
    {
        // if (Input.GetButton("Fire4"))
        // {
        //     thisPlayer.squishAttack.DoShitWhileTouchingGround();
        // }
    }
}

/// <summary> =========================================================================
/// The state when absorbing
/// </summary> =========================================================================
public class StateAbsorbState : PlayerState
{
    public static Action OnAbsorbPressed;
    public static Action OnAbsorbReleased;
    
    public StateAbsorbState(Player player) : base(player)
    {
        state = PossibleStates.AbsorbState;
        thisPlayer = player;
    }

    public override void Enter(PlayerState fromState)
    {
        OnAbsorbPressed?.Invoke();
    }

    public override void Exit(PlayerState toState)
    {
        OnAbsorbReleased?.Invoke();
    }

    public override void Update(float deltaTime)
    {
        if (thisPlayer._inputSystem.Player.Absorb.WasReleasedThisFrame())
        {
            thisPlayer.SetState(new StateDefault(thisPlayer));
        }
    }

    public override void FixedUpdate(float fixedDeltaTime)
    {
    }
}


/// <summary> =========================================================================
/// When the hand hits something that can damage it
/// </summary> =========================================================================
public class StateDamagedState : PlayerState
{
    // private float _t = 0f;
    // private float _damageAnimationTime = 0f;

    public StateDamagedState(Player player) : base(player)
    {
        state = PossibleStates.DamagedState;
        thisPlayer = player;
    }

    public override void Enter(PlayerState fromState)
    {
        //Disable the ability to move around or put in inputs
        thisPlayer.DisableMovement();
        thisPlayer.CurrentAttackType.StopTraveling();

        //Start the hand blinking or color changing or whatever
        thisPlayer.CurrentAttackType?.HandleGettingHurt();

        //Play a hand hurt animation?

        // _t = 0f;
        StateGame.PlayerInGameControlsEnabled = false;
    }

    public override void Exit(PlayerState toState)
    {
        StateGame.PlayerInGameControlsEnabled = true;
        thisPlayer.EnableMovement();

        //Return the hand materials to normal look
    }

    public override void Update(float deltaTime)
    {
        //TODO:: If hp <= 0 , enter DeathState

        // _t += deltaTime;
        // if (_t >= _damageAnimationTime)
        // {
        //     
        // }
    }

    public override void FixedUpdate(float fixedDeltaTime)
    {
    }
}