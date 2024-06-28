using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OfferEvent", menuName = "Events/Offer Event")]
public class OfferEvent : ScriptableObject
{
    public event Action<int> OnOfferStart;
    public event Action<int, Tuple<int[], int[]>> OnOfferSent;
    public event Action<int, int, Tuple<int[], int[]>, bool> OnOfferResponse;

    public void StartOffer(int agentID)
    {
        OnOfferStart?.Invoke(agentID);
    }

    public void SendOffer(int senderID, Tuple<int[], int[]> offer)
    {
        OnOfferSent?.Invoke(senderID, offer);
    }

    public void RespondOffer(int responderID, int senderID, Tuple<int[], int[]> offer, bool accepted)
    {
        OnOfferResponse?.Invoke(responderID, senderID, offer, accepted);
    }
}