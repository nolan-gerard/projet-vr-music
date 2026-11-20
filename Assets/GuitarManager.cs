using UnityEngine;
using UnityEngine.InputSystem;

public class GuitarManager : MonoBehaviour
{
    [Header("Références aux 6 cordes")]
    public GuitarString[] strings = new GuitarString[6];

    [Header("Banques de sons (6 clips par état)")]
    public AudioClip[] chordOpen = new AudioClip[6];  
    public AudioClip[] chordPos1 = new AudioClip[6];  
    public AudioClip[] chordPos2 = new AudioClip[6];   

    [Header("VR Inputs (Main Gauche)")]
    public InputActionProperty leftTriggerAction;

    // 0 = Hors zone / À vide, 1 = Zone 1, 2 = Zone 2
    private int currentNeckPosition = 0; 

    private enum GuitarState { Open, Pos1, Pos2 }
    private GuitarState currentState = GuitarState.Open;

    private void Start()
    {
        ApplyChordClips(GuitarState.Open);
    }

    private void Update()
    {
        EvaluateStateAndApplyChord();
    }

    public void SetNeckPosition(int positionID)
    {
        currentNeckPosition = positionID;
    }

    private void EvaluateStateAndApplyChord()
    {
        float triggerValue = leftTriggerAction.action != null ? leftTriggerAction.action.ReadValue<float>() : 0f;
        bool isTriggerPressed = triggerValue > 0.1f;

        GuitarState targetState = GuitarState.Open;

        if (isTriggerPressed)
        {
            if (currentNeckPosition == 1) targetState = GuitarState.Pos1;
            else if (currentNeckPosition == 2) targetState = GuitarState.Pos2;
            else targetState = GuitarState.Open;
        }
        else
        {
            targetState = GuitarState.Open;
        }

        if (targetState != currentState)
        {
            currentState = targetState;
            ApplyChordClips(currentState);
        }
    }

    private void ApplyChordClips(GuitarState state)
    {
        AudioClip[] targetClips = chordOpen;

        switch (state)
        {
            case GuitarState.Open: targetClips = chordOpen; break;
            case GuitarState.Pos1: targetClips = chordPos1; break;
            case GuitarState.Pos2: targetClips = chordPos2; break;
        }

        for (int i = 0; i < 6; i++)
        {
            if (strings[i] != null && targetClips != null && targetClips.Length > i)
            {
                strings[i].SetNote(targetClips[i]);
            }
        }
    }
}