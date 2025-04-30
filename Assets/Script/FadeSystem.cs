using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeSystem : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 1.0f;
    [SerializeField] private GameObject fadePanel;
    [SerializeField] private Image fadeImage;
    
    private static FadeSystem instance;
    
    // Propriété Singleton pour accéder à l'instance depuis d'autres scripts
    public static FadeSystem Instance
    {
        get { return instance; }
    }
    
    private void Awake()
    {
        // Configuration du singleton
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        // S'assurer que le panel est désactivé au départ
        if (fadePanel != null)
        {
            fadePanel.SetActive(false);
        }
    }
    
    // Fondu en noir puis fondu transparent (pour entrer dans une maison)
    public IEnumerator FadeInOut(System.Action onFadeInComplete = null)
    {
        // Activer le panel et s'assurer qu'il est transparent
        fadePanel.SetActive(true);
        fadeImage.color = new Color(0, 0, 0, 0);
        
        // Fondu en noir
        float elapsedTime = 0;
        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // S'assurer que le fondu est complètement noir
        fadeImage.color = new Color(0, 0, 0, 1);
        
        // Exécuter l'action de callback (téléportation, changement de scène, etc.)
        onFadeInComplete?.Invoke();
        
        // Attendre un peu avant de faire le fondu transparent
        yield return new WaitForSeconds(0.5f);
        
        // Fondu transparent
        elapsedTime = 0;
        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // S'assurer que le fondu est complètement transparent
        fadeImage.color = new Color(0, 0, 0, 0);
        
        // Désactiver le panel
        fadePanel.SetActive(false);
    }
}