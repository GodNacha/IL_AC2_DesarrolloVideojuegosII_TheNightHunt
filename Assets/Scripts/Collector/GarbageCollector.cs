using UnityEngine;

public class GarbageCollector : MonoBehaviour
{
    private bool isFull;
    private int capacity;

    public void OnTriggerEnter(Collider other)
    {
        TrashObject trash = other.GetComponent<TrashObject>();

        DragRigidbody dragtrash = other.GetComponent<DragRigidbody>();

        if (trash != null && dragtrash != null)
        {
            trash.Collect();
            dragtrash.HandleInputEnd(Input.mousePosition);
        }

        Debug.Log("Interacción item con basurero");
    }
}
