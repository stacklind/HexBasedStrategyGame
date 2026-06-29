using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler
{
    public static PlayerHandler Instance { get; private set; }

    private Player[] players;
    
    public PlayerHandler(GameObject playerPrefab, int numberOfPlayers)
    {
        Instance = this;
        players = new Player[numberOfPlayers];
        for (int i = 0; i < numberOfPlayers; i++)
        {
            players[i] = new Player(i, GameObject.Instantiate(playerPrefab));
        }
    }

    public static bool Init(GameObject playerPrefab)
    {
        if (Instance == null)
        {
            new PlayerHandler(playerPrefab, GameHandler.MapSettings.StartingPlayers);
            return true;
        }
        return false;
    }

    public Player GetPlayer(int id)
    {
        return players[id];
    }
}
