using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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
        float duration = 0.4f; // 0.2f çok hızlıydı, 0.4f idealdir
        float elapsed = 0f;
    
        // 1. Kartı Y ekseninde küçültürken biraz yukarı "zıplatma" (Opsiyonel derinlik hissi)
        while (elapsed < duration / 2)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (duration / 2);
            float scale = Mathf.Lerp(1, 0, t);
            transform.localScale = new Vector3(scale, 1.1f, 1); // Hafifçe uzasın
            yield return null;
        }
    
        // Görsel değişimi tam 90 derecedeyken (scale 0 iken) yapıyoruz
        backVisual.SetActive(!showingFront);
    
        elapsed = 0f;
        // 2. Kartı tekrar genişlet
        while (elapsed < duration / 2)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (duration / 2);
            float scale = Mathf.Lerp(0, 1, t);
            transform.localScale = new Vector3(scale, 1, 1);
            yield return null;
        }
    }

    // GameManager'ın ID'yi okuyabilmesi için gerekli
    public int GetID()
    {
        return _cardID;
    }
    
    // Kart eşleştiğinde çağrılacak fonksiyon
    public void SetMatched()
    {
        _isMatched = true;
        _isFlipped = true; // Kartın açık kalmasını sağlar
        
        // Eşleşen kartları sahneden silmek yerine görünmez yapabiliriz
        // Ya da bir animasyon oynatıp sonra kapatabiliriz
        StartCoroutine(MatchAnimationRoutine());
    }
    
    private IEnumerator MatchAnimationRoutine()
    {
        yield return new WaitForSeconds(0.3f);
        // Kartı yavaşça küçülterek yok etme efekti
        float elapsed = 0f;
        while (elapsed < 0.3f)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, elapsed / 0.3f);
            yield return null;
        }
        gameObject.SetActive(false); // Kartı tamamen gizle
    }
}
