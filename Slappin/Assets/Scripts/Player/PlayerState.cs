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
    public StateEmpty()
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
    private Player thisPlayer;
    
    public StateDefault(Player player)
    {
        state = PossibleStates.DefaultState;
        thisPlayer = player;
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
        //Handle the moving around here?
    }
}

/// <summary> =========================================================================
/// The state when the slap is heading down or not
/// </summary> =========================================================================
public class StateSlapState : PlayerState
{
    public override void Enter(PlayerState fromState)
    {
        //Disable the ability to move around
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
    public override void Enter(PlayerState fromState)
    {
        //Disable the ability to move around or put in inputs
    }

    public override void Exit(PlayerState toState)
    {
        //Go to the default state or death state
    }

    public override void Update(float deltaTime)
    {
        //Set a timer to be able to return to default state if not dead
    }

    public override void FixedUpdate(float fixedDeltaTime)
    {
    }
}