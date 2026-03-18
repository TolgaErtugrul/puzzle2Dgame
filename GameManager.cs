using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Diğer kodlardan kolayca erişmek için

    [Header("Game Settings")]
    public List<Card> allCards = new List<Card>();
    
    private Card _firstSelected;
    private Card _secondSelected;
    private bool _isProcessing = false; // İki kart kontrol edilirken dokunmayı engellemek için

    void Awake()
    {
        Instance = this;
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
}
