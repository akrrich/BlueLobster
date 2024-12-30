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

    private event Action onGameStatePlaying;
    private event Action onGameStatePlayingFixedUpdate;

    public static GameManager Instance { get => instance; }

    public Action OnGameStatePlaying { get => onGameStatePlaying; set => onGameStatePlaying = value; }
    public Action OnGameStatePlayingFixedUpdate { get => onGameStatePlayingFixedUpdate; set => onGameStatePlayingFixedUpdate = value; }

    private GameState gameState;
    public GameState GameState { get => gameState; }


    void Awake()
    {
        CreateGameManagerSingleton();
        InitializeGameManager();
    }

    void Update()
    {
        if (gameState == GameState.Playing)
        {
            updateManager.UpdateAllGame();
        }
    }

    void FixedUpdate()
    {
        if (gameState == GameState.Playing)
        {
            updateManager.FixedUpdateAllGame();
        }
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
    }
}
