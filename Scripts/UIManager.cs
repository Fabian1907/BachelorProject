using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject[] cardPrefabs = new GameObject[5];
    [SerializeField] private GameObject[] unknownCardPrefabs = new GameObject[5];
    [SerializeField] private RectTransform[] handTransforms = new RectTransform[4];
    private List<GameObject> cardObjects = new List<GameObject>();

    private List<List<int[]>> perceivedHands = new List<List<int[]>>(4);

    public HandsChangedEvent handsChangedEvent;

    #region Event Subscription
    private void OnEnable()
    {
        handsChangedEvent.OnHandsChanged += ChangedHands;
    }

    private void OnDisable()
    {
        handsChangedEvent.OnHandsChanged -= ChangedHands;
    }
    #endregion

    void Awake()
    {
        for (int i = 0; i < 4; ++i)
        {
            perceivedHands.Add(new List<int[]>());
        }
    }

    void ChangedHands(List<List<int[]>> newHands, bool newTurn)
    {
        DestroyCards();
        perceivedHands = newHands;
        DisplayHands();
    }

    void DisplayHands()
    {
        DisplayHand(0, 0);
        DisplayHand(1, 1);
    }

    // Display hand of player id as perceived by player perceiveId
    void DisplayHand(int id, int perceiveId)
    {
        int[] hand = perceivedHands[id][id];
        RectTransform handTransform = handTransforms[id];
        int handSize = perceivedHands[id][id].Sum();
        int cardCount = 0;

        for (int i = 0; i < hand.Length; ++i)
        {
            for (int j = 0; j < hand[i]; ++j)
            {
                // Choose prefab
                GameObject prefab;
                if (j < perceivedHands[perceiveId][id][i])
                {
                    prefab = cardPrefabs[i];
                }
                else
                {
                    prefab = unknownCardPrefabs[i];
                }

                // Instantiate card prefab at the hand position
                GameObject newCardObject = Instantiate(prefab, handTransform);

                // Offset the card
                RectTransform cardRectTransform = newCardObject.GetComponent<RectTransform>();
                float offsetX = cardCount * 110f - (handSize - 1) * 55f;
                cardRectTransform.localPosition = handTransform.anchoredPosition + new Vector2(offsetX, 0f);

                // Add the card to the list of card objects to delete later
                cardObjects.Add(newCardObject);
                ++cardCount;
            }
        }
    }

    // Destroy card gameObjects in scene
    void DestroyCards()
    {
        foreach (var card in cardObjects)
        {
            Destroy(card);
        }
        cardObjects.Clear();
    }

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
