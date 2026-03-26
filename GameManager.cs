using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro; 

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
    
    private Card _firstSelected;
    private Card _secondSelected;
    private bool _isProcessing = false;
    private int _matchedPairs = 0;
    private bool _isTimerRunning = false;

    void Awake()
    {
        Instance = this;
    }

    // Kart tıklandığında Card.cs tarafından çağrılır
    public void OnCardFlipped(Card flippedCard)
    {
        if (_isProcessing) return;

        // Aynı karta iki kez tıklanmasını engelle
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
            StartCoroutine(CheckMatchRoutine());
        }
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
    }
}
