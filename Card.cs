using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [Header("UI References")]
    public Image frontImage; // Kartın ön yüzündeki ikon
    public GameObject backVisual; // Kartın arka kapak görseli

    private int _cardID; // Hangi kartla eşleşeceğini bilmek için
    private bool _isFlipped = false;
    private bool _isMatched = false;

    // Kartın ID'sini ve görselini ayarlayan fonksiyon
    public void SetupCard(int id, Sprite icon)
    {
        _cardID = id;
        frontImage.sprite = icon;
        
        // Başlangıçta kart kapalı olsun
        HideCard();
    }

    public void ShowCard()
    {
        if (_isMatched || _isFlipped) return;
        
        _isFlipped = true;
        backVisual.SetActive(false); // Arka kapağı gizle, ön yüz görünsün
    }

    public void HideCard()
    {
        _isFlipped = false;
        backVisual.SetActive(true); // Arka kapağı göster
    }

    private IEnumerator FlipRoutine(bool showingFront)
    {
        float duration = 0.2f;
        float elapsed = 0f;
    
        // 1. Kartı daralt (Kapanıyormuş gibi)
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float scale = Mathf.Lerp(1, 0, elapsed / duration);
            transform.localScale = new Vector3(scale, 1, 1);
            yield return null;
        }
    
        // 2. Tam bu noktada görseli değiştir
        backVisual.SetActive(!showingFront);
    
        // 3. Kartı tekrar genişlet (Açılıyormuş gibi)
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float scale = Mathf.Lerp(0, 1, elapsed / duration);
            transform.localScale = new Vector3(scale, 1, 1);
            yield return null;
        }
    }
}
