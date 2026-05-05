using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "MemoryGame/Level Data")]
public class LevelData : ScriptableObject
{
    public int levelNumber;
    public int rowCount;    // Satır sayısı
    public int columnCount; // Sütun sayısı
    public float timeLimit; // Bu seviye için verilen süre

    [Header("Star Thresholds (Moves)")]
    public int threeStarMoveLimit; // Örn: 4 kart için 2, 30 kart için 18
    public int twoStarMoveLimit;   // Örn: 4 kart için 4, 30 kart için 25
}
