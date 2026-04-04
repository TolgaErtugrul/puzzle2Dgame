using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManager : MonoBehaviour, IAdsLoadListener, IAdsShowListener
{
    public string adUnitId = "Rewarded_Android"; // Dashboard'da "Placements" kısmında yazar

    public void LoadAd()
    {
        Debug.Log("Reklam yükleniyor...");
        Advertisement.Load(adUnitId, this);
    }

    public void ShowAd()
    {
        Debug.Log("Reklam gösteriliyor...");
        Advertisement.Show(adUnitId, this);
    }

    // Reklam bittiğinde çalışacak olan yer burası!
    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        if (showCompletionState == UnityAdsShowCompletionState.COMPLETED)
        {
            // OYUNCU REKLAMI BİTİRDİ - ÖDÜLÜ VER!
            GameManager.Instance.WatchAdAndContinue();
        }
    }

    // Diğer zorunlu interface fonksiyonları (boş kalabilirler)
    public void OnAdsAdLoaded(string placementId) { }
    public void OnAdsFailedToLoad(string placementId, AdsLoadError error, string message) { }
    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message) { }
    public void OnUnityAdsShowStart(string placementId) { }
    public void OnUnityAdsShowClick(string placementId) { }
}
