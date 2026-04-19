using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections; // Coroutine (Animasyon) için gerekli

public class WinScreenManager : MonoBehaviour
{
    public static WinScreenManager Instance { get; private set; }

    [Header("UI Elemanları")]
    public GameObject winPanel;
    public TextMeshProUGUI scoreText;
    public Image[] starImages;
    public GameObject gameplayUI;

    [Header("Yıldız Renkleri")]
    public Color activeStarColor = Color.white;
    public Color inactiveStarColor = new Color(0.3f, 0.3f, 0.3f, 1f);

    [Header("Animasyon Ayarları")]
    public float animationDuration = 0.5f; // Pop-up'ın ekrana gelme süresi
    [Tooltip("Panelin büyüme şekli (Baştan sona giden ve hafifçe 1'i geçen bir eğri çizin)")]
    public AnimationCurve popUpCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (winPanel != null) winPanel.SetActive(false);
        if (gameplayUI != null) gameplayUI.SetActive(true);
    }

    public void ShowWinScreen(int finalScore, int starsEarned)
    {
        if (gameplayUI != null) gameplayUI.SetActive(false);

        // Skoru ve yıldızları ayarla
        scoreText.text = "SCORE: " + finalScore.ToString();
        for (int i = 0; i < starImages.Length; i++)
        {
            if (i < starsEarned) starImages[i].color = activeStarColor;
            else starImages[i].color = inactiveStarColor;
        }

        // Oyunu (zamanı) durdur
        Time.timeScale = 0f;

        // ANİMASYONU BAŞLAT
        StartCoroutine(AnimatePopup());
    }

    // --- ANİMASYON FONKSİYONU ---
    private IEnumerator AnimatePopup()
    {
        // 1. Paneli aç ama boyutunu (Scale) 0 yap (Görünmez olsun)
        winPanel.transform.localScale = Vector3.zero;
        winPanel.SetActive(true);

        float elapsedTime = 0f;

        // Belirlediğimiz süre boyunca döngüyü çalıştır
        while (elapsedTime < animationDuration)
        {
            // Zaman durduğu için "unscaledDeltaTime" (gerçek zaman) kullanıyoruz
            elapsedTime += Time.unscaledDeltaTime; 

            // Sürenin yüzde kaçındayız? (0 ile 1 arası bir değer)
            float percent = elapsedTime / animationDuration;

            // Bu yüzdeyi Inspector'daki sihirli eğrimize (Curve) veriyoruz
            float curveValue = popUpCurve.Evaluate(percent);

            // Panelin boyutunu bu değere göre ayarlıyoruz
            winPanel.transform.localScale = new Vector3(curveValue, curveValue, 1f);

            // Bir sonraki kareyi (frame) bekle
            yield return null; 
        }

        // Animasyon bitince boyutun tam 1 (orijinal boyut) olduğundan emin ol
        winPanel.transform.localScale = Vector3.one;
    }

    public void OnRetryClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnMainMenuClicked()
    {
        Time.timeScale = 1f;
        VectorFlow.Managers.UIManager.goToLevelSelect = true; // Win ekranından dönerken de direkt Level Select'e git
        SceneManager.LoadScene("MenuScene");
    }
}