using UnityEngine;

public class FloatingUI : MonoBehaviour
{
    [SerializeField] private float speed = 5f;    // Zıplama hızı
    [SerializeField] private float height = 20f;  // Zıplama mesafesi
    private Vector3 startPos;

    void OnEnable()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        // Sinüs dalgası kullanarak yukarı aşağı hareket oluşturuyoruz
        float newY = Mathf.Sin(Time.time * speed) * height;
        transform.localPosition = new Vector3(startPos.x, startPos.y + newY, startPos.z);
    }
}
