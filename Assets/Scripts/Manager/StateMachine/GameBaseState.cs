public abstract class GameBaseState
{
    public abstract void EnterState(GameStateManager gameStateManager);
    public abstract void UpdateState(GameStateManager gameStateManager);
    public abstract void ExitState(GameStateManager gameStateManager);
}