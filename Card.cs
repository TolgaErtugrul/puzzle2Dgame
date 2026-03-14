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
}
