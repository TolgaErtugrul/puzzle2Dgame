using UnityEngine;

public class AdsManager : MonoBehaviour
{
    void Start()
    {
        // Ödüllü reklam etkinliklerini dinle
        IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoAdRewardedEvent;
    }

    public void ShowAd()
    {
        if (IronSource.Agent.isRewardedVideoAvailable())
        {
            IronSource.Agent.showRewardedVideo();
        }
        else
        {
            Debug.Log("Reklam henüz hazır değil!");
        }
    }

    // Reklam başarıyla izlendiğinde çalışacak fonksiyon
    void RewardedVideoAdRewardedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
    {
        Debug.Log("Reklam tamamlandı! Ödül veriliyor...");
        GameManager.Instance.WatchAdAndContinue();
    }
}
