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
    private SpriteRenderer[] spriteRenderers;
    [SerializeField] private PlayerScript player;
    private int yPressCount = 0;
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

        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        if (spriteRenderers.Length == 0)
        {
            Debug.LogError("Aucun SpriteRenderer trouvé!");
            return;
        }

        foreach (var renderer in spriteRenderers)
        {
            renderer.enabled = false;
        }
        isVisible = false;
    }

    void Update()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null) return;
        }

        if (isVisible)
        {
            MoveViseur();
            
            if (Input.GetKeyDown(KeyCode.Y))
            {
                yPressCount++;
                if (foudre != null)
                {
                    foudre.ShowFoudre(transform.position);
                }
                
                // Quitte le mode visée après 3 appuis sur Y
                if (yPressCount >= 3)
                {
                    HideViseur();
                }
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                HideViseur();
            }
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
        yPressCount = 0;
    }

    private void HideViseur()
    {
        isVisible = false;
        yPressCount = 0;
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
}
