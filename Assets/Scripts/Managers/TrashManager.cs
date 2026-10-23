using NUnit.Framework;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TrashManager : MonoBehaviour
{
    public static TrashManager Instance { get; private set; }

    [SerializeField] private Slider progressSlider;

    [SerializeField] private int totalTrash;
    [SerializeField] private int collectedTrash;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {

        totalTrash = FindObjectsByType<TrashObject>(FindObjectsSortMode.None).Length;

        UpdateProgress();
    }

    public void CollectTrash(TrashObject trash)
    {
        collectedTrash++;

        UpdateProgress();

        Debug.Log(
            $"Basura recogida: {collectedTrash}/{totalTrash}"
        );
    }

    private void UpdateProgress()
    {
        if (progressSlider == null) return;

        progressSlider.value = totalTrash > 0
            ? (float)collectedTrash / totalTrash
            : 1f;
    }
}
