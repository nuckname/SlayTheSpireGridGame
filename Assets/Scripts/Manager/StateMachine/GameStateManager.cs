using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    private GameBaseState currentState;

    public PlayerTurnState InProgressState = new PlayerTurnState();
    public EnemyTurnState OverState = new EnemyTurnState();

    private void Start()
    {
        currentState = InProgressState;
        currentState.EnterState(this);
    }

    private void Update()
    {
        currentState?.UpdateState(this);
    }

    public void SwitchState(GameBaseState state)
    {
        currentState = state;
        currentState.EnterState(this);
    }
}