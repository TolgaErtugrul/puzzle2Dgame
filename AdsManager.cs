using UnityEngine;
using Unity.Services.Mediation;

public class AdsManager : MonoBehaviour
{
    public string adUnitId = "Rewarded_Android"; 
    IRewardedAd rewardedAd;

    async void Start()
    {
        // Reklam objesini oluştur
        rewardedAd = MediationService.Instance.CreateRewardedAd(adUnitId);
        
        // Etkinlikleri dinle
        rewardedAd.OnUserRewarded += UserRewarded;
        
        // Reklamı yükle
        await rewardedAd.LoadAsync();
    }

    public async void ShowAd()
    {
        if (rewardedAd.AdState == AdState.Loaded)
        {
            await rewardedAd.ShowAsync();
        }
        else
        {
            Debug.Log("Reklam henüz hazır değil, tekrar yükleniyor...");
            await rewardedAd.LoadAsync();
        }
    }

    void UserRewarded(object sender, RewardEventArgs e)
    {
        Debug.Log("Ödül kazanıldı!");
        GameManager.Instance.WatchAdAndContinue();
    }
}
