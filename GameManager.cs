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

    [Header("New Rules UI")]
    public TextMeshProUGUI infoText;

    [Header("Language UI")]
    public Image languageButtonImage; // Butonun Image bileşeni
    public Sprite trFlag; // TR Bayrağı Sprite'ı
    public Sprite enFlag; // EN Bayrağı Sprite'ı

    [Header("Icon Collections")]
    public List<Sprite> fruitIcons;
    public List<Sprite> animalIcons;
    public List<Sprite> emojiIcons;
    public List<Sprite> techIcons;

    [Header("Special Sprites")]
    public Sprite bonusSprite; // Altın ikon veya +5 yazısı
    public Sprite bombSprite;  // Bomba ikonu
    private int _bonusPairID = -1;
    private int _bombPairID = -2;
    
    private Card _firstSelected;
    private Card _secondSelected;
    private bool _isProcessing = false;
    private int _matchedPairs = 0;
    private bool _isTimerRunning = false;
    private float _remainingTime;
    private int _comboCount = 0;

    public GameObject matchEffectPrefab;

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
    
            if (timerText != null)
            {
                // 🔥 BURASI KRİTİK: "Süre: " yerine LanguageManager kullanmalıyız
                timerText.text = LanguageManager.GetText("time") + Mathf.CeilToInt(_remainingTime).ToString();
                
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
        if (moveText != null)
            moveText.text = LanguageManager.GetText("moves") + _moveCount;
    
        if (timerText != null)
            timerText.text = LanguageManager.GetText("time") + Mathf.CeilToInt(_remainingTime);
    }

    private IEnumerator CheckMatchRoutine()
    {
        yield return new WaitForSeconds(0.4f);

        if (_firstSelected.GetID() == _secondSelected.GetID())
        {
            // ✅ EŞLEŞME OLDU
            int matchedID = _firstSelected.GetID();
            _matchedPairs++; // Önce sayacı artır ki son çift kontrolü yapabilelim
        
            if (matchedID == _bonusPairID)
            {
                _remainingTime += 5f;
                StartCoroutine(ShowLevelWarning(LanguageManager.GetText("bonus_match")));
            }
            else if (matchedID == _bombPairID)
            {
                // EĞER SON ÇİFT DEĞİLSE bombayı patlat
                if (_matchedPairs < totalPairs)
                {
                    StartCoroutine(ShowLevelWarning(LanguageManager.GetText("bomb_hit")));
                    // Eşleşmemiş tüm kartları kapat
                    foreach (var card in allCards)
                    {
                        if (!card._isMatched) card.HideCard();
                    }
                }
            }
            
            _comboCount++; // Kombo artar (x1, x2, x3...)
    
            if (_comboCount >= 2) // x2 ve sonrası için ödül ver
            {
                float bonus = _comboCount; // x2 ise 2sn, x3 ise 3sn...
                _remainingTime += bonus;
                StartCoroutine(ShowLevelWarning("COMBO x" + _comboCount + "! +" + bonus + "sn"));
            }
            
            _firstSelected.SetMatched();
            _secondSelected.SetMatched();
            _matchedPairs++;
            AudioManager.Instance.PlaySFX(matchSound);

            if (_matchedPairs >= totalPairs) WinGame();
        }
        else
        {
            // ❌ EŞLEŞME YOK
            _comboCount = 0; // Hata yapınca kombo sıfırlanır
            yield return new WaitForSeconds(0.5f);
            
            // 🔥 CEZA SİSTEMİ: Seviye 10'dan sonra (Index 10 ve sonrası) 3 saniye eksilt
            if (_currentLevelIndex >= 10) 
            {
                _remainingTime -= 3f;
                if (_remainingTime < 0) _remainingTime = 0;
                
                // Görsel geri bildirim: Süre metnini anlık kırmızı yap
                StartCoroutine(FlashTimerRed());
            }
    
            _firstSelected.HideCard();
            _secondSelected.HideCard();
            AudioManager.Instance.PlaySFX(failSound);
        }
    
        _firstSelected = null;
        _secondSelected = null;
        if (_matchedPairs < totalPairs) _isProcessing = false;

        CheckForShuffle();
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
        _comboCount = 0; 
        if (currentLevel == null) return;
    
        // 1. Tema Geçiş Yazısı (Sadece geçiş seviyelerinde)
        if (_currentLevelIndex % 10 == 0)
        {
            string themeKey = "";
            if (_currentLevelIndex == 0) themeKey = "theme_fruit";
            else if (_currentLevelIndex == 10) themeKey = "theme_animal";
            else if (_currentLevelIndex == 20) themeKey = "theme_emoji";
            else if (_currentLevelIndex == 30) themeKey = "theme_letter";
            else if (_currentLevelIndex == 40) themeKey = "theme_food";
    
            if(themeKey != "") StartCoroutine(ShowLevelWarning(LanguageManager.GetText(themeKey)));
        }
    
        // Grid ayarları
        GridLayoutGroup gridLayout = cardGridParent.GetComponent<GridLayoutGroup>();
        if (gridLayout != null)
        {
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = currentLevel.columnCount;
        }
    
        _remainingTime = currentLevel.timeLimit; 
        _timerActive = false; 
        _matchedPairs = 0;
        _moveCount = 0;
        UpdateUI();
    
        // Temizlik
        foreach (Transform child in cardGridParent) { Destroy(child.gameObject); }
        allCards.Clear();
    
        int totalCards = currentLevel.rowCount * currentLevel.columnCount;
        totalPairs = totalCards / 2;

        // 2.A. Özel Kart Atamaları
        _bonusPairID = (_currentLevelIndex >= 20) ? 0 : -1; // Seviye 20+ ise ilk çift bonustur
        _bombPairID = (_currentLevelIndex >= 40) ? 1 : -2;  // Seviye 40+ ise ikinci çift bombadır
        
        // ID Listesi oluşturma ve karıştırma
        List<int> idList = new List<int>();
        for (int i = 0; i < totalPairs; i++) { idList.Add(i); idList.Add(i); }
        for (int i = 0; i < idList.Count; i++)
        {
            int temp = idList[i];
            int randomIndex = Random.Range(i, idList.Count);
            idList[i] = idList[randomIndex];
            idList[randomIndex] = temp;
        }
    
        // 2. Kartları oluştururken seçtiğimiz 'activeIcons' listesini kullanıyoruz
        for (int i = 0; i < totalCards; i++)
        {
            GameObject newCard = Instantiate(cardPrefab, cardGridParent);
            Card cardScript = newCard.GetComponent<Card>();
            
            int currentID = idList[i];
            Sprite iconToUse;
    
            // Özel İkon Kontrolü
            if (currentID == _bonusPairID) iconToUse = bonusSprite;
            else if (currentID == _bombPairID) iconToUse = bombSprite;
            else iconToUse = activeIcons[currentID % activeIcons.Count];
    
            cardScript.SetupCard(currentID, iconToUse);
            allCards.Add(cardScript);
        }
    
        // 3. Coroutine yönetimi (Önce durdur, sonra başlat)
        StopAllCoroutines(); 
    
        // Seviye 10 (Index 9) uyarısı
        if (_currentLevelIndex == 10)
        {
            StartCoroutine(ShowLevelWarning(LanguageManager.GetText("warning_intro")));
        }
    
        StartCoroutine(ShowCardsAtStart());
    }
    
    // Yardımcı Fonksiyon: Seviyeye göre ikon listesi döndürür
    List<Sprite> GetIconsForCurrentLevel()
    {
        if (_currentLevelIndex < 10) return fruitIcons;
        if (_currentLevelIndex < 20) return animalIcons;
        if (_currentLevelIndex < 30) return emojiIcons;
        if (_currentLevelIndex < 40) return letterIcons; // Alfabe/Sayı listesi
        return emojiIcons; // 40 ve sonrası için
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
        if (_currentLevelIndex < levels.Count - 1)
        {
            _currentLevelIndex++;
            currentLevel = levels[_currentLevelIndex];
        }
        else
        {
            // 18. seviye bittiyse: Seviyeyi başa sarma, ama süreyi %20 azalt!
            Debug.Log("Master Mode: Aynı seviye, daha az süre!");
            currentLevel = levels[_currentLevelIndex]; // Son seviyede kal
            currentLevel.timeLimit *= 0.8f; // Süreyi her seferinde daralt (Dikkat: ScriptableObject'i kalıcı bozabilir, geçici değişkene ata)
        }
        
        GenerateLevel();
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
            AudioManager.Instance.PlaySFX(flipSound);
            _firstSelected.ShowCard();
        }
        else
        {
            _secondSelected = flippedCard;
            AudioManager.Instance.PlaySFX(flipSound);
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

    // Sürenin azaldığını belli eden küçük bir efekt
    private IEnumerator FlashTimerRed()
    {
        timerText.color = Color.red;
        yield return new WaitForSeconds(0.3f);
        timerText.color = Color.white;
    }

    private IEnumerator ShowLevelWarning(string message)
    {
        infoText.text = message;
        infoText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f); // 3 saniye ekranda kalsın
        infoText.gameObject.SetActive(false);
    }

    public void ChangeLanguage()
    {
        LanguageManager.currentLanguage = (LanguageManager.currentLanguage == Language.TR) ? Language.EN : Language.TR;
        
        // Bayrağı değiştir
        if (languageButtonImage != null)
        {
            languageButtonImage.sprite = (LanguageManager.currentLanguage == Language.TR) ? trFlag : enFlag;
        }
    
        UpdateUI();
    }

    public void OpenSettings()
    {
        _timerActive = false; // Süreyi durdur
    }

    public void CloseSettings()
    {
        // Eğer oyun bitmediyse ve kart gösterimi yapılmıyorsa süreyi devam ettir
        if (_remainingTime > 0 && !_isProcessing) 
        {
            _timerActive = true; 
        }
    }

    private IEnumerator ComboPopupEffect(int combo)
    {
        infoText.text = "COMBO x" + combo;
        infoText.color = Color.yellow; // Kombo sarı parlasın
        infoText.gameObject.SetActive(true);
    
        // Büyüme Efekti
        float elapsed = 0f;
        float duration = 0.2f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float scale = Mathf.Lerp(1f, 1.5f, elapsed / duration);
            infoText.transform.localScale = Vector3.one * scale;
            yield return null;
        }
    
        yield return new WaitForSeconds(0.3f); // Biraz bekle
    
        // Küçülüp Kaybolma
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float scale = Mathf.Lerp(1.5f, 0f, elapsed / duration);
            infoText.transform.localScale = Vector3.one * scale;
            yield return null;
        }
    
        infoText.gameObject.SetActive(false);
        infoText.transform.localScale = Vector3.one; // Scale'i sıfırla
    }

    void CheckForShuffle()
    {
        if (_currentLevelIndex >= 30 && _moveCount > 0 && _moveCount % 10 == 0)
        {
            StartCoroutine(ShuffleRoutine());
        }
    }

    private IEnumerator ShuffleRoutine()
    {
        _isProcessing = true; // Karışırken kimse basamasın
        StartCoroutine(ShowLevelWarning(LanguageManager.GetText("shuffling")));
        yield return new WaitForSeconds(1f);
    
        // Sadece henüz eşleşmemiş kartları topla
        List<Card> remainingCards = allCards.FindAll(c => !c.IsMatched);
    
        foreach (Card card in remainingCards)
        {
            // Unity Hierarchy'deki yerini rastgele değiştirerek fiziksel yerini değiştiriyoruz
            card.transform.SetSiblingIndex(Random.Range(0, cardGridParent.childCount));
        }
    
        yield return new WaitForSeconds(0.5f);
        _isProcessing = false;
    }
    StartCoroutine(ShowCardsAtStart());
}
