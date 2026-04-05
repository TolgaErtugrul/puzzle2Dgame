using UnityEngine;
// LevelPlay (ironSource) kütüphanesini kullanıyoruz
using com.unity3d.mediation; 

public class AdsInitializer : MonoBehaviour
{
    [SerializeField] string _androidGameId = "SENIN_ID_BURAYA";

    void Awake()
    {
        // LevelPlay başlatma komutu
        IronSource.Agent.init(_androidGameId);
        Debug.Log("LevelPlay Başlatılıyor...");
    }

    void OnApplicationPause(bool isPaused)
    {
        IronSource.Agent.onApplicationPause(isPaused);
    }
}
