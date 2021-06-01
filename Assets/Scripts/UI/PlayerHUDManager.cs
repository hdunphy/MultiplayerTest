using MLAPI;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHUDManager : MonoBehaviour
{
    [SerializeField] private RectTransform LifeImagePrefab;
    [SerializeField] private RectTransform LivesBackground;
    [SerializeField] private RectTransform HealthUI;
    [SerializeField] private int TotalLives;
    [SerializeField] private float ImageOffset;

    private int currentLives, totalLives;
    private List<RectTransform> lifeImages;

    private void Start()
    {
        SetTotalLives(TotalLives);
        //Debug.Log(OwnerClientId);
    }

    private void SetTotalLives(int _totalLives)
    {
        currentLives = totalLives = _totalLives;
        Rect livesRect = HealthUI.rect;
        livesRect.width = totalLives * (LifeImagePrefab.rect.width + (2 * ImageOffset)) + ImageOffset;

        lifeImages = new List<RectTransform>();

        for (int i = 0; i < totalLives; i++)
        {
            var img = Instantiate(LifeImagePrefab, LivesBackground);
            img.anchoredPosition = new Vector3((LifeImagePrefab.rect.width + ImageOffset) * i + ImageOffset, 0, 0);
            lifeImages.Add(img);
        }
    }

    public void LocalPlayerDied(ulong fromClientId)
    {
        if(NetworkManager.Singleton.LocalClientId == fromClientId)
        {
            LocalPlayerDied();
        }
    }
    public void LocalPlayerDied()
    {
        Debug.Log("Player died");
        SetLifeImagesActive(--currentLives);
    }

    public void SetLifeImagesActive(int count)
    {
        for (int i = 0; i < lifeImages.Count; i++)
        {

            if (lifeImages[i].gameObject.activeSelf && count <= i)
            {
                var image = lifeImages[i];
                LeanTween.alpha(image, 0, 1f)
                    .setOnComplete(() => image.gameObject.SetActive(false));
            }
        }
    }
}
