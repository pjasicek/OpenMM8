using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Assets.OpenMM8.Scripts.Gameplay;

public class NpcTalkUI
{
    public Canvas NpcTalkCanvas;
    public GameObject NpcTalkObj;
    public Image NpcTalkBackgroundImg;
    public Text NpcResponseText;
    public TalkAvatar TalkAvatar;
    public Text LocationNameText;
    public RectTransform TopicButtonHolder;
    public List<GameObject> TopicButtonList = new List<GameObject>();

    public RectTransform AvatarBtnHolder;
    public List<AvatarBtnContext> AvatarBtnList = new List<AvatarBtnContext>();

    public const float DefaultResponseY = -292.0f;
}
