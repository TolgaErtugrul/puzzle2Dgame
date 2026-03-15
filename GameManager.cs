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

        // Burada kartların ID'lerini karşılaştıracağız...
        // [Gelecek adım: Eşleşme mantığı]

        yield return new WaitForSeconds(1f);
        _isProcessing = false;
    }
}
