using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Mediation;

public class AdsInitializer : MonoBehaviour
{
    public string gameId = "SENIN_DASHBOARD_ID_BURAYA";

    async void Awake()
    {
        try
        {
            // Unity Servislerini başlat
            await UnityServices.InitializeAsync();
            Debug.Log("Unity Services Başlatıldı.");
            
            // Reklamları başlat (Mediation)
            InitializationOptions options = new InitializationOptions();
            options.SetGameId(gameId);
            // Burada ek başlatma ayarları yapılabilir
        }
        catch (System.Exception e)
        {
            Debug.LogError("Başlatma hatası: " + e.Message);
        }
    }
}
