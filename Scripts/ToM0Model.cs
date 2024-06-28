using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


// FOR NOW TREAT THIS AS A TOM_0 ONLY MODEL
public class ToM0Model
{
    public int playerID;
    private List<float[]> handBeliefs = new List<float[]>(4);

    int[,] totalOffers = new int[9, 9]
    {
        { 5, 5, 5, 5, 5, 5, 5, 5, 5 },
        { 5, 5, 5, 5, 5, 5, 5, 5, 5 },
        { 5, 5, 5, 5, 5, 5, 5, 5, 5 },
        { 5, 5, 5, 5, 5, 5, 5, 5, 5 },
        { 5, 5, 5, 5, 5, 5, 5, 5, 5 },
        { 5, 5, 5, 5, 5, 5, 5, 5, 5 },
        { 5, 5, 5, 5, 5, 5, 5, 5, 5 },
        { 5, 5, 5, 5, 5, 5, 5, 5, 5 },
        { 5, 5, 5, 5, 5, 5, 5, 5, 5 }
    };
    int[,] acceptedOffers = new int[9, 9]
    {
        { 5, 5, 5, 5, 5, 5, 5, 5, 5 },
        { 5, 5, 5, 5, 5, 5, 5, 5, 5 },
        { 5, 5, 5, 5, 5, 5, 5, 5, 5 },
        { 5, 5, 5, 5, 5, 5, 5, 5, 5 },
        { 5, 5, 5, 5, 5, 5, 5, 5, 5 },
        { 5, 5, 5, 5, 5, 5, 5, 5, 5 },
        { 5, 5, 5, 5, 5, 5, 5, 5, 5 },
        { 5, 5, 5, 5, 5, 5, 5, 5, 5 },
        { 5, 5, 5, 5, 5, 5, 5, 5, 5 }
    };

    void ObserveOfferResponse(Tuple<int[], int[]> offer, bool accepted, int respondingPlayerID)
    {
        // change offers matrices and beliefs based on response
    }

    public float GetAcceptanceRate(Tuple<int[], int[]> offer)
    {
        int cardsPlus = offer.Item2.Sum();
        int cardsMinus = offer.Item1.Sum();
        return acceptedOffers[cardsPlus, cardsMinus] / totalOffers[cardsPlus, cardsMinus];
    }

    /*
    List<float[]> getHandBeliefs(int playerID) 
    { 
        // combine handBeliefs of different order ToM models using the confidence
        return handBeliefs;
    }
    */
}
