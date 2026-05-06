using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelButton : MonoBehaviour
{
    public int levelIndex;
    public TextMeshProUGUI levelNumberText;
    public Image[] starIcons;
    public GameObject lockIcon;
    public Button button;

    public void Setup(int index, int stars, bool isUnlocked)
    {
        levelIndex = index;
        levelNumberText.text = (index + 1).ToString();
        button.interactable = isUnlocked;
        lockIcon.SetActive(!isUnlocked);

        // Yıldızları göster
        for (int i = 0; i < starIcons.Length; i++)
        {
            starIcons[i].color = (i < stars) ? Color.yellow : Color.gray;
        }
    }

    public void OnClick()
    {
        // GameManager'a bu seviyeyi yüklemesini söyleyeceğiz
        // SceneManager.LoadScene("GameScene"); gibi...
    }
}
