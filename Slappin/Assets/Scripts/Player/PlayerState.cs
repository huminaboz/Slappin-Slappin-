using UnityEngine;

public enum PossibleStates
{
    Z_Null = -1,

    DefaultState = 0,

    SlapState = 100,

    FlickState = 500,

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
    }

    public override void Exit(PlayerState toState)
    {
    }

    public override void Update(float deltaTime)
    {
        if (!StateGame.PlayerInGameControlsEnabled) return;

        //NOTE: IF I decided to handle movement here, we wouldn't have the fun
        //sliding around on the way up - unless I converted all that to sub-states here
        
        if (Input.GetButtonDown("Fire1"))
        {
            thisPlayer.SetState(new StateSlapState(thisPlayer));
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
        thisPlayer.slapAttack.DoAttack();
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
        
        //Start the hand blinking or color changing or whatever
        thisPlayer.CurrentAttackType.HandleGettingHurt();
        
        //Play a hand hurt animation?
        
        // _t = 0f;
    }

    public override void Exit(PlayerState toState)
    {
        thisPlayer.EnableMovement();
        
        //Return the hand materials to normal look
        thisPlayer.CurrentAttackType.RestoreDefaultAppearance();
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