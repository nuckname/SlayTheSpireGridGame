using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static bool CanClickCards = true;

    private GameBaseState currentState;

    public PlayerTurnState PlayerTurn = new PlayerTurnState();
    public EnemyTurnState EnemyTurn = new EnemyTurnState();

    private void Start()
    {
        currentState = PlayerTurn;
        currentState.EnterState(this);
    }

    private void Update()
    {
        currentState?.UpdateState(this);
    }

    public void SwitchState(GameBaseState state)
    {
        currentState?.ExitState(this);
        currentState = state;
        currentState.EnterState(this);
    }
}