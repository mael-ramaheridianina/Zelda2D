using UnityEngine;

public class Lanmola_BodyScript : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private float offset;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetSortingOrder(int order)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = order;
        }
    }

    public void Initialize(float offset)
    {
        this.offset = offset;
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = -(int)(offset * 10);
        }
    }
}
