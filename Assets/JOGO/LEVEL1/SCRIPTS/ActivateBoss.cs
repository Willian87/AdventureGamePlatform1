using UnityEngine;

public class ActivateOnTrigger : MonoBehaviour
{
    [Header("Target GameObject to Activate")]
    [SerializeField] private GameObject targetObject;

    [Header("Trigger Settings")]
    [SerializeField] private string triggeringTag = "Player"; // The tag of the object that can trigger this event

   
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            if (targetObject != null)
            {
                targetObject.gameObject.SetActive(true); // Activate the target object
                Debug.Log($"{targetObject.name} has been activated by {collision.name}.");
                this.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogWarning("Target object is not assigned in ActivateOnTrigger script.");
            }
        }
    }
    //private void OnTriggerExit(Collider other)
    //{
    //    // Optionally deactivate the target object when the trigger is exited
    //    if (other.CompareTag(triggeringTag))
    //    {
    //        if (targetObject != null)
    //        {
    //            targetObject.SetActive(false); // Deactivate the target object
    //            Debug.Log($"{targetObject.name} has been deactivated by {other.name}.");
    //        }
    //    }
    //}
}

