using UnityEngine;
using UnityEngine.UI;

public class HeartManager : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxAuthorizedHearts = 3;  // Nombre de coeurs autorisés au départ
    private const int ABSOLUTE_MAX_HEARTS = 20;  // Maximum absolu de coeurs (20 points de vie)
    public int maxHealth { get { return maxAuthorizedHearts * 2; } }  // Santé maximale actuelle
    public int currentHealth;    // Santé actuelle

    [Header("UI Settings")]
    public Image[] heartImages;  // Images des coeurs dans l'UI
    public Sprite fullHeart;
    public Sprite halfHeart;
    public Sprite emptyHeart;

    void Start()
    {
        // Initialise la santé au maximum autorisé
        currentHealth = maxHealth;
        UpdateHeartDisplay();
    }

    private void UpdateHeartDisplay()
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            // Active uniquement les coeurs autorisés
            if (i < maxAuthorizedHearts)
            {
                heartImages[i].enabled = true;
                
                // Calcul des points de vie pour ce coeur
                int heartHealthValue = (i + 1) * 2;

                if (currentHealth >= heartHealthValue)
                {
                    heartImages[i].sprite = fullHeart;
                }
                else if (currentHealth == heartHealthValue - 1)
                {
                    heartImages[i].sprite = halfHeart;
                }
                else
                {
                    heartImages[i].sprite = emptyHeart;
                }
            }
            else
            {
                heartImages[i].enabled = false;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Max(0, currentHealth - damage);
        UpdateHeartDisplay();
    }

    public void Heal(int healAmount)
    {
        int newHealth = Mathf.Min(maxHealth, currentHealth + healAmount);
        if (newHealth != currentHealth)
        {
            currentHealth = newHealth;
            UpdateHeartDisplay();
            Debug.Log($"Soigné de {healAmount}. Nouvelle santé : {currentHealth}/{maxHealth}");
        }
    }

    // Méthode pour augmenter le nombre de coeurs autorisés
    public bool IncreaseMaxHearts()
    {
        if (maxAuthorizedHearts >= ABSOLUTE_MAX_HEARTS)
        {
            Debug.Log("Nombre maximum de coeurs déjà atteint!");
            return false;
        }

        maxAuthorizedHearts++;
        currentHealth = maxHealth; // Remplit tous les cœurs
        UpdateHeartDisplay();
        Debug.Log($"Nouveau maximum de coeurs : {maxAuthorizedHearts}");
        return true;
    }
}
