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
    private HeartsHealthSystem heartsHealthSystem;

    private void Awake()
    {
        heartImagesList = new List<HeartImage>();
    }

    private void Start()
    {
        HeartsHealthSystem heartsHealthSystem = new HeartsHealthSystem(4);

        Debug.Log("Heart 1 = " + heartsHealthSystem.GetHeartList()[0].GetFragmentAmount());
        Debug.Log("Heart 2 = " + heartsHealthSystem.GetHeartList()[1].GetFragmentAmount());
        Debug.Log("Heart 3 = " + heartsHealthSystem.GetHeartList()[2].GetFragmentAmount());
        Debug.Log("Heart 4 = " + heartsHealthSystem.GetHeartList()[3].GetFragmentAmount());

        SetHeartsHealthSystem(heartsHealthSystem);
    }

    public void SetHeartsHealthSystem(HeartsHealthSystem heartsHealthSystem)
    {
        this.heartsHealthSystem = heartsHealthSystem;

        List<HeartsHealthSystem.Heart> heartList = heartsHealthSystem.GetHeartList();

        Vector2 heartAnchoredPosition = new Vector2(0, 0);

        for (int i = 0; i < heartList.Count; i++)
        {
            HeartsHealthSystem.Heart heart = heartList[i];

            CreateHeartImage(heartAnchoredPosition)
                .SetHeartFragments(heart.GetFragmentAmount());

            heartAnchoredPosition += new Vector2(50, 0);
        }

        heartsHealthSystem.OnDamaged += HeartsHealthSystem_OnDamaged;
    }

    private void HeartsHealthSystem_OnDamaged(object sender, System.EventArgs e)
    {
        List<HeartsHealthSystem.Heart> heartList = heartsHealthSystem.GetHeartList();

        for (int i = 0; i < heartImagesList.Count; i++)
        {
            HeartImage heartImage = heartImagesList[i];
            HeartsHealthSystem.Heart heart = heartList[i];

            heartImage.SetHeartFragments(heart.GetFragmentAmount());
        }
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

    public void Damage1()
    {
        heartsHealthSystem.Damage(1);
    }

    public void Damage4()
    {
        heartsHealthSystem.Damage(4);
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