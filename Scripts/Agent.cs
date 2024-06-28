using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class Agent : MonoBehaviour
{
    #region Declarations
    public int order = 0;
    private float time = 1f;
    private float timestep = 0.02f;

    public HandsChangedEvent handsChangedEvent;
    public OfferEvent offerEvent;
    public CatanData catanData;

    private Model model;

    [SerializeField] private int agentID;
    private List<int[]> hands = new List<int[]>(4);
    private int opponentHandSize;
    #endregion

    #region Event Subscription
    private void OnEnable()
    {
        handsChangedEvent.OnHandsChanged += UpdateHands;
        offerEvent.OnOfferStart += MakeOffer;
        offerEvent.OnOfferSent += ReceiveOffer;
        offerEvent.OnOfferResponse += ObserveOfferResponse;
    }

    private void OnDisable()
    {
        handsChangedEvent.OnHandsChanged -= UpdateHands;
        offerEvent.OnOfferStart -= MakeOffer;
        offerEvent.OnOfferSent -= ReceiveOffer;
        offerEvent.OnOfferResponse -= ObserveOfferResponse;
    }
    #endregion

    private void Awake()
    {
        model = new Model(catanData, agentID, order);
    }

    // Write the martices of the model to text files
    public void WriteMatrices()
    {
        model.WriteAcceptedMatrixToFile("accepted" + agentID + ".txt");
        model.WriteTotalMatrixToFile("total" + agentID + ".txt");
    }

    // Change the perceived hands 
    void UpdateHands(List<List<int[]>> newHands, bool newTurn)
    {
        hands = newHands[agentID];
        int[] opponentHand = hands[1 - agentID];
        opponentHandSize = opponentHand.Sum();

        if (newTurn && order > 0)
        {
            model.ResetHandBeliefs(opponentHandSize);
            int[] opponentHandKnown = opponentHand.Take(5).ToArray();
            Tuple<int[], int[]> fakeOffer = Tuple.Create(opponentHandKnown, new int[] { 0, 0, 0, 0, 0 });
            model.ChangeHandBeliefsTrade(fakeOffer, opponentHandSize);
        }
    }

    // Find best offer or decide to stop trading
    void MakeOffer(int turnID)
    {
        if (turnID != agentID)
            return;

        Tuple<int[], int[]> offerToSend = model.FindBestOffer(hands[agentID], opponentHandSize);

        int utilityAfterOffer = model.CalculateUtility(model.GetHandAfterOffer(hands[agentID], offerToSend));
        int utilityHand = model.CalculateUtility(hands[agentID]);

        Debug.Log("Agent: " + agentID + " Hand: " + utilityHand + " Offer: " + utilityAfterOffer + " Time: " + time);
        Debug.Log("Given Cards: " + "[" + string.Join(", ", offerToSend.Item1) + "]");
        Debug.Log("Received Cards: " + "[" + string.Join(", ", offerToSend.Item2) + "]");

        if ((float)utilityHand > (utilityAfterOffer * time))
        {
            // When agent chooses to stop trading, log offer as declined
            model.ObserveOfferResponse(agentID, offerToSend, false, opponentHandSize);
            offerToSend = Tuple.Create(new int[] { 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0 });
            time = 1f;
        }

        offerEvent.SendOffer(agentID, offerToSend);
    }

    // Receive offer and decide wether to accept
    void ReceiveOffer(int senderID, Tuple<int[], int[]> offer)
    {
        if (senderID == agentID)
            return;

        Tuple<int[], int[]> offerToMe = Tuple.Create(offer.Item2, offer.Item1);

        if (order > 0) 
            model.UpdateHandBeliefs(offerToMe, opponentHandSize);

        // This means the the other agents wants to stop trading
        if (offer.Item1.Sum() + offer.Item2.Sum() == 0) 
        {
            time = 1f;
            offerEvent.RespondOffer(agentID, senderID, offer, false);
            return;
        }

        bool accept = model.Respond(hands[agentID], offerToMe, opponentHandSize);
        if (accept && order > 0)
            model.ChangeHandBeliefsTrade(offerToMe, opponentHandSize);
        
        offerEvent.RespondOffer(agentID, senderID, offer, accept);
    }

    // Change matrices based on response 
    void ObserveOfferResponse(int responderID, int senderID, Tuple<int[], int[]> offer, bool accepted)
    {
        if (responderID != agentID)
        {
            time = time - timestep;
            model.ObserveOfferResponse(agentID, offer, accepted, opponentHandSize);
        }
    }
}