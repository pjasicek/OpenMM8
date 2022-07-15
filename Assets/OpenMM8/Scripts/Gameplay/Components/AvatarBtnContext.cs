using Assets.OpenMM8.Scripts.Gameplay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class AvatarBtnContext : MonoBehaviour
{
    public NpcTalkProperties TalkProperties;

    public GameObject Holder;
    public Button AvatarButton;
    public Image Avatar;
    public Text AvatarText;

    private void Awake()
    {
        Holder = this.gameObject;
        AvatarButton = GetComponent<Button>();
        AvatarText = transform.Find("AvatarText").GetComponent<Text>();
        Avatar = transform.Find("Avatar").GetComponent<Image>();
    }
}