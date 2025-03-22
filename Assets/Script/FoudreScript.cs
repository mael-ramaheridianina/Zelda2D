using UnityEngine;

public class FoudreScript : MonoBehaviour
{
    [SerializeField] private float displayDuration = 0.5f; // Durée d'affichage de la foudre
    private float displayTimer = 0f;
    private bool isDisplaying = false;

    void Start()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (isDisplaying)
        {
            displayTimer += Time.deltaTime;
            if (displayTimer >= displayDuration)
            {
                HideFoudre();
            }
        }
    }

    public void ShowFoudre(Vector3 position)
    {
        transform.position = position;
        gameObject.SetActive(true);
        isDisplaying = true;
        displayTimer = 0f;
    }

    private void HideFoudre()
    {
        gameObject.SetActive(false);
        isDisplaying = false;
    }
}
