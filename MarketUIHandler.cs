using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MarketUIHandler : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI totalStarsText;
    public TextMeshProUGUI extraTimeCountText;
    public TextMeshProUGUI cancelBombCountText;

    [Header("Buttons")]
    public Button buyExtraTimeBtn;
    public Button buyCancelBombBtn;

    void Start()
    {
        UpdateMarketUI();
    }

    public void UpdateMarketUI()
    {
        // Toplam yıldızı güncelle
        int totalStars = PlayerPrefs.GetInt("TotalStars", 0);
        totalStarsText.text = totalStars.ToString();

        // Envanter miktarlarını güncelle
        extraTimeCountText.text = "Sahip olunan: " + PlayerPrefs.GetInt("Inventory_ExtraTime", 0);
        cancelBombCountText.text = "Sahip olunan: " + PlayerPrefs.GetInt("Inventory_CancelBomb", 0);

        // Butonların tıklanabilirliğini kontrol et (Yetersiz bakiye kontrolü)
        buyExtraTimeBtn.interactable = totalStars >= ShopManager.EXTRA_TIME_PRICE;
        buyCancelBombBtn.interactable = totalStars >= ShopManager.CANCEL_BOMB_PRICE;
    }

    public void OnClick_BuyExtraTime()
    {
        int stars = PlayerPrefs.GetInt("TotalStars", 0);
        
        if (stars >= ShopManager.EXTRA_TIME_PRICE)
        {
            if (ShopManager.BuyExtraTime())
            {
                UpdateMarketUI();
                // Büyüme efekti (B. maddesi)
                StartCoroutine(PunchScale(buyExtraTimeBtn.GetComponent<RectTransform>()));
            }
        }
        else
        {
            // Yetersiz bakiye: Yıldız metnini kırmızı yak söndür
            StartCoroutine(FlashRed(totalStarsText));
        }
    }

    public void OnClick_BuyCancelBomb()
    {
        if (ShopManager.BuyCancelBomb())
        {
            Debug.Log("Bomba İptali Satın Alındı!");
            UpdateMarketUI();
        }
    }

    public IEnumerator FlashRed(TextMeshProUGUI text)
    {
        Color originalColor = text.color;
        text.color = Color.red; // Yazıyı kırmızı yap
        yield return new WaitForSeconds(0.1f);
        text.color = originalColor;
        yield return new WaitForSeconds(0.1f);
        text.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        text.color = originalColor;
    }

    public IEnumerator PunchScale(RectTransform rect)
    {
        Vector3 originalScale = rect.localScale;
        rect.localScale = originalScale * 1.2f; // %20 büyüt
        yield return new WaitForSeconds(0.1f);
        rect.localScale = originalScale; // Eski haline getir
    }
}
