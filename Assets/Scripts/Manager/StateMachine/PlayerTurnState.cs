public class PlayerTurnState : GameBaseState
{
    public override void EnterState(GameStateManager gameStateManager)
    {
        GameStateManager.CanClickCards = true;
    }

    public override void UpdateState(GameStateManager gameStateManager)
    {
        
    }

    public override void ExitState(GameStateManager gameStateManager)
    {
        GameStateManager.CanClickCards = false;
        // play our cards in order and then switch to enemy turn state
    }
}