using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

public class GameManager : MonoBehaviour
{
    #region Declarations
    private List<int[]> hands = new List<int[]>(4);
    private List<List<int[]>> perceivedHands = new List<List<int[]>>(4);

    public HandsChangedEvent handsChangedEvent;
    public OfferEvent offerEvent;
    public CatanData catanData;
    private Model managerModel;

    private int turn = 0;
    private bool newRound = false;
    private bool play = false;
    private const int MaxRounds = 5000;
    private int roundsPlayed = 0;
    private int agent0Score;
    private int agent1Score;
    private int agent0totalScore = 0;
    private int agent1totalScore = 0;
    private StreamWriter scoreWriter;
    #endregion

    #region Offer Matrices
    private int[,] totalOffers0 = new int[8, 8]
    {
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0}
    };

    private int[,] acceptedOffers0 = new int[8, 8]
    {
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0}
    };

    private int[,] totalOffers1 = new int[8, 8]
    {
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0}
    };

    private int[,] acceptedOffers1 = new int[8, 8]
    {
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0}
    };
    #endregion

    #region Event Subscription
    private void OnEnable()
    {
        offerEvent.OnOfferResponse += ProcessTrade;
    }

    private void OnDisable()
    {
        offerEvent.OnOfferResponse -= ProcessTrade;
    }
    #endregion

    void Awake()
    {
        managerModel = new Model(catanData, -1, 0);
        for (int i = 0; i < 4; ++i)
        {
            hands.Add(new int[] { 0, 0, 0, 0, 0 });
            perceivedHands.Add(new List<int[]>());
        }
        scoreWriter = new StreamWriter("AgentScoresClosed10.csv");
        scoreWriter.WriteLine("Round,Agent1Score,Agent2Score");  // CSV header
    }

    public void DealCards()
    {
        RemoveCards(false);
        GiveRandomHand(0);
        GiveRandomHand(1);
        GeneratePercievedHands(0);
        GeneratePercievedHands(1);
        handsChangedEvent.NewHands(perceivedHands, true);
    }

    // Automatically plays rounds untill disabled
    public void Play()
    {
        play = !play;
        StartCoroutine(PlaySlow());
    }

    // Used to have some time between actions
    IEnumerator PlaySlow()
    {
        while (play)
        {
            DealCards();
            yield return new WaitForSeconds(0.005f);
            PlayRound();
            yield return new WaitForSeconds(0.002f);
        }
    }

    // Auutomatically play one round (untill an agent no longer wants to trade)
    public void PlayRound()
    {
        newRound = false;
        StartCoroutine(PlayRoundSlow());
    }

    // Used to have some time between actions
    IEnumerator PlayRoundSlow()
    {
        while (!newRound) 
        { 
            offerEvent.StartOffer(turn);
            yield return new WaitForSeconds(0.005f);
        }
    }

    // Tell agent to make an offer
    public void MakeOffer()
    {
        newRound = false;
        offerEvent.StartOffer(turn);
    }

    // Process the trade, wether to end the round or trade cards if accepted. Offer is from perspective of sender
    void ProcessTrade(int responderID, int senderID, Tuple<int[], int[]> offer, bool accepted)
    {
        // Swap turn after every offer made
        turn = 1 - turn;

        // No offer is better than keeping hand, agent wants to stop trading
        if (offer.Item1.Sum() + offer.Item2.Sum() == 0)
        {
            newRound = true;
            agent0Score = managerModel.CalculateUtility(hands[0]);
            agent1Score = managerModel.CalculateUtility(hands[1]);
            agent0totalScore += agent0Score;
            agent1totalScore += agent1Score;
            roundsPlayed++;
            scoreWriter.WriteLine($"{roundsPlayed},{agent0Score},{agent1Score}");

            Debug.Log("a0: " + agent0totalScore + " a1: " + agent1totalScore);

            if(roundsPlayed >= MaxRounds)
            {
                play = false;
                WriteMatrices();
                scoreWriter.Close();
            }

            return;
        }

        if (senderID == 0)
            ++totalOffers0[offer.Item1.Sum(), offer.Item2.Sum()];
        else
            ++totalOffers1[offer.Item1.Sum(), offer.Item2.Sum()];

        if (accepted)
        {
            // track accepted offers
            if (senderID == 0)
                ++acceptedOffers0[offer.Item1.Sum(), offer.Item2.Sum()];
            else
                ++acceptedOffers1[offer.Item1.Sum(), offer.Item2.Sum()];
            TradeCards(responderID, senderID, offer);
        }
    }

    // Swap cards of the playes to complete the trade
    void TradeCards(int responderID, int senderID, Tuple<int[], int[]> offer)
    {
        hands[senderID] = hands[senderID].Zip(offer.Item1, (a, b) => a - b).ToArray();
        hands[senderID] = hands[senderID].Zip(offer.Item2, (a, b) => a + b).ToArray();
        hands[responderID] = hands[responderID].Zip(offer.Item2, (a, b) => a - b).ToArray();
        hands[responderID] = hands[responderID].Zip(offer.Item1, (a, b) => a + b).ToArray();

        RemoveCards(true);
        GeneratePercievedHands(0);
        GeneratePercievedHands(1);
        handsChangedEvent.NewHands(perceivedHands, false);
    }

    // Remove cards from lists
    void RemoveCards(bool onlyPerceived)
    {
        for (int i = 0; i < hands.Count; ++i)
        {
            if (!onlyPerceived)
            { 
                for (int j = 0; j < hands[i].Length; ++j)
                {
                    hands[i][j] = 0;
                }
            }
            perceivedHands[i].Clear();
        }
    }

    // Give player id a hand of 2 to 8 random cards                             note: 4 cus trades with under 4 cards seem lackluster
    void GiveRandomHand(int id)
    {
        int handSize = UnityEngine.Random.Range(4, 8);
        for (int i = 0; i < handSize; ++i)
        {
            ++hands[id][UnityEngine.Random.Range(0, 5)];
        }
    }

    public void WriteMatrices()
    {
        WriteMatrixToFile("accepted0.txt", acceptedOffers0);
        WriteMatrixToFile("accepted1.txt", acceptedOffers1);
        WriteMatrixToFile("total0.txt", totalOffers0);
        WriteMatrixToFile("total1.txt", totalOffers1);
    }

    public void WriteMatrixToFile(string filePath, int[,] matrix)
    {
        StreamWriter writer = new StreamWriter(filePath);

        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                writer.Write(matrix[i, j] + " ");
            }
            writer.WriteLine();
        }

        writer.Close();

        Debug.Log("Matrix data has been written to: " + filePath);
    }

    // Generate hands perceived by player id, 100% chance for each card of different player to be forgotten
    void GeneratePercievedHands(int id) 
    {
        float chanceToRemember = 0.0f;
        for (int i = 0; i < hands.Count; ++i)
        {
            int[] hand = new int[] { 0, 0, 0, 0, 0, 0 };
            for (int j = 0; j < hands[i].Length; ++j)
            {
                int resourceAmount = hands[i][j];
                for (int k = 0; k < resourceAmount; ++k)
                {
                    if (UnityEngine.Random.value < chanceToRemember || i == id)
                    {
                        ++hand[j];
                    }
                    else
                    {
                        ++hand[5];
                    }
                }
            }
            perceivedHands[id].Add(hand);
        }
    }

    // Print hands as perceived by player id
    void PrintPerceivedHands(int id)
    {
        if (id >= 0 && id < perceivedHands.Count)
        {
            Debug.Log("Perceived hand for player " + id + ":");
            foreach (var hand in perceivedHands[id])
            {
                string handString = "";
                for (int j = 0; j < hand.Length; ++j)
                {
                    handString += hand[j] + " ";
                }
                Debug.Log(handString);
            }
        }
        else
        {
            Debug.LogError("Invalid player ID: " + id);
        }
    }

}
