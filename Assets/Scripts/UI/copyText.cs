using UnityEngine;
using TMPro; // TMP kullanmak için bu kütüphaneyi eklemelisin

public class CopyText : MonoBehaviour
{
    private TextMeshProUGUI myText;
    private TextMeshProUGUI parentText;

    void Awake()
    {
        // Kendi üzerindeki TMP bileşenini al
        myText = GetComponent<TextMeshProUGUI>();

        // Parent objesindeki TMP bileşenini al
        if (transform.parent != null)
        {
            parentText = transform.parent.GetComponent<TextMeshProUGUI>();
        }
    }

    void Update()
    {
        // Eğer her iki bileşen de bulunduysa metni kopyala
        if (myText != null && parentText != null)
        {
            myText.text = parentText.text;
        }
    }
}