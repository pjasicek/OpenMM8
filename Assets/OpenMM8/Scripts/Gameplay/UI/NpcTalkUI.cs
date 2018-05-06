using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class NpcTalkUI
{
    public Canvas NpcTalkCanvas;
    public Image NpcResponseBackground;
    public Text NpcResponseText;
    public Image NpcAvatar;
    public Text LocationNameText;
    public Text NpcNameText;
    public RectTransform TopicButtonHolder;
    public List<GameObject> TopicButtonList = new List<GameObject>();

    public const float DefaultResponseY = -292.0f;
}
