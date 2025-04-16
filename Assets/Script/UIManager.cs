using UnityEngine;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI damageInfoText;
    [SerializeField] private Color textColor = Color.white;
    
    private static UIManager instance;
    private EnemyScript currentEnemy;
    private Coroutine healthDisplayCoroutine;
    
    public static UIManager Instance
    {
        get { return instance; }
    }
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        if (damageInfoText != null)
        {
            damageInfoText.color = textColor;
            damageInfoText.gameObject.SetActive(false);
        }
    }
    
    public void ShowEnemyHealth(string enemyName, float currentHealth, float maxHealth, EnemyScript enemy)
    {
        if (damageInfoText != null)
        {
            if (healthDisplayCoroutine != null)
            {
                StopCoroutine(healthDisplayCoroutine);
            }
            
            currentEnemy = enemy;
            healthDisplayCoroutine = StartCoroutine(TrackEnemyHealthCoroutine(enemyName, maxHealth));
        }
    }

    public void ShowEnemyHealth(string enemyName, float currentHealth, float maxHealth)
    {
        // Code pour afficher la santé
    }
    
    private IEnumerator TrackEnemyHealthCoroutine(string enemyName, float maxHealth)
    {
        // Activer le texte
        damageInfoText.gameObject.SetActive(true);
        
        // Continuer de mettre à jour tant que l'ennemi est vivant
        while (currentEnemy != null && currentEnemy.GetCurrentHealth() > 0)
        {
            // Mettre à jour l'affichage avec la santé actuelle
            float currentHealth = currentEnemy.GetCurrentHealth();
            damageInfoText.text = $"{enemyName} : {Mathf.Ceil(currentHealth)}/{maxHealth} PV";
            
            yield return new WaitForSeconds(0.1f); // Mise à jour fréquente
        }
        
        // Afficher un message de mort quand l'ennemi est vaincu
        if (currentEnemy == null || currentEnemy.GetCurrentHealth() <= 0)
        {
            damageInfoText.text = $"{enemyName} : Vaincu!";
            yield return new WaitForSeconds(1.0f);
        }
        
        // Masquer le texte
        damageInfoText.gameObject.SetActive(false);
        currentEnemy = null;
    }
}
