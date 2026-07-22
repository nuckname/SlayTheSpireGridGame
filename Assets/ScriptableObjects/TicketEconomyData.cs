using UnityEngine;

[CreateAssetMenu(fileName = "NewTicketEconomy", menuName = "Game Data/Ticket Economy")]
public class TicketEconomyData : ScriptableObject
{
    public int startingTickets = 0;

    public void ResetState()
    {
        startingTickets = 0;
    }
}