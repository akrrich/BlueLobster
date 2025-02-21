using System;

public class PlayerEvents
{
    // Evento para ejecutar las pantallas de derrota cuando muere el player
    private static event Action onPlayerDefeated;
    public static Action OnPlayerDefeated { get => onPlayerDefeated; set => onPlayerDefeated = value; }

    // Evento para activar boton de agarrar objetos
    private static event Action onEnabledPickUpButton;
    public static Action OnEnabledPickUpButton { get => onEnabledPickUpButton; set => onEnabledPickUpButton = value; }

    // Evento para desactivar boton de agarrar objetos
    private static event Action onDisabledPickUpButton;
    public static Action OnDisabledPickUpButton { get => onDisabledPickUpButton; set => onDisabledPickUpButton = value; }
}
