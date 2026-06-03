using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class MarketUIHandler : MonoBehaviour
{
    [Header("UI Elements - Bakiyeler")]
    public TextMeshProUGUI totalStarsText;
    public TextMeshProUGUI extraTimeCountText;
    public TextMeshProUGUI cancelBombCountText;

    [Header("UI Elements - Fiyatlar")]
    public TextMeshProUGUI extraTimePriceText;
    public TextMeshProUGUI cancelBombPriceText;
    
    [Header("Buttons")]
    public Button buyExtraTimeBtn;
    public Button buyCancelBombBtn;

    void Start()
    {
        UpdateMarketUI();
    }

    public void UpdateMarketUI()
    {
        // Yıldız bakiyesi
        int stars = PlayerPrefs.GetInt("TotalStars", 0);
        totalStarsText.text = totalStars.ToString();
    
        // Adetler (Buradaki isimlerin PlayerPrefs kayıtlarınla aynı olduğundan emin ol)
        extraTimeCountText.text = "Adet: " + PlayerPrefs.GetInt("Inventory_ExtraTime", 0);
        cancelBombCountText.text = "Adet: " + PlayerPrefs.GetInt("Inventory_CancelBomb", 0);

        // Fiyat etiketlerini ShopManager'dan çekip yazdırıyoruz (Artık 999 yazmayacak)
        if(extraTimePriceText != null) extraTimePriceText.text = ShopManager.EXTRA_TIME_PRICE.ToString();
        if(cancelBombPriceText != null) cancelBombPriceText.text = ShopManager.CANCEL_BOMB_PRICE.ToString();
        
        // Butonların tıklanabilirliğini kontrol et (Yetersiz bakiye kontrolü)
        buyExtraTimeBtn.interactable = totalStars >= ShopManager.EXTRA_TIME_PRICE;
        buyCancelBombBtn.interactable = totalStars >= ShopManager.CANCEL_BOMB_PRICE;
    }

    public void OnClick_BuyExtraTime()
    {
        if (PlayerPrefs.GetInt("TotalStars", 0) >= ShopManager.EXTRA_TIME_PRICE)
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
        if (PlayerPrefs.GetInt("TotalStars", 0) >= ShopManager.CANCEL_BOMB_PRICE)
        {
            if (ShopManager.BuyCancelBomb())
            {
                UpdateMarketUI();
                StartCoroutine(PunchScale(buyCancelBombBtn.GetComponent<RectTransform>()));
            }
        }
        else
        {
            StartCoroutine(FlashRed(totalStarsText));
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
