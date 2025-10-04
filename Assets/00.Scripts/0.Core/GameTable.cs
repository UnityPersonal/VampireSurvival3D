using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Game Table", fileName = "New Game Table")]
public class GameTable : ScriptableObject
{
    [Serializable]
    public struct GameTableData
    {
        public int DataID;
        public ScriptableObject Data;
    }
    
    public GameTableData[] table;
        
}