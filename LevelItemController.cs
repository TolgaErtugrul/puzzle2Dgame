using UnityEngine;
using TMPro;

public class LevelItemController : MonoBehaviour
{
    public TextMeshProUGUI extraTimeStockText;
    public TextMeshProUGUI cancelBombStockText;

    void OnEnable()
    {
        // Panel her açıldığında stokları güncelle
        UpdateStocks();
    }

    public void UpdateStocks()
    {
        extraTimeStockText.text = "x" + PlayerPrefs.GetInt("Inventory_ExtraTime", 0);
        cancelBombStockText.text = "x" + PlayerPrefs.GetInt("Inventory_CancelBomb", 0);
    }

    // Butonlara basıldığında GameManager'daki ilgili fonksiyonları çağıracağız
    public void UseExtraTime()
    {
        FindObjectOfType<GameManager>().PrepareExtraTime();
        UpdateStocks();
    }

    public void UseCancelBomb()
    {
        FindObjectOfType<GameManager>().PrepareCancelBomb();
        UpdateStocks();
    }
}
