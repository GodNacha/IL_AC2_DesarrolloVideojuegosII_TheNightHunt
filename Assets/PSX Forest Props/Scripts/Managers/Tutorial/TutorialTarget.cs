using UnityEngine;

public class TutorialTarget : MonoBehaviour, IShootable
{
    public void OnShot()
    {
        TutorialManager.Instance.TargetDestroyed();
        Destroy(gameObject);
    }
}