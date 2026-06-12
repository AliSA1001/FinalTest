#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// EDITOR-ONLY manual tester for the Score/Rating system. Because Combat and the other signal sources do
/// not exist yet, this lets you exercise <see cref="T_ScoreSystem"/> in isolation from the keyboard and see
/// its outbound signals in the Console. Wrapped in UNITY_EDITOR, so it is compiled out of player builds.
///
/// Keys (new Input System; no .inputactions wiring needed):
///   P = parry (Good)     O = perfect parry     H = attack hit
///   K = enemy killed     C = collectable       J = player damaged
///   L = player died      [ = act started       ] = act finished
/// </summary>
public class T_ScoreDebugger : MonoBehaviour
{
    private int _debugActIndex;

    private void OnEnable()
    {
        T_GameSignals.ScoreChanged += LogScoreChanged;
        T_GameSignals.ComboChanged += LogComboChanged;
        T_GameSignals.ScoreEventLanded += LogScoreEvent;
        T_GameSignals.ActRated += LogActRated;
        T_GameSignals.RunResultReady += LogRunResult;
    }

    private void OnDisable()
    {
        T_GameSignals.ScoreChanged -= LogScoreChanged;
        T_GameSignals.ComboChanged -= LogComboChanged;
        T_GameSignals.ScoreEventLanded -= LogScoreEvent;
        T_GameSignals.ActRated -= LogActRated;
        T_GameSignals.RunResultReady -= LogRunResult;
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
        {
            return;
        }

        if (keyboard.pKey.wasPressedThisFrame) T_GameSignals.RaiseParryLanded(ParryQuality.Good);
        if (keyboard.oKey.wasPressedThisFrame) T_GameSignals.RaiseParryLanded(ParryQuality.Perfect);
        if (keyboard.hKey.wasPressedThisFrame) T_GameSignals.RaiseAttackHitLanded();
        if (keyboard.kKey.wasPressedThisFrame) T_GameSignals.RaiseEnemyKilled();
        if (keyboard.cKey.wasPressedThisFrame) T_GameSignals.RaiseCollectableCollected();
        if (keyboard.jKey.wasPressedThisFrame) T_GameSignals.RaisePlayerDamaged();
        if (keyboard.lKey.wasPressedThisFrame) T_GameSignals.RaisePlayerDied();

        if (keyboard.leftBracketKey.wasPressedThisFrame)
        {
            T_GameSignals.RaiseActStarted(_debugActIndex);
        }
        if (keyboard.rightBracketKey.wasPressedThisFrame)
        {
            T_GameSignals.RaiseActFinished(_debugActIndex);
            _debugActIndex++;
        }
    }

    private void LogScoreChanged(int total, int delta) => Debug.Log($"[Score] total={total} (Δ{delta})");
    private void LogComboChanged(int combo, float multiplier) => Debug.Log($"[Combo] {combo} hits → multiplier x{multiplier:0.00}");
    private void LogScoreEvent(ScoreEventType type, int points) => Debug.Log($"[Event] {type} +{points}");
    private void LogActRated(int actIndex, int stars, int finalScore) => Debug.Log($"[Act {actIndex}] {stars}★ (score {finalScore})");
    private void LogRunResult(int totalStars, bool passed) => Debug.Log($"[Run] total {totalStars}★ → {(passed ? "WIN" : "LOSE")}");
}
#endif
