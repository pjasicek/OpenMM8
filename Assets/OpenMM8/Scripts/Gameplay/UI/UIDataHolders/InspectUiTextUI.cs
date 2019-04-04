using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InspectUiTextUI
{
    public const float DEFAULT_WIDTH = 385;
    public const float DEFAULT_HEIGHT = 200;
    public const float CURSOR_SPACE = 15;
    public const float DEFAULT_Y = 440;
    public readonly Vector2 DEFAULT_POSITION = new Vector2(-192.5f, 200.0f);

    public const float TOP_SPACE_PX = 15;
    public const float BOTTOM_SPACE_PX = 15;

    public GameObject Holder;

    public RectTransform BackgroundTransfrom;
    public Text NameText;
    public Text InfoText;
    public Image LeftEdge;
    public Image RightEdge;
}
