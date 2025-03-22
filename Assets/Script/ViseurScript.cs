using UnityEngine;

public class ViseurScript : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    private Camera mainCamera;
    private bool isVisible = false;
    public bool IsVisible 
    { 
        get { return isVisible; } 
    }
    private SpriteRenderer[] spriteRenderers; // Tableau pour tous les SpriteRenderer
    [SerializeField] private PlayerScript player;
    private int yPressCount = 0;
    [SerializeField] private float resetDelay = 5f; // Délai de réinitialisation
    private float resetTimer = 0f;
    private bool isResetting = false;
    [SerializeField] private Stamina1Script stamina1;
    [SerializeField] private Stamina2Script stamina2;
    [SerializeField] private FoudreScript foudre;

    void Start()
    {
        Debug.Log("Start appelé");
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Pas de caméra principale trouvée!");
            return;
        }

        // Récupère tous les SpriteRenderer (parent et enfants)
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        if (spriteRenderers.Length == 0)
        {
            Debug.LogError("Aucun SpriteRenderer trouvé!");
            return;
        }

        // Cache tous les sprites
        foreach (var renderer in spriteRenderers)
        {
            renderer.enabled = false;
        }
        isVisible = false;
    }

    void Update()
    {
        // S'assure que la caméra est toujours référencée
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null) return;
        }

        if (isVisible)
        {
            isResetting = false;
            resetTimer = 0f;
            MoveViseur();
            
            // Gestion du compteur d'appuis sur Y
            if (Input.GetKeyDown(KeyCode.Y))
            {
                yPressCount++;
                if (yPressCount >= 3)
                {
                    HideViseur();
                }
                else if (foudre != null)
                {
                    foudre.ShowFoudre(transform.position);
                }
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                HideViseur();
            }
        }
        else if (!isVisible && (stamina1 != null && !stamina1.gameObject.activeSelf || 
                               stamina2 != null && !stamina2.gameObject.activeSelf))
        {
            // Compte le temps quand on n'est pas en mode viseur
            isResetting = true;
            resetTimer += Time.deltaTime;

            if (resetTimer >= resetDelay)
            {
                ResetStaminas();
            }
        }
    }

    private void ToggleViseur()
    {
        isVisible = !isVisible;
        // Active/Désactive tous les sprites
        foreach (var renderer in spriteRenderers)
        {
            renderer.enabled = isVisible;
        }

        if (isVisible)
        {
            Vector3 centerPosition = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
            centerPosition.z = 0;
            transform.position = centerPosition;
            Debug.Log($"Viseur activé à la position : {centerPosition}");
        }
    }

    public void ShowViseur()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("Pas de caméra principale trouvée!");
                return;
            }
        }

        isVisible = true;
        foreach (var renderer in spriteRenderers)
        {
            renderer.enabled = true;
        }

        Vector3 centerPosition = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
        centerPosition.z = 0;
        transform.position = centerPosition;
        yPressCount = 0; // Réinitialise le compteur au début du mode visé
    }

    private void HideViseur()
    {
        isVisible = false;
        yPressCount = 0; // Réinitialise le compteur
        foreach (var renderer in spriteRenderers)
        {
            renderer.enabled = false;
        }
        if (player != null)
        {
            player.ResetFromViseur();
        }
    }

    private void MoveViseur()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        
        Vector3 movement = new Vector3(horizontalInput, verticalInput, 0) * moveSpeed * Time.deltaTime;
        transform.position += movement;
    }

    private void ResetStaminas()
    {
        if (stamina1 != null) stamina1.Reset();
        if (stamina2 != null) stamina2.Reset();
        resetTimer = 0f;
        isResetting = false;
    }
}
