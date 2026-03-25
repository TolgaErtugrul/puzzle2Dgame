using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Diğer kodlardan kolayca erişmek için

    [Header("Game Settings")]
    public List<Card> allCards = new List<Card>();

    [Header("UI & Logic")]
    public int totalPairs;       // Bu seviyedeki toplam çift sayısı
    private int _matchedPairs;    // Şu ana kadar bulunan çift sayısı
    
    private Card _firstSelected;
    private Card _secondSelected;
    private bool _isProcessing = false; // İki kart kontrol edilirken dokunmayı engellemek için
    private float _currentTime;
    private bool _isTimerRunning = false;

    public CanvasGroup winPanelGroup;

    void Awake()
    {
        Instance = this;
    }
    
    void Update()
    {
        if (_isTimerRunning)
        {
            if (_currentTime > 0)
            {
                _currentTime -= Time.deltaTime;
                // Burada UI'daki süreyi güncelleyeceğiz
            }
            else
            {
                _currentTime = 0;
                _isTimerRunning = false;
                GameOver(); // Süre bitti!
            }
        }
    }

    public void OnCardFlipped(Card flippedCard)
    {
        if (_isProcessing) return;

        if (_firstSelected == null)
        {
            _firstSelected = flippedCard;
        }
        else
        {
            _secondSelected = flippedCard;
            StartCoroutine(CheckMatchRoutine());
        }
    }

    private IEnumerator CheckMatchRoutine()
    {
        _isProcessing = true;
    
        // Kısa bir bekleme (oyuncunun ikinci kartı görmesi için)
        yield return new WaitForSeconds(0.5f);
    
        if (_firstSelected.GetID() == _secondSelected.GetID())
        {
            // ✅ EŞLEŞME OLDU
            _firstSelected.SetMatched();
            _secondSelected.SetMatched();
            // İleride buraya kartları yok etme veya efekt ekleme gelecek
        }
        else
        {
            // ❌ EŞLEŞME YOK
            yield return new WaitForSeconds(0.5f);
            _firstSelected.HideCard();
            _secondSelected.HideCard();
        }
    
        // Seçimleri temizle ve yeni hamleye izin ver
        _firstSelected = null;
        _secondSelected = null;
        _isProcessing = false;
    }

    private void ShuffleCards(List<Card> cardsToShuffle)
    {
        for (int i = cardsToShuffle.Count - 1; i > 0; i--)
        {
            // 0 ile i arasında rastgele bir index seç
            int randomIndex = Random.Range(0, i + 1);
    
            // Elemanların yerini değiştir (Swap)
            Card temp = cardsToShuffle[i];
            cardsToShuffle[i] = cardsToShuffle[randomIndex];
            cardsToShuffle[randomIndex] = temp;
        }
    }

    public void SetupGrid()
    {
        ShuffleCards(allCards);
    
        // Karıştırılmış listeye göre hiyerarşideki sıralamayı güncelle
        for (int i = 0; i < allCards.Count; i++)
        {
            allCards[i].transform.SetSiblingIndex(i);
        }
    }

    public IEnumerator StartGameSequence()
    {
        _isProcessing = true; // Oyuncunun kartlara dokunmasını engelle
    
        // 1. Kartları karıştır ve diz
        SetupGrid();
    
        // 2. Tüm kartları aç
        foreach (Card card in allCards)
        {
            card.ShowCard();
        }
    
        // 3. 2 saniye bekle
        yield return new WaitForSeconds(2f);
    
        // 4. Tüm kartları kapat
        foreach (Card card in allCards)
        {
            card.HideCard();
        }
    
        _isProcessing = false; // Artık oyuncu dokunabilir
    }

    private IEnumerator CheckMatchRoutine()
{
    _isProcessing = true;
    yield return new WaitForSeconds(0.5f);

    if (_firstSelected.GetID() == _secondSelected.GetID())
    {
        // ✅ EŞLEŞME OLDU
        _firstSelected.SetMatched();
        _secondSelected.SetMatched();
        
        _matchedPairs++; // Bir çift daha bulundu!

        // Tüm çiftler bulundu mu?
        if (_matchedPairs >= totalPairs)
        {
            WinGame();
        }
    }
    else
    {
        // ❌ EŞLEŞME YOK
        yield return new WaitForSeconds(0.5f);
        _firstSelected.HideCard();
        _secondSelected.HideCard();
    }

    _firstSelected = null;
    _secondSelected = null;
    _isProcessing = false;
    }
    
    public void WinGame()
    {
        _isTimerRunning = false;
        StartCoroutine(FadeInPanel(winPanelGroup));
    }
    
    public void GameOver()
    {
        _isTimerRunning = false;
        Debug.Log("Süre doldu! Kaybettin.");
        // Burada "Tekrar Dene" menüsünü açabilirsin
    }

    private IEnumerator FadeInPanel(CanvasGroup cg)
    {
        float duration = 0.5f; // yarım saniyede açılsın
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(0, 1, elapsed / duration);
            yield return null;
        }
    }
}
