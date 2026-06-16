using Unity.Services.LevelPlay; // Örnekte gördüğün kütüphane
using UnityEngine;

public class AdsManager : MonoBehaviour
{
    public static AdsManager Instance; // Kolay erişim için
    private LevelPlayRewardedAd rewardedVideoAd;

    private string currentRewardType = "";

    void Awake() { Instance = this; }

    void Start()
    {
        // SDK Başlatma (Örnekteki gibi)
        LevelPlay.OnInitSuccess += OnSdkInitSuccess;
        LevelPlay.Init(AdConfig.AppKey);
    }

    void OnSdkInitSuccess(LevelPlayConfiguration config)
    {
        Debug.Log("Ads SDK Hazır!");
        SetupRewardedAd();
    }

    void SetupRewardedAd()
    {
        // Reklam objesini oluştur
        rewardedVideoAd = new LevelPlayRewardedAd(AdConfig.RewardedVideoAdUnitId);

        // Ödül olayını dinle (En önemli kısım burası)
        rewardedVideoAd.OnAdRewarded += OnUserRewarded;
        
        // Reklamı yükle
        rewardedVideoAd.LoadAd();
    }

    public void ShowRewardedAd(string rewardType)
    {
        currentRewardType = rewardType;
        
        if (rewardedVideoAd != null && rewardedVideoAd.IsAdReady())
        {
            rewardedVideoAd.ShowAd();
        }
        else
        {
            Debug.Log("Reklam henüz hazır değil, tekrar yükleniyor...");
            rewardedVideoAd.LoadAd();
        }
    }

    // Oyuncu reklamı bitirince çalışacak fonksiyon
    void OnUserRewarded(LevelPlayAdInfo adInfo, LevelPlayReward reward)
    {
        if (currentRewardType == "Continue")
        {
            GameManager.Instance.WatchAdAndContinue();
        }
        else if (currentRewardType == "MarketStars")
        {
            // Market scriptindeki AwardStars'ı tetikle
            // MarketUIHandler sahnede bir tane olduğu için FindObjectOfType kullanabiliriz
            FindObjectOfType<MarketUIHandler>()?.AwardStars();
        }
        
        // Bir sonraki kullanım için reklamı tekrar yükle
        rewardedVideoAd.LoadAd();
    }
}
