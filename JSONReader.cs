using UnityEngine;
using System.Collections.Generic;

public class JSONReader : Singleton<JSONReader> {
    public bool isLoaded = false;
    public TextAsset textJSON;

    [System.Serializable]
    public class CardData {
        public string name;
        public int id;
        public string type;
        public int power;
        public string description;
        public int attackmod;
        public int defensemod;
        public int healthmod;
        public string effect;
    }

    [System.Serializable]
    public class CardDataList 
    {
        public CardData[] carddata;
    }

    public CardDataList cardDataList = new CardDataList();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (textJSON != null)
        {
            cardDataList = JsonUtility.FromJson<CardDataList>(textJSON.text);
            Debug.Log("Card Data Loaded Successfully");
            isLoaded = true;
        }
        else
        {
            Debug.LogError("JSON file path is not set.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
