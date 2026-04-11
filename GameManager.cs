using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro; 
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI & Panels")]
    public CanvasGroup winPanelGroup;
    
    [Header("Game Settings")]
    public List<Card> allCards = new List<Card>();
    public int totalPairs;

    [Header("Level Generation")]
    public LevelData currentLevel; // Oluşturduğun Level_1 dosyasını buraya sürükle
    public GameObject cardPrefab;  // Prefab klasöründeki Card objesini buraya sürükle
    public Transform cardGridParent; // Hierarchy'deki CardGrid objesini buraya sürükle
    public List<Sprite> cardIcons; // Kartların üzerine gelecek resimler

    [Header("Score System")]
    public TextMeshProUGUI moveText; // Sahnedeki hamle yazısını buraya sürükle
    private int _moveCount = 0;

    [Header("Level System")]
    public List<LevelData> levels; // Oluşturduğun Level_1, Level_2 gibi dosyaları buraya sürükle
    private int _currentLevelIndex = 0;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip flipSound;
    public AudioClip matchSound;
    public AudioClip failSound;

    [Header("Timer UI")]
    public TextMeshProUGUI timerText; // Hierarchy'deki geri sayım metnini buraya sürükle
    public CanvasGroup gameOverPanelGroup; // Yenilgi panelini buraya sürükle
    
    private Card _firstSelected;
    private Card _secondSelected;
    private bool _isProcessing = false;
    private int _matchedPairs = 0;
    private bool _isTimerRunning = false;
    private float _remainingTime;

    public TextMeshProUGUI bestMoveText;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (_timerActive)
        {
            _remainingTime -= Time.deltaTime;
    
            // Süreyi ekranda formatlı göster (Örn: 00:15)
            if (timerText != null)
            {
                timerText.text = "Süre: " + Mathf.CeilToInt(_remainingTime).ToString();
                
                // Süre 5 saniyeden az kalınca yazıyı kırmızı yapıp oyuncuyu uyarabilirsin
                if (_remainingTime < 5f) timerText.color = Color.red;
                else timerText.color = Color.white;
            }
    
            if (_remainingTime <= 0)
            {
                _remainingTime = 0;
                GameOver();
            }
        }
    }

    // Kart tıklandığında Card.cs tarafından çağrılır
    public void OnCardClicked(Card clickedCard)
    {
        // EĞER: Sistem işlem yapıyorsa VEYA kart zaten eşleşmişse VEYA aynı karta tekrar basılıyorsa -> İZİN VERME
        if (_isProcessing || clickedCard.IsMatched || clickedCard == _firstSelected) 
            return;
    
        clickedCard.ShowCard();
    
        if (_firstSelected == null)
        {
            _firstSelected = clickedCard;
        }
        else
        {
            _secondSelected = clickedCard;
            // İkinci kart seçildiği an tıklamayı KİLİTLE!
            _isProcessing = true; 
            StartCoroutine(CheckMatchRoutine());
        }
    }
    
    void UpdateUI()
    {
        moveText.text = "Hamle: " + _moveCount;
        // En iyi skoru göster (Varsayılan 0 ise hiç oynanmamış demektir)
        int best = PlayerPrefs.GetInt("BestScore_" + currentLevel.name, 0);
        bestMoveText.text = "En İyi: " + (best == 0 ? "-" : best.ToString());
    }

    private IEnumerator CheckMatchRoutine()
    {
        // Kartların dönme animasyonu bitene kadar kısa bir süre bekleyelim
        yield return new WaitForSeconds(0.4f); 
    
        if (_firstSelected.GetID() == _secondSelected.GetID())
        {
            // ✅ EŞLEŞME OLDU!
            _firstSelected.SetMatched();
            _secondSelected.SetMatched();
            _matchedPairs++;
            PlaySound(matchSound);
    
            // KRİTİK NOKTA: Eğer son çiftse, beklemeye girmeden saati DURDUR
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
            PlaySound(failSound);
        }
    
        _firstSelected = null;
        _secondSelected = null;

        // Eğer oyun bittiyse (WinGame çalıştıysa) kilidi açmamıza gerek yok
        if (_matchedPairs < totalPairs)
        {
            _isProcessing = false;
        }
    }

    public void WinGame()
    {
        _timerActive = false; // saati durdur
        _isTimerRunning = false; 
        
        Debug.Log("Oyun bitti, süre durduruldu: " + _remainingTime);
    
        if (winPanelGroup != null)
        {
            StartCoroutine(FadeInPanel(winPanelGroup));
        }

        void SaveBestScore()
        {
            string key = "BestScore_" + currentLevel.name;
            int currentBest = PlayerPrefs.GetInt(key, 0);
        
            if (currentBest == 0 || _moveCount < currentBest)
            {
                PlayerPrefs.SetInt(key, _moveCount);
                PlayerPrefs.Save();
            }
        }
    }

    private IEnumerator FadeInPanel(CanvasGroup cg)
    {
        cg.gameObject.SetActive(true);
        cg.interactable = true;
        cg.blocksRaycasts = true;
        float duration = 0.5f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(0, 1, elapsed / duration);
            yield return null;
        }
    }

    public void GenerateLevel()
    {
        if (currentLevel == null) return;
    
        // Grid ayarları
        GridLayoutGroup gridLayout = cardGridParent.GetComponent<GridLayoutGroup>();
        if (gridLayout != null)
        {
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = currentLevel.columnCount;
        }
    
        // ⚠️ DÜZELTME: Süreyi burada BAŞLATMIYORUZ, sadece tanımlıyoruz.
        _remainingTime = currentLevel.timeLimit; 
        _timerActive = false; // Henüz kapalı
        _matchedPairs = 0;
        _moveCount = 0;
        UpdateUI();
    
        // Temizlik ve Kart Oluşturma
        foreach (Transform child in cardGridParent) { Destroy(child.gameObject); }
        allCards.Clear();
    
        int totalCards = currentLevel.rowCount * currentLevel.columnCount;
        totalPairs = totalCards / 2;
    
        List<int> idList = new List<int>();
        for (int i = 0; i < totalPairs; i++) { idList.Add(i); idList.Add(i); }
        for (int i = 0; i < idList.Count; i++)
        {
            int temp = idList[i];
            int randomIndex = Random.Range(i, idList.Count);
            idList[i] = idList[randomIndex];
            idList[randomIndex] = temp;
        }
    
        for (int i = 0; i < totalCards; i++)
        {
            GameObject newCard = Instantiate(cardPrefab, cardGridParent);
            Card cardScript = newCard.GetComponent<Card>();
            if (idList[i] < cardIcons.Count)
                cardScript.SetupCard(idList[i], cardIcons[idList[i]]);
            allCards.Add(cardScript);
        }
    
        // 🚀 Tek bir coroutine başlatıyoruz
        StopAllCoroutines(); 
        StartCoroutine(ShowCardsAtStart());
    }

    public IEnumerator StartGameSequence()
    {
        _isProcessing = true; 
        yield return new WaitForSeconds(1f); // Kartların oluşması için kısa bir bekleme
    
        foreach (Card card in allCards)
        {
            card.ShowCard();
        }
    
        yield return new WaitForSeconds(2f); // 2 saniye oyuncu ezberlesin
    
        foreach (Card card in allCards)
        {
            card.HideCard();
        }
    
        _isProcessing = false;
    }

    public void LoadNextLevel()
    {
        _currentLevelIndex++;
    
        if (_currentLevelIndex < levels.Count)
        {
            // Bir sonraki leveli ayarla
            currentLevel = levels[_currentLevelIndex];
            
            // UI'ları ve sayaçları sıfırla
            _matchedPairs = 0;
            _moveCount = 0;
            UpdateUI();
            
            // WinPanel'i kapat
            winPanelGroup.alpha = 0;
            winPanelGroup.gameObject.SetActive(false);
    
            // Yeni gridi oluştur
            GenerateLevel();
            StartCoroutine(StartGameSequence());
        }
        else
        {
            Debug.Log("Tüm oyun bitti! Tebrikler.");
            // Burada bir "Tebrikler, tüm bölümleri bitirdin" paneli açılabilir.
        }
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null) audioSource.PlayOneShot(clip);
    }

    public void StartTimer(float seconds)
    {
        _remainingTime = seconds;
        _timerActive = true;
    }

    public void GameOver()
    {
        _timerActive = false;
        _isProcessing = true; // Oyuncunun daha fazla kart açmasını engelle
        
        Debug.Log("Süre Doldu! Kaybettin.");
        
        // Eğer bir yenilgi panelin varsa onu açalım
        if (gameOverPanelGroup != null)
        {
            StartCoroutine(FadeInPanel(gameOverPanelGroup));
        }
    }

    // --- REKLAM / DEVAM ET MANTIĞI ---
    public void WatchAdAndContinue()
    {
        // Reklam izleme kodu ileride buraya gelecek. 
        // Şimdilik butona basıldığı anda süreyi ekler:
        
        _remainingTime += 10f; // 10 saniye ekle
        _timerActive = true;   // Sayacı tekrar başlat
        _isProcessing = false; // Kartlara tıklamayı tekrar aç
        
        // Yenilgi panelini kapat
        if (gameOverPanelGroup != null)
        {
            gameOverPanelGroup.alpha = 0;
            gameOverPanelGroup.interactable = false;
            gameOverPanelGroup.blocksRaycasts = false;
            gameOverPanelGroup.gameObject.SetActive(false);
        }
        
        Debug.Log("Reklam izlendi, +10 saniye eklendi!");
    }
    
    // --- BÖLÜMÜ SIFIRLA MANTIĞI ---
    public void RestartLevel()
    {
        // Tüm değişkenleri sıfırla
        _matchedPairs = 0;
        _moveCount = 0;
        UpdateUI();
    
        // Paneli kapat
        if (gameOverPanelGroup != null)
        {
            gameOverPanelGroup.alpha = 0;
            gameOverPanelGroup.gameObject.SetActive(false);
        }
    
        // Seviyeyi yeniden oluştur ve süreyi başlat
        GenerateLevel();
        StartCoroutine(StartGameSequence());
    }

    private IEnumerator ShowCardsAtStart()
    {
        _isProcessing = true; 
        _timerActive = false; // Kartlar gösterilirken süre akmasın
    
        yield return new WaitForSeconds(0.5f);
    
        // Kartları aç (Card.cs'deki kontrolü bypass etmek için doğrudan görsele müdahale)
        foreach (var card in allCards)
        {
            card.backVisual.SetActive(false);
        }
    
        yield return new WaitForSeconds(1.5f);
    
        // Kartları kapat
        foreach (var card in allCards)
        {
            card.HideCard();
        }
    
        _isProcessing = false; // Tıklamaları aç
        _timerActive = true;   // SÜRE ŞİMDİ BAŞLASIN!
    }

    public void OnCardFlipped(Card flippedCard)
    {
        // 1. KİLİT: Eğer işlem yapılıyorsa veya kart zaten eşleşmişse basılmasın
        if (_isProcessing || flippedCard.IsMatched) return;
    
        // 2. KİLİT: Aynı karta üst üste basılmasın
        if (flippedCard == _firstSelected) return;
    
        if (_firstSelected == null)
        {
            _firstSelected = flippedCard;
            PlaySound(flipSound);
            _firstSelected.ShowCard();
        }
        else
        {
            _secondSelected = flippedCard;
            PlaySound(flipSound);
            _secondSelected.ShowCard();
    
            _moveCount++;
            UpdateUI();
    
            // 🔥 KRİTİK DÜZENLEME: İkinci kart seçildiği an kilidi kapatıyoruz!
            // Coroutine içine girmeden hemen önce burada true yapmalıyız.
            _isProcessing = true; 
            
            StartCoroutine(CheckMatchRoutine());
        }
    }

    private IEnumerator ShowCardsPreviewRoutine()
    {
        _isProcessing = true; // Oyuncu bu sırada basamasın
        _timerActive = false; // Sayaç henüz başlamasın
    
        yield return new WaitForSeconds(0.5f); // Kısa bir bekleme (sahne yüklensin)
    
        // Tüm kartları aç
        foreach (Card card in allCards)
        {
            card.ShowCard();
        }
    
        yield return new WaitForSeconds(1.5f); // Oyuncunun ezberlemesi için süre
    
        // Tüm kartları geri kapat
        foreach (Card card in allCards)
        {
            card.HideCard();
        }
    
        _isProcessing = false; // Tıklamaları aç
        _timerActive = true;   // Süreyi şimdi başlat!
    }

    public bool IsSystemBusy()
    {
        return _isProcessing;
    }

    StartCoroutine(ShowCardsAtStart());
}
