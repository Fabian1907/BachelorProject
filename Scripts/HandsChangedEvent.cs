using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HandsChangedEvent", menuName = "Events/Hands Changed Event")]
public class HandsChangedEvent : ScriptableObject
{
    public event System.Action<List<List<int[]>>, bool> OnHandsChanged;

    public void NewHands(List<List<int[]>> hands, bool newTurn)
    {
        OnHandsChanged?.Invoke(hands, newTurn);
    }
}