using UnityEngine;

public class ViseurScript : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    private Camera mainCamera;
    private bool isVisible = false;
    private SpriteRenderer[] spriteRenderers; // Tableau pour tous les SpriteRenderer

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
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Touche R pressée");
            ToggleViseur();
        }

        if (isVisible)
        {
            MoveViseur();
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

    private void MoveViseur()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        
        Vector3 movement = new Vector3(horizontalInput, verticalInput, 0) * moveSpeed * Time.deltaTime;
        transform.position += movement;
    }
}
