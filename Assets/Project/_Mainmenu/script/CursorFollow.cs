using UnityEngine;
using UnityEngine.UI;

public class CursorFollowUI : MonoBehaviour
{
    private RectTransform rect;
    private Image image;

    [Header("Cursor Sprites")]
    public Sprite happyCursor;
    public Sprite pointCursor;
    public Sprite sadCursor;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        image = GetComponent<Image>();
    }

    void Start()
    {
        Cursor.visible = false;
        SetHappy();
    }

    void Update()
    {
        rect.position = Input.mousePosition;
    }

    public void SetHappy()
    {
        image.sprite = happyCursor;
    }

    public void SetPoint()
    {
        image.sprite = pointCursor;
    }

    public void SetSad()
    {
        image.sprite = sadCursor;
    }

    void OnApplicationQuit()
    {
        Cursor.visible = true;
    }
}