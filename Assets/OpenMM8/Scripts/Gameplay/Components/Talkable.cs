using UnityEngine;
using System.Collections;
using UnityEngine.Video;
using Assets.OpenMM8.Scripts.Gameplay;
using UnityEngine.Experimental.UIElements;

public class Talkable : Interactable
{
    public string Name;
    public string Location;
    public string GreetText;
    public Sprite Avatar;

    public override bool Interact(GameObject interacter)
    {
        if (!enabled)
        {
            return false;
        }

        HostilityChecker ownerHostilityChecker = GetComponent<HostilityChecker>();
        HostilityChecker interacterHostilityChecker = interacter.GetComponent<HostilityChecker>();
        if ((ownerHostilityChecker != null) && (interacterHostilityChecker != null))
        {
            if (ownerHostilityChecker.IsHostileTo(interacter))
            {
                return false;
            }
        }

        PlayerParty playerParty = interacter.GetComponent<PlayerParty>();
        if (playerParty != null)
        {
            if (playerParty.ActiveCharacter != null)
            {
                GameMgr.PlayRandomSound(
                    playerParty.ActiveCharacter.CharacterSounds.Greeting,
                    playerParty.PlayerAudioSource);
            }
            else
            {
                GameMgr.PlayRandomSound(
                    playerParty.Characters[UnityEngine.Random.Range(0, playerParty.Characters.Count)].CharacterSounds.Greeting,
                    playerParty.PlayerAudioSource);
            }
        }

        NpcTalkUI ui = GameMgr.Instance.NpcTalkUI;
        ui.NpcNameText.text = Name;
        ui.LocationNameText.text = Location;
        ui.NpcResponseText.text = GreetText;
        ui.NpcTalkCanvas.enabled = true;
        ui.NpcAvatar.sprite = Avatar;
        GameMgr.Instance.Minimap.enabled = false;
        GameMgr.Instance.MinimapCloseButtonImage.enabled = true;
        GameMgr.Instance.PartyBuffsAndButtonsCanvas.enabled = false;
        GameMgr.Instance.PauseGame();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        TextGenerator textGen = new TextGenerator();
        TextGenerationSettings generationSettings = 
            ui.NpcResponseText.GetGenerationSettings(ui.NpcResponseText.rectTransform.rect.size);
        float height = textGen.GetPreferredHeight(GreetText, generationSettings);

        float textSizeY = (height / 2.0f) / 10.0f;
        Vector2 v = new Vector2(
            ui.NpcResponseBackground.rectTransform.anchoredPosition.x, 
            NpcTalkUI.DefaultResponseY + textSizeY + 5.0f);
        ui.NpcResponseBackground.rectTransform.anchoredPosition = v;

        return true;
    }
}
