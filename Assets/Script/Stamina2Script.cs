using UnityEngine;

public class Stamina2Script : MonoBehaviour
{
    [SerializeField] private ViseurScript viseur;
    [SerializeField] private Stamina1Script stamina1;
    private int yPressCount = 0;

    void Start()
    {
        if (viseur == null)
        {
            Debug.LogError("Viseur non assigné sur Stamina2!");
        }
        if (stamina1 == null)
        {
            Debug.LogError("Stamina1 non assigné sur Stamina2!");
        }
    }

    void Update()
    {
        if (viseur != null && viseur.IsVisible && Input.GetKeyDown(KeyCode.Y))
        {
            yPressCount++;
            if (yPressCount == 2)
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void Reset()
    {
        yPressCount = 0;
        gameObject.SetActive(true);
    }
}
