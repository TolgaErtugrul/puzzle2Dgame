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
    public void OnCardFlipped(Card flippedCard)
    {
        if (_isProcessing) return;
        if (flippedCard == _firstSelected) return;
    
        if (_firstSelected == null)
        {
            _firstSelected = flippedCard;
            _firstSelected.ShowCard();
        }
        else
        {
            _secondSelected = flippedCard;
            _secondSelected.ShowCard();
            
            _moveCount++; // Her iki kart açıldığında hamleyi artır
            UpdateUI();
            
            StartCoroutine(CheckMatchRoutine());
        }
    }
    
    void UpdateUI()
    {
        if (moveText != null)
            moveText.text = "Hamle: " + _moveCount;
    }

    private IEnumerator CheckMatchRoutine()
    {
        _isProcessing = true;

        // Kartların dönme animasyonu için bekle
        yield return new WaitForSeconds(0.6f);

        if (_firstSelected.GetID() == _secondSelected.GetID())
        {
            // ✅ EŞLEŞME OLDU
            _firstSelected.SetMatched();
            _secondSelected.SetMatched();
            _matchedPairs++;

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
        Debug.Log("Oyun Kazandı!");
        if (winPanelGroup != null)
        {
            StartCoroutine(FadeInPanel(winPanelGroup));
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
        if (currentLevel == null) return; // Hata almamak için kontrol
        
        // Eski kartları temizle
        foreach (Transform child in cardGridParent) { Destroy(child.gameObject); }
        allCards.Clear();
    
        int totalCards = currentLevel.rowCount * currentLevel.columnCount;
        totalPairs = totalCards / 2;
    
        // Önce griddeki eski kartları temizle (varsa)
        foreach (Transform child in cardGridParent) { Destroy(child.gameObject); }
        allCards.Clear();
    
        int totalCards = currentLevel.rowCount * currentLevel.columnCount;
        totalPairs = totalCards / 2;
    
        List<int> idList = new List<int>();
        for (int i = 0; i < totalPairs; i++)
        {
            idList.Add(i); // Çiftin birincisi
            idList.Add(i); // Çiftin ikincisi
        }

        // Grid ayarlarını dinamik olarak güncelle
        GridLayoutGroup gridLayout = cardGridParent.GetComponent<GridLayoutGroup>();
        if (gridLayout != null)
        {
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = currentLevel.columnCount;
        }
        
        // ID listesini karıştır
        for (int i = 0; i < idList.Count; i++)
        {
            int temp = idList[i];
            int randomIndex = Random.Range(i, idList.Count);
            idList[i] = idList[randomIndex];
            idList[randomIndex] = temp;
        }
    
        // Kartları oluştur
        for (int i = 0; i < totalCards; i++)
        {
            GameObject newCard = Instantiate(cardPrefab, cardGridParent);
            Card cardScript = newCard.GetComponent<Card>();
            
            int cardID = idList[i];
            cardScript.SetupCard(cardID, cardIcons[cardID]);
            allCards.Add(cardScript);
        }

        // Kartları oluşturduktan sonra:
        StartCoroutine(StartGameSequence());
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
}
