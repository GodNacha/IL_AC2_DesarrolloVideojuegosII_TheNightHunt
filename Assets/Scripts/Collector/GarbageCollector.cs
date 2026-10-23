using UnityEngine;

public class GarbageCollector : MonoBehaviour
{
    private bool isFull;
    private int capacity;

    public void OnTriggerEnter(Collider other)
    {
        TrashObject trash = other.GetComponent<TrashObject>();

        if (trash != null)
        {
            trash.Collect();
        }

        Debug.Log("Interacción item con basurero");
    }
}
