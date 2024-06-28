using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CatanData", menuName = "Catan/CatanData")]
public class CatanData : ScriptableObject
{
    public Dictionary<string, int[]> buyCosts = new Dictionary<string, int[]>();
    public Dictionary<string, int> utilityMap = new Dictionary<string, int>();
    public const int WHEAT = 0, ORE = 1, SHEEP = 2, BRICK = 3, WOOD = 4;
    public string[] resources;
    public string[] buyables;

    public float[,] buyOverlap = new float[5, 5]
    {     //wh      or      sh      br      wo
        {   0.33f,  0.67f,  0.67f,  0.33f,  0.33f   },      // wh
        {   1f,     0.5f,   0.5f,   0f,     0f      },      // or
        {   1f,     0.5f,   0f,     0.5f,   0.5f    },      // sh
        {   0.5f,   0f,     0.5f,   0f,     1f      },      // br
        {   0.5f,   0f,     0.5f,   1f,     0f      }       // wo
    };

    // Initialize the costs in OnEnable or Awake
    private void OnEnable()
    {
        // resources ordered from approximate best to worst
        resources  = new string[] { "Wheat", "Ore", "Sheep", "Brick", "Wood" };
        buyables    = new string[] { "Settlement", "Road", "City", "Development Card" };

        buyCosts.Clear();

        // Add item costs                              wh or sh br wo
        buyCosts.Add("Settlement",         new int[] { 1, 0, 1, 1, 1 }); 
        buyCosts.Add("Road",               new int[] { 0, 0, 0, 1, 1 }); 
        buyCosts.Add("City",               new int[] { 2, 3, 0, 0, 0 }); 
        buyCosts.Add("Development Card",   new int[] { 1, 1, 1, 0, 0 });

        // Utility values for the basic game elements
        utilityMap.Add("Settlement",        30);
        utilityMap.Add("Road",              10);
        utilityMap.Add("City",              40);
        utilityMap.Add("Development Card",  20);
        utilityMap.Add("Card",              3);
    }

    public int GetResourceAmount()
    {
        return resources.Length;
    }
}
