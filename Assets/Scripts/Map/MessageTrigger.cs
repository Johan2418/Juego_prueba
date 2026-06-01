using UnityEngine;

public class MessageTrigger : MonoBehaviour
{
    [SerializeField] private string message = "Zona descubierta";
    [SerializeField] private bool once = true;
    private bool activated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activated && once) return;
        
        if (other.CompareTag("Player"))
        {
            var interactor = other.GetComponentInChildren<PlayerInteractor>();
            if (interactor != null)
            {
                interactor.ShowNotification(message, 3f);
                activated = true;
            }
        }
    }
}
