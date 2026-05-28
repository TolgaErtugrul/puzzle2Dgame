using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelItemController : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI extraTimeStockText;
    public TextMeshProUGUI cancelBombStockText;
    
    [Header("Buttons")]
    public Button extraTimeBtn;
    public Button cancelBombBtn;

    // Panel her SetActive(true) yapıldığında bu çalışır
    void OnEnable()
    {
        UpdateItemButtons();
    }

    public void UpdateItemButtons()
    {
        int extraTimeCount = PlayerPrefs.GetInt("Inventory_ExtraTime", 0);
        int cancelBombCount = PlayerPrefs.GetInt("Inventory_CancelBomb", 0);

        // Metinleri güncelle
        extraTimeStockText.text = "x" + extraTimeCount;
        cancelBombStockText.text = "x" + cancelBombCount;

        // Stok 0 ise butonu tıklanamaz yap, değilse aç
        extraTimeBtn.interactable = extraTimeCount > 0;
        cancelBombBtn.interactable = cancelBombCount > 0;
    }

    public void UseExtraTime()
    {
        // GameManager'a "Hazırlan" komutu ver
        FindObjectOfType<GameManager>().PrepareExtraTime();
        UpdateItemButtons(); // Sayıyı düşürmek için UI'ı yenile
    }

    public void UseCancelBomb()
    {
        FindObjectOfType<GameManager>().PrepareCancelBomb();
        UpdateItemButtons();
    }
}
