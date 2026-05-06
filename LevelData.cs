using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{
    public int levelID;
    public int rowCount;
    public int columnCount;
    public float timeLimit;
    public int bombPairID = -2; // -2: Yok, -1: Rastgele, 0+: Belirli ID
    public int bonusPairID = -2;

    [Header("Star Thresholds (Moves)")]
    public int threeStarMoveLimit; // Örn: 10 hamle ve altı 3 yıldız
    public int twoStarMoveLimit;   // Örn: 15 hamle ve altı 2 yıldız
}
