using UnityEngine;
using System.Collections;

public class DoorInteractionScript : MonoBehaviour
{
    [SerializeField] private Transform insideSpawnPoint; // Point d'apparition à l'intérieur de la maison
    [SerializeField] private GameObject doorVisualObject; // Objet visuel de la porte (optionnel)
    [SerializeField] private bool isInside = false; // Sommes-nous à l'intérieur ou l'extérieur?
    
    private bool playerInRange = false;
    private PlayerScript player;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            player = other.GetComponent<PlayerScript>();
            
            Debug.Log("Joueur à proximité de la porte. Appuyez sur A pour entrer.");
            
            // Optionnel: Montrer une indication visuelle que l'interaction est possible
            // ShowInteractionPrompt();
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            player = null;
            
            // Optionnel: Cacher l'indication visuelle
            // HideInteractionPrompt();
        }
    }
    
    private void Update()
    {
        if (playerInRange && player != null && Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(EnterHouse());
        }
    }
    
    private IEnumerator EnterHouse()
    {
        // Désactiver les contrôles du joueur pendant la transition
        player.SetCanMove(false);
        
        // Trouver le système de fondu
        FadeSystem fadeSystem = FadeSystem.Instance;
        if (fadeSystem != null)
        {
            // Lancer le fondu avec un callback pour téléporter le joueur
            yield return StartCoroutine(fadeSystem.FadeInOut(() => 
            {
                if (insideSpawnPoint != null)
                {
                    // Téléporter le joueur à l'intérieur/extérieur
                    player.transform.position = insideSpawnPoint.position;
                    isInside = !isInside;
                    
                    // Optionnel : changer l'apparence de la porte
                    if (doorVisualObject != null)
                    {
                        doorVisualObject.SetActive(!isInside);
                    }
                }
            }));
        }
        
        // Réactiver les contrôles du joueur
        player.SetCanMove(true);
    }
}