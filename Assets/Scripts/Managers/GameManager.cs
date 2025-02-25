using UnityEngine;
using System;

public enum GameState
{
    Menu,
    Playing
}

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    private UpdateManager updateManager;
    private DeviceManager deviceManager;

    private event Action onGameStatePlaying;
    private event Action onGameStatePlayingFixedUpdate;
    private event Action onGameStateMenu;

    private GameState gameState;


    public static GameManager Instance { get => instance; }

    public Action OnGameStatePlaying { get => onGameStatePlaying; set => onGameStatePlaying = value; }
    public Action OnGameStatePlayingFixedUpdate { get => onGameStatePlayingFixedUpdate; set => onGameStatePlayingFixedUpdate = value; }
    public Action OnGameStateMenu { get => onGameStateMenu; set => onGameStateMenu = value; }

    public GameState GameState { get => gameState; }


    void Awake()
    {
        CreateGameManagerSingleton();
        InitializeGameManager();
    }

    void Update()
    {
        UpdateGame();
    }

    void FixedUpdate()
    {
        FixedUpdateGame();
    }


    public void ChangeStateTo(GameState newState)
    {
        gameState = newState;
    }


    private void CreateGameManagerSingleton()
    {
        if (instance == null)
        {
            instance = this;
        }

        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void InitializeGameManager()
    {
        gameState = GameState.Menu;

        updateManager = new UpdateManager();
        deviceManager = new DeviceManager();
    }

    private void UpdateGame()
    {
        switch (gameState)
        {
            case GameState.Menu:
                updateManager.UpdateGameInMenuState();
                break;

            case GameState.Playing:
                updateManager.UpdateAGameInPlayingState();
                break;
        }
    }

    private void FixedUpdateGame()
    {
        if (gameState == GameState.Playing)
        {
            updateManager.FixedUpdateGameInPlayingState();
        }
    }
}
