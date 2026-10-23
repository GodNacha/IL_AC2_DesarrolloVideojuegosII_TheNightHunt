using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class TrashObject : MonoBehaviour
{
    public bool collected;

    private void Start()
    {
        collected = false;
    }

    public void Collect()
    {
        if (collected) return;

        collected = true;

        TrashManager.Instance.CollectTrash(this);

        Destroy(gameObject);
    }

}
