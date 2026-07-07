using UnityEngine;

/// <summary>
/// Integration bridge: turns the Game Over screen showing up into a <see cref="T_GameSignals.PlayerDied"/>
/// signal, without touching any teammate code. Waad's <c>GameOverUI.ShowGameOver()</c> activates the game-over
/// panel on death; this component rides on that panel and raises PlayerDied from <c>OnEnable</c>, which applies
/// the Score death penalty, breaks the combo, cues the crowd, and plays the death SFX.
///
/// Setup: drop this on the game-over panel GameObject (the one <c>GameOverUI.gameOverPanel</c> points at). That
/// object must start inactive — which it already must, for the game to work — so OnEnable only ever runs on an
/// actual death. References only the event bus.
/// </summary>
public class T_PlayerDeathBridge : MonoBehaviour
{
    private void OnEnable()
    {
        // Guard against OnEnable running in the Editor (e.g. entering Play with the panel momentarily active).
        if (!Application.isPlaying) return;
        T_GameSignals.RaisePlayerDied();
    }
}
