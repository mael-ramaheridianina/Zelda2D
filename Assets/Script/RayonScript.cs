using UnityEngine;

public class RayonScript : MonoBehaviour
{
    private void Start()
    {
        // Trouve l'épée et s'abonne à son événement
        EpéeScript epee = Object.FindFirstObjectByType<EpéeScript>();
        if (epee != null)
        {
            epee.OnEpeeCollectee += DisableRayon;
        }
    }

    private void DisableRayon()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        // Se désabonne de l'événement quand le rayon est détruit
        EpéeScript epee = Object.FindFirstObjectByType<EpéeScript>();
        if (epee != null)
        {
            epee.OnEpeeCollectee -= DisableRayon;
        }
    }
}
