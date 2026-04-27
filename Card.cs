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
    public bool _isMatched = false;

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
        // 🔥 DÜZELTME: Eğer GameManager bir işlem yapıyorsa (kartları kapatıyor, eşleşme bakıyor vb.) 
        // veya bu kart zaten eşleşmişse tıklamaya izin verme!
        if (GameManager.Instance.IsSystemBusy() || _isMatched || _isFlipped) return;
        
        _isFlipped = true;
        backVisual.SetActive(false); 
    
        GameManager.Instance.OnCardFlipped(this);
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
        
        // Objenin kendisini kapatmak yerine (SetActive(false)) 
        // sadece görsellerini ve tıklanabilirliğini kapatıyoruz.
        // Böylece hiyerarşide yer kaplamaya devam eder ve Grid kaymaz.
        
        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = 0;
            cg.interactable = false;
            cg.blocksRaycasts = false;
        }
    }
    
    public void PlayMatchAnimation()
    {
        StartCoroutine(MatchAnimRoutine());
    }
    
    private IEnumerator MatchAnimRoutine()
    {
        float duration = 0.3f;
        Vector3 targetScale = Vector3.one * 1.2f;
        Vector3 originalScale = Vector3.one;
    
        // Büyüme
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / duration);
            yield return null;
        }
        // Küçülme
        elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsed / duration);
            yield return null;
        }
    }
}
