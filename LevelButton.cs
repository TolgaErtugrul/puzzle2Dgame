using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelButton : MonoBehaviour
{
    public int levelIndex;
    public TextMeshProUGUI levelText;
    public Image[] stars;
    public GameObject lockIcon;
    public Button mainButton;

    // Menü yöneticisi bu butonu oluşturduğunda bu fonksiyonu çağıracak
    public void Setup(int index, int starCount, bool isUnlocked)
    {
        levelIndex = index;
        levelText.text = (index + 1).ToString();
        
        // Kilit durumu
        lockIcon.SetActive(!isUnlocked);
        mainButton.interactable = isUnlocked;
        levelText.gameObject.SetActive(isUnlocked);

        // Yıldızları göster
        for (int i = 0; i < stars.Length; i++)
        {
            if (isUnlocked)
            {
                stars[i].gameObject.SetActive(true);
                stars[i].color = (i < starCount) ? Color.yellow : Color.gray;
            }
            else
            {
                stars[i].gameObject.SetActive(false);
            }
        }
    }

    public void OnClick()
    {
        Debug.Log("Butona basıldı, index: " + levelIndex);
        if(LevelMenuManager.Instance != null)
        {
            LevelMenuManager.Instance.StartSelectedLevel(levelIndex);
        }
        else
        {
            Debug.LogError("LevelMenuManager bulunamadı!");
        }
    }
}
