using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "MemoryGame/Level Data")]
public class LevelData : ScriptableObject
{
    public int levelNumber;
    public int rowCount;    // Satır sayısı
    public int columnCount; // Sütun sayısı
    public float timeLimit; // Bu seviye için verilen süre
}
