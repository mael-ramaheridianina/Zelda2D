using UnityEngine;

public class Stamina1Script : MonoBehaviour
{
    [SerializeField] private ViseurScript viseur; // Référence au script du viseur
    
    void Start()
    {
        if (viseur == null)
        {
            Debug.LogError("Viseur non assigné sur Stamina1!");
        }
    }

    void Update()
    {
        // Vérifie si on est en mode viseur et que Y est pressé
        if (viseur != null && viseur.IsVisible && Input.GetKeyDown(KeyCode.Y))
        {
            gameObject.SetActive(false);
        }
    }

    // Méthode pour réactiver le GameObject
    public void Reset()
    {
        gameObject.SetActive(true);
    }
}
