using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoseScreenManager : MonoBehaviour
{
    public static LoseScreenManager Instance { get; private set; }

    [Header("UI Elemanları")]
    public GameObject losePanel; // Yeni yaptığımız panel
    public TextMeshProUGUI scoreText; 
    public GameObject gameplayUI; // Oyun bitince kapanacak olan oyun içi ekranı

    [Header("Animasyon Ayarları")]
    public float animationDuration = 0.5f;
    public AnimationCurve popUpCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Başlangıçta paneli gizle
        if (losePanel != null) losePanel.SetActive(false);
    }

    public void ShowLoseScreen(int finalScore)
    {
        // 1. Oyun içi UI'ı gizle
        if (gameplayUI != null) gameplayUI.SetActive(false);

        // 2. Skoru yazdır
        scoreText.text = "Score: " + finalScore.ToString();

        // 3. Zamanı durdur ve animasyonu başlat
        Time.timeScale = 0f;
        StartCoroutine(AnimatePopup());
    }

    private IEnumerator AnimatePopup()
    {
        losePanel.transform.localScale = Vector3.zero;
        losePanel.SetActive(true);

        float elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float percent = elapsedTime / animationDuration;
            float curveValue = popUpCurve.Evaluate(percent);
            losePanel.transform.localScale = new Vector3(curveValue, curveValue, 1f);
            yield return null;
        }
        losePanel.transform.localScale = Vector3.one;
    }

    // --- BUTON FONKSİYONLARI ---
    public void OnRetryClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnMainMenuClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuScene");
    }
}