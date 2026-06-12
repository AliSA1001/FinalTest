using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartsHealthVisual : MonoBehaviour
{
    [SerializeField] private Sprite heart0Sprite;
    [SerializeField] private Sprite heart1Sprite;
    [SerializeField] private Sprite heart2Sprite;
    [SerializeField] private Sprite heart3Sprite;
    [SerializeField] private Sprite heart4Sprite;

    private List<HeartImage> heartImagesList;

    private void Awake()
    {
        heartImagesList = new List<HeartImage>();
    }

    private void Start()
    {
        CreateHeartImage(new Vector2(0, 0)).SetHeartFragments(4);
        CreateHeartImage(new Vector2(30, 0)).SetHeartFragments(1);
        CreateHeartImage(new Vector2(60, 0)).SetHeartFragments(0);
    }

    private HeartImage CreateHeartImage(Vector2 anchoredPosition)
    {
        GameObject heartGameObject = new GameObject("Heart", typeof(Image));

        heartGameObject.transform.SetParent(transform, false);

        RectTransform rectTransform = heartGameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(32, 32);

        Image heartImageUI = heartGameObject.GetComponent<Image>();

        HeartImage heartImage = new HeartImage(this, heartImageUI);

        heartImagesList.Add(heartImage);

        return heartImage;
    }

    public class HeartImage
    {
        private Image heartImage;
        private HeartsHealthVisual heartsHealthVisual;

        public HeartImage(HeartsHealthVisual heartsHealthVisual, Image heartImage)
        {
            this.heartsHealthVisual = heartsHealthVisual;
            this.heartImage = heartImage;
        }

        public void SetHeartFragments(int fragments)
        {
            switch (fragments)
            {
                case 0:
                    heartImage.sprite = heartsHealthVisual.heart0Sprite;
                    break;

                case 1:
                    heartImage.sprite = heartsHealthVisual.heart1Sprite;
                    break;

                case 2:
                    heartImage.sprite = heartsHealthVisual.heart2Sprite;
                    break;

                case 3:
                    heartImage.sprite = heartsHealthVisual.heart3Sprite;
                    break;

                case 4:
                    heartImage.sprite = heartsHealthVisual.heart4Sprite;
                    break;
            }
        }
    }
}