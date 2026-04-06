using Unity.Services.LevelPlay; // Örnekte gördüğün kütüphane
using UnityEngine;

public class AdsManager : MonoBehaviour
{
    public static AdsManager Instance; // Kolay erişim için
    private LevelPlayRewardedAd rewardedVideoAd;

    void Awake()
    {
        Instance = this;
    }

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

    public void ShowRewardedAd()
    {
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
        Debug.Log("Reklam başarıyla izlendi!");
        GameManager.Instance.WatchAdAndContinue();
        
        // Bir sonraki kullanım için reklamı tekrar yükle
        rewardedVideoAd.LoadAd();
    }
}
