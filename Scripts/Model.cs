using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Model
{
    public CatanData catanData;
    private Model opponentModel;
    private bool opponentCardsUnknown = true;
    int order;

    public int agentID;
    Tuple<int[], int[]> opponentOffer;
    int[] opponentHandGuess;

    #region OfferAcceptanceMatrices
    private int[,] totalOffers = new int[17, 17]
    ///*
    {
        { 3264, 939, 138, 185, 169, 217, 233, 217, 214, 39, 11, 7, 5, 5, 5, 5, 5 },
        { 43, 492, 371, 169, 152, 159, 132, 156, 163, 42, 9, 6, 5, 5, 5, 5, 5 },
        { 25, 10005, 141, 100, 105, 170, 145, 147, 134, 34, 7, 7, 5, 5, 5, 5, 5 },
        { 19, 198, 125, 86, 100, 92, 77, 86, 90, 31, 7, 6, 5, 5, 5, 5, 5 },
        { 12, 49, 43, 34, 56, 51, 50, 56, 64, 22, 7, 5, 5, 5, 5, 5, 5 },
        { 5, 21, 82, 30, 47, 37, 37, 40, 42, 15, 6, 5, 5, 5, 5, 5, 5 },
        { 5, 7, 18, 23, 34, 31, 32, 29, 29, 11, 5, 5, 5, 5, 5, 5, 5 },
        { 5, 5, 11, 17, 22, 39, 20, 23, 24, 7, 5, 5, 5, 5, 5, 5, 5 },
        { 5, 5, 7, 10, 10, 36, 16, 19, 16, 5, 5, 5, 5, 5, 5, 5, 5 },
        { 5, 5, 5, 6, 6, 8, 9, 7, 5, 5, 5, 5, 5, 5, 5, 5, 5 },
        { 5, 5, 5, 5, 5, 7, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 },
        { 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 },
        { 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 },
        { 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 },
        { 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 },
        { 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 },
        { 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 }
    };
    //*/
    // trained with p 0 ts 0.02 c 3

    private int[,] acceptedOffers = new int[17, 17]
    ///*
    {
        { 5, 59, 5, 6, 5, 6, 6, 5, 6, 5, 5, 5, 5, 5, 5, 5, 5 },
        { 5, 69, 21, 6, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 },
        { 5, 3448, 13, 7, 5, 6, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 },
        { 5, 46, 20, 9, 8, 7, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 },
        { 5, 12, 7, 5, 7, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 },
        { 5, 7, 21, 5, 7, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 },
        { 5, 5, 5, 5, 6, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 },
        { 5, 5, 5, 5, 5, 10, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 },
        { 5, 5, 5, 5, 5, 13, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 },
        { 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 },
        { 5, 5, 5, 5, 5, 7, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 },
        { 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 },
        { 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 },
        { 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 },
        { 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 },
        { 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 },
        { 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 }
    };
    //*/
    #endregion

    // Dictionary of hand beliefs in ranges from believed min to max
    Dictionary<string, (int, int)> opponentHandBeliefs = new Dictionary<string, (int, int)>();
    
    public Model(CatanData _catanData, int _agentID, int _order)
    {
        catanData = _catanData;
        agentID = _agentID;
        order = _order;

        if (order > 0)
        {
            opponentModel = new Model(catanData, 1 - agentID, order - 1);
        }
    }

    public void ResetHandBeliefs(int opponentHandSize)
    {
        opponentHandBeliefs.Clear();
        // Opponents can have 0 to max of each resource
        foreach (string resource in catanData.resources)
            opponentHandBeliefs.Add(resource, (0, opponentHandSize));
    }

    // Change hand beliefs after a successful trade
    public void ChangeHandBeliefsTrade(Tuple<int[], int[]> offer, int opponentHandSize)
    {
        for (int resource = 0; resource < catanData.GetResourceAmount(); ++resource)
        {
            int resourceGiven = offer.Item1[resource];
            int resourceReceived = offer.Item2[resource];
            int resourceChange = resourceGiven - resourceReceived;

            string resourceName = catanData.resources[resource];
            int min = opponentHandBeliefs[resourceName].Item1;
            int max = opponentHandBeliefs[resourceName].Item2;

            opponentHandBeliefs[resourceName] = (Mathf.Max(0, min + resourceChange), 
                                                 Mathf.Min(opponentHandSize, max + resourceChange));
        }
        FindOpponentDesires(opponentHandSize);
    }

    // Update hand beliefs when opponent makes offer
    public void UpdateHandBeliefs(Tuple<int[], int[]> offer, int opponentHandSize)
    {
        for (int resource = 0; resource < catanData.GetResourceAmount(); ++resource)
        {
            string resourceName = catanData.resources[resource];
            int min = opponentHandBeliefs[resourceName].Item1;
            int max = opponentHandBeliefs[resourceName].Item2;

            int resourceAmount = offer.Item2[resource];
            int otherResources = offer.Item2.Sum() - resourceAmount;

            opponentHandBeliefs[resourceName] = (Mathf.Max(min, resourceAmount), 
                                                 Mathf.Min(max, opponentHandSize - otherResources));
        }

        for (int resourceAsked = 0; resourceAsked < catanData.GetResourceAmount(); ++resourceAsked)
        {
            int resourceAskedAmount = offer.Item1[resourceAsked];
            for (int i = 0; i < resourceAskedAmount; ++i)
            {
                for (int resource = 0; resource < catanData.GetResourceAmount(); ++resource)
                {
                    float random = UnityEngine.Random.value;
                    float overlap = catanData.buyOverlap[resourceAsked, resource];
                    float leniency = 0.2f;
                    if (random + leniency < overlap || overlap == 1f)
                    {
                        string resourceName = catanData.resources[resource];
                        int min = opponentHandBeliefs[resourceName].Item1;
                        int max = opponentHandBeliefs[resourceName].Item2;
                        opponentHandBeliefs[resourceName] = (Mathf.Min(++min, max), max);

                        for (int otherResource = 0; otherResource < catanData.GetResourceAmount(); ++otherResource)
                        {
                            if (otherResource != resource)
                            {
                                string resourceNameOther = catanData.resources[otherResource];
                                int minOther = opponentHandBeliefs[resourceNameOther].Item1;
                                int maxOther = opponentHandBeliefs[resourceNameOther].Item2;
                                opponentHandBeliefs[resourceNameOther] = (minOther, Mathf.Max(--maxOther, minOther));
                            }
                        }
                    }
                }
            }
        }

        opponentCardsUnknown = false;
        FindOpponentDesires(opponentHandSize);
    }

    // EXPERIMENT WITH LENIENCY!!
    // 0.2:     a0: 3518 a1: 2846 a0: 3727 a1: 3630 a0: 3359 a1: 3202 a0: 9518 a1: 9128
    // 0.25:    a0: 3539 a1: 3485 a0: 3272 a1: 3236
    // 0.15:    a0: 3692 a1: 3519 a0: 4214 a1: 3922
    // 0.18:    a0: 3492 a1: 3270 a0: 3891 a1: 3391 a0: 5892 a1: 5510 a0: 6801 a1: 6864
    // 0.1:     a0: 3662 a1: 3556 a0: 5251 a1: 5007
    // 0.3:     a0: 3402 a1: 3406

    #region Write Matrices
    public void WriteAcceptedMatrixToFile(string filePath)
    {
        StreamWriter writer = new StreamWriter(filePath);

        for (int i = 0; i < 17; i++)
        {
            for (int j = 0; j < 17; j++)
            {
                writer.Write(acceptedOffers[i, j] + " ");
            }
            writer.WriteLine();
        }

        writer.Close();

        Debug.Log("Accepted data has been written to: " + filePath);
    }

    public void WriteTotalMatrixToFile(string filePath)
    {
        StreamWriter writer = new StreamWriter(filePath);

        for (int i = 0; i < 17; i++)
        {
            for (int j = 0; j < 17; j++)
            {
                writer.Write(totalOffers[i, j] + " ");
            }
            writer.WriteLine();
        }

        writer.Close();

        Debug.Log("Total data has been written to: " + filePath);
    }
    #endregion

    public bool Respond(int[] hand, Tuple<int[], int[]> offer, int opponentHandSize)
    {
        if (!CanAcceptOffer(hand, offer))
            return false;

        Tuple<int[], int[]> bestOfferOld = FindBestOffer(hand, opponentHandSize);
        float scoreOld = CalculateScore(hand, bestOfferOld, opponentHandSize);

        bool accept = false;

        int[] handAfterOffer = GetHandAfterOffer(hand, offer).ToArray();
        Tuple<int[], int[]> bestOfferAfter = FindBestOffer(handAfterOffer, GetOpponentHandSizeAfterOffer(opponentHandSize, offer));
        float scoreAfter = CalculateScore(handAfterOffer, bestOfferAfter, opponentHandSize);

        //Debug.Log(" >>>>> scoreAfter: " + scoreAfter + " <<<<<< scoreOld " + scoreOld);
        if (scoreAfter > scoreOld)
            accept = true;

        return accept;
    }

    // Helper function to check whether agent has enough cards to accept offer
    bool CanAcceptOffer(int[] hand, Tuple<int[], int[]> offer)
    {
        return hand.Zip(offer.Item1, (a, b) => a >= b).All(result => result);
    }

    // Count the number of cards in the opponents hand after offer
    int GetOpponentHandSizeAfterOffer(int opponentHandSize, Tuple<int[], int[]> offer)
    {
        opponentHandSize += offer.Item1.Sum();
        opponentHandSize -= offer.Item2.Sum();
        return opponentHandSize;
    }

    public void ObserveOfferResponse(int respondingPlayerID, Tuple<int[], int[]> offer, bool accepted, int opponentHandSize)
    {
        ++totalOffers[offer.Item1.Sum(), offer.Item2.Sum()];
        if (accepted) 
        {
            ++acceptedOffers[offer.Item1.Sum(), offer.Item2.Sum()];
            if (order >  0 && !opponentCardsUnknown)
                ChangeHandBeliefsTrade(offer, opponentHandSize);
        }
    }

    public Tuple<int[], int[]> FindBestOffer(int[] hand, int opponentHandSize)
    {
        int[] handCopy = hand.ToArray();
        // Use the iterative method to find an offer first as a baseline
        Tuple<int[], int[]> bestOffer = FindBestOfferIterative(hand, new int[] { 0, 0, 0, 0, 0 }, opponentHandSize);
        float maxScore = CalculateScore(hand, bestOffer, opponentHandSize);

        foreach (string buyable in catanData.buyables)
        {
            // Calcuulate resources needed to buy one more buyable
            while (CanBuy(buyable, handCopy))
                SubtractCost(buyable, ref handCopy);
            
            int[] neededToBuy = catanData.buyCosts[buyable].Zip(handCopy, (a, b) => a - b).ToArray();

            for (int i = 0; i < neededToBuy.Length; ++i)
                if (neededToBuy[i] < 0) neededToBuy[i] = 0;

            if (neededToBuy.Sum() > opponentHandSize)
                continue;

            // Use iterative method to calculate best score when buying one more buyable
            Tuple<int[], int[]> bestOfferPlus = FindBestOfferIterative(hand, neededToBuy, opponentHandSize);
            float score = CalculateScore(hand, bestOfferPlus, opponentHandSize);

            if (score > maxScore)
            {
                bestOffer = bestOfferPlus;
                maxScore = score;
            }
        }

        return bestOffer;
    }

    Tuple<int[], int[]> FindBestOfferIterative(int[] hand, int[] toReceive, int opponentHandSize)
    {
        int[] givenCards = new int[] { 0, 0, 0, 0, 0 };
        int[] receivedCards = toReceive.ToArray();
        int[] handCopy = hand.ToArray();
        int[] originalHand = hand.ToArray();
        int originalOpponentHandSize = opponentHandSize;
        opponentHandSize = originalOpponentHandSize - receivedCards.Sum();

        float bestScore = CalculateScore(hand, new Tuple<int[], int[]>(givenCards, receivedCards), opponentHandSize);
        Tuple<int[], int[]> bestOffer = new Tuple<int[], int[]>(givenCards.ToArray(), receivedCards.ToArray());

        while (true)
        {
            bool updated = false;
            // For every resource try if the score of the offer increases when adding it to the given or received cards
            for (int resource = 0; resource < catanData.GetResourceAmount(); ++resource)
            {
                if (handCopy[resource] > 0)
                {
                    ++givenCards[resource];
                    updated |= TryUpdateOffer(originalHand, givenCards, receivedCards, opponentHandSize, ref bestScore, ref bestOffer);
                    --givenCards[resource];
                }

                if (opponentHandSize > 0)
                {
                    ++receivedCards[resource];
                    updated |= TryUpdateOffer(originalHand, givenCards, receivedCards, opponentHandSize, ref bestScore, ref bestOffer);
                    --receivedCards[resource];
                }
            }

            if (updated)
            {
                givenCards = bestOffer.Item1;
                receivedCards = bestOffer.Item2;
                opponentHandSize = originalOpponentHandSize - receivedCards.Sum();
                handCopy = originalHand.Zip(receivedCards, (a, b) => a + b).ToArray();
                handCopy = originalHand.Zip(givenCards, (a, b) => a - b).ToArray();
            }
            else
            {
                break;
            }
        }

        return bestOffer;
    }

    bool TryUpdateOffer(int[] originalHand, int[] givenCards, int[] receivedCards, int opponentHandSize, ref float bestScore, ref Tuple<int[], int[]> bestOffer)
    {
        float score = CalculateScore(originalHand, new Tuple<int[], int[]>(givenCards, receivedCards), opponentHandSize);
        if (score > bestScore)
        {
            bestScore = score;
            bestOffer = new Tuple<int[], int[]>(givenCards.ToArray(), receivedCards.ToArray());
            return true;
        }
        return false;
    }

    // Calculates score of an offer
    public float CalculateScore(int[] hand, Tuple<int[], int[]> offer, int opponentHandSize)
    {
        int[] newHand = GetHandAfterOffer(hand, offer);
        // Higher order agents take the guessed opponent hand into account when calculating the score of an offer
        if (order > 0)
        {
            Tuple<int[], int[]> offerToOpponent = Tuple.Create(offer.Item2, offer.Item1);
            int[] newOpponentHand = GetHandAfterOffer(opponentHandGuess, offerToOpponent);
            return (CalculateUtility(newHand) - CalculateUtility(newOpponentHand) + 30) * GetAcceptanceRate(offer);
        }
        else
            return CalculateUtility(newHand) * GetAcceptanceRate(offer);
    }

    public int CalculateUtility(int[] cards)
    {
        HashSet<List<string>> possibleBuys = GeneratePossibleBuys(cards);

        int maxUtility = 0;
        foreach (List<string> buyCombination in possibleBuys)
        {
            int utility = 0;
            int[] cardsCopy = cards;

            foreach (string buyable in buyCombination)
            {
                utility += catanData.utilityMap[buyable];
                SubtractCost(buyable, ref cardsCopy);
            }

            // Add score for each card left in the hand
            utility += cardsCopy.Sum() * catanData.utilityMap["Card"];

            if (utility > maxUtility)
            {
                maxUtility = utility;
            }
        }

        return maxUtility;
    }

    private HashSet<List<string>> GeneratePossibleBuys(int[] cards)
    {
        HashSet<List<string>> set = new HashSet<List<string>>();
        List<string> currentList = new List<string>();

        // Function to recursively generate combinations
        void GenerateCombinations(int[] remainingCards, List<string> combination)
        {
            // If no more cards left, add current combination to set
            if (remainingCards.All(card => card == 0))
            {
                set.Add(new List<string>(combination));
                return;
            }

            // For each buyable, try to buy it and go back after
            foreach (string toBuy in catanData.buyables)
            {
                if (CanBuy(toBuy, remainingCards))
                {
                    SubtractCost(toBuy, ref remainingCards);
                    combination.Add(toBuy);
                    GenerateCombinations(remainingCards, combination);
                    AddCost(toBuy, ref remainingCards);
                    combination.RemoveAt(combination.Count - 1);
                }
            }

            set.Add(new List<string>(combination));
            return;
        }

        // Start the recursion
        GenerateCombinations(cards, currentList);

        return set;
    }

    public float GetAcceptanceRate(Tuple<int[], int[]> offer)
    {
        int cardsMinus = offer.Item1.Sum();
        int cardsPlus = offer.Item2.Sum();
        float acceptanceRate = (float)acceptedOffers[cardsMinus, cardsPlus] / (float)totalOffers[cardsMinus, cardsPlus];
        // has not been accepted enough
        if (acceptanceRate < 0.05f)
            return 0;

        if (order > 0)
        {
            for (int resource = 0; resource < catanData.GetResourceAmount(); ++resource)
            {
                int askingForAmount = offer.Item2[resource];
                string resourceName = catanData.resources[resource];

                // opponent can not accept offer
                if (opponentHandBeliefs[resourceName].Item2 < askingForAmount)
                    return 0;

                // opponent has this card to spare
                if (opponentOffer.Item1[resource] > 0)
                {
                    // we want to give
                    if (offer.Item1[resource] > 0)
                        acceptanceRate *= 0.5f;
                    
                    // we want to receive
                    if (offer.Item2[resource] > 0)
                        acceptanceRate *= 1.2f;
                }

                // opponent wants this card
                if (opponentOffer.Item2[resource] > 0 && offer.Item1[resource] > 0)
                {
                    // we want to give
                    if (offer.Item1[resource] > 0)
                        acceptanceRate = (acceptanceRate + 0.2f) * 1.6f;
                    // we want to receive
                    if (offer.Item2[resource] > 0)
                        acceptanceRate *= 0.2f;
                }
            }
        }
        return acceptanceRate;
    }
    // values tested:   0.2     1.3     2.0     0.2:    a0: 3016 a1: 3048
    //                  0.3     1.2     1.9     0.3:    a0: 3235 a1: 3083 a0: 3123 a1: 2689 a0: 3295 a1: 3052
    //                  0.4     1.2     1.8     0.4:    a0: 3214 a1: 3225
    //                  0.3     1.3     1.8     0.3:    a0: 3316 a1: 3139
    //                  0.3     1.6     2.5     0.3:    a0: 3220 a1: 3083
    //                  0.3     1.2     3.5     0.3:    a0: 3064 a1: 3089 
    //                  0.3     1.2     0.2*1.5 0.3:    a0: 3256 a1: 2989  
    //                  0.3     1.2     0.2*1.6 0.3:    a0: 4536 a1: 4126
    //                  0.3     1.2     0.2*1.7 0.3:    a0: 4483 a1: 4267
    //                  0.25    1.2     0.2*1.6 0.25:   a0: 3606 a1: 3224 a0: 12277 a1: 11870
    //                  0.25    1.2     0.3*1.4 0.25:   a0: 3336 a1: 3457
    //                  0.25    1.2     0.2*1.55 0.25:  bad
    //                  0.3     1.2     0.2*1.55 0.3:   a0: 3518 a1: 2846 a0: 3727 a1: 3630 a0: 3359 a1: 3202
    //                  0.3     1.3     2        0.3:   a0: 9854 a1: 9591
    //                  0.3     1.3     0.2*1.7  0.3:   a0: 10695 a1: 10582
    //                  1       1       0.2*1.6  0.2:   a0: 9384 a1: 9228 
    //                  0.5     1       0.2*1.6  0.2:   a0: 10555 a1: 9690  1.089 a0: 21026 a1: 20153 1.043
    //                  0.1     1       0.2*1.6  0.2:   a0: 10404 a1: 10454
    //                  0.5     1.5     0.2*1.6  0.2:   a0: 89804 a1: 86558 1.037
    //                  0.5     1.1     0.2*1.6  0.2:   a0: 48549 a1: 47572 1.021

    // track if trade happened in round
    void FindOpponentDesires(int opponentHandSize)
    {
        int[] opponentHand = new int[5] { 0, 0, 0, 0, 0 };
        for (int resource = 0; resource < catanData.GetResourceAmount(); ++resource)
        {
            opponentHand[resource] = opponentHandBeliefs[catanData.resources[resource]].Item1;
        }

        for (int i = 0; opponentHand.Sum() < opponentHandSize && i < 25; ++i)
        {
            int randomCard = UnityEngine.Random.Range(0, 5);
            if (opponentHand[randomCard] < opponentHandBeliefs[catanData.resources[randomCard]].Item2)
                ++opponentHand[randomCard];
        }

        opponentHandGuess = opponentHand;
        opponentOffer = opponentModel.FindBestOffer(opponentHand, opponentHandSize);
    }

    public int[] GetHandAfterOffer(int[] hand, Tuple<int[], int[]> offer)
    {
        hand = hand.Zip(offer.Item1, (a, b) => a - b).ToArray();
        hand = hand.Zip(offer.Item2, (a, b) => a + b).ToArray();
        return hand;
    }

    #region Print Functions
    /*
    Debug.Log("Possible Buys:");
    foreach (List<string> buyCombination in possibleBuys)
    {
        Debug.Log(string.Join(", ", buyCombination));
    }
    */
    /*
    void PrintBeliefs()
    {
        foreach (var kvp in opponentHandBeliefs)
        {
            Debug.Log($"{kvp.Key}, range: ({kvp.Value.Item1}, {kvp.Value.Item2})");
        }
    }
    */
    #endregion

    #region Buying Helper Functions
    bool CanBuy(string buyable, int[] cards)
    {
        // Use LINQ function to check if all elements of cards are greater or equal to the cost
        return cards.Zip(catanData.buyCosts[buyable], (a, b) => a >= b).All(result => result);
    }

    void SubtractCost(string buyable, ref int[] cards)
    {
        // Use LINQ function to subtract the costs from the cards
        cards = cards.Zip(catanData.buyCosts[buyable], (a, b) => a - b).ToArray();
    }

    void AddCost(string buyable, ref int[] cards)
    {
        // Use LINQ function to add the costs to the cards
        cards = cards.Zip(catanData.buyCosts[buyable], (a, b) => a + b).ToArray();
    }
    #endregion
}