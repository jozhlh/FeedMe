using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public enum Modes {searchTutorial, placeTutorial/*, buyTutorial*/, play}
    public static float sensitivity;
   // public static int totalOil;
    public static float animationsDuration;
  //  public static int oilForVictory;
    public static int oilScore;
    public static int numberOfHumanPlayers;
    public static Modes gameMode;
    public static int p1GamesWon;
    public static int p2GamesWon;
    public static int aiDifficulty = 0;
    public static int currentLevel = -1;
    public static float iphoneXnotch = 60.0f;

    public static bool appleTV = false;
    public static bool fireTV = false;

    public static bool startUp = true;

    [Header("Input")]
    [SerializeField]
    [Range(0.0f, 1500.0f)]
    private float tapToSwipeSensitivity = 200.0f;

    [SerializeField]
    private float durationOfDiscoveryAnimations;
    [SerializeField]
    private int valueOfOilDiscovery = 1;

	// Use this for initialization
	void Awake ()
    {
        sensitivity = tapToSwipeSensitivity;
        animationsDuration = durationOfDiscoveryAnimations;
        oilScore = valueOfOilDiscovery;
        DontDestroyOnLoad(gameObject);
        numberOfHumanPlayers = 0;
        gameMode = Modes.play;
        p1GamesWon = 0;
        p2GamesWon = 0;
        startUp = true;
	}
	
	public void SetNumberOfPlayers(int numPlayers)
    {
        numberOfHumanPlayers = numPlayers;
    }

    public void SetTutorialMode(Modes selectedMode)
    {
        gameMode = selectedMode;
    }

    public void SetDifficulty(int difficulty)
    {
        aiDifficulty = difficulty;
    }

    public void SetCurrentLevel(int levelNumber)
    {
        currentLevel = levelNumber;
    }

    public void ResetGamesWon()
    {
        p1GamesWon = 0;
        p2GamesWon = 0;
    }

    public void IncrementGamesWon(int playerNum)
    {
        if (playerNum == 0)
        {
            p1GamesWon++;
        }
        else
        {
            p2GamesWon++;
        }
    }
}
