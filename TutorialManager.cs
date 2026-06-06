public class TutorialManager : MonoBehaviour
{
    public GameObject handPointer; // El ikonu
    public RectTransform level1Button; // Seviye 1 butonu

    void Start()
    {
        // Daha önce hiç oynanmadıysa (İlk açılış)
        if (PlayerPrefs.GetInt("TutorialCompleted", 0) == 0)
        {
            ShowMenuTutorial();
        }
        else
        {
            handPointer.SetActive(false);
        }
    }

    void ShowMenuTutorial()
    {
        handPointer.SetActive(true);
        // El ikonunu Seviye 1 butonunun üzerine konumlandır
        handPointer.transform.position = level1Button.position;
        // İstersen burada basit bir 'zıplama' animasyonu da ekleyebilirsin
    }
}
