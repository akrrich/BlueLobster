using System;
using UnityEngine;

public class PlayerEvents : MonoBehaviour
{
    private static event Action onPlayerDefeated;

    public static Action OnPlayerDefeated { get => onPlayerDefeated; set => onPlayerDefeated = value; } 
}
