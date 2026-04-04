using UnityEngine;
using UnityEngine.Advertisements;

public class AdsInitializer : MonoBehaviour, IAdsInitializationListener
{
    [SerializeField] string _androidGameId = "SENIN_ID_BURAYA";
    [SerializeField] bool _testMode = true;

    void Awake()
    {
        InitializeAds();
    }

    public void InitializeAds()
    {
        Advertisement.Initialize(_androidGameId, _testMode, this);
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads Başarıyla Başlatıldı.");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Başlatılamadı: {error} - {message}");
    }
}
