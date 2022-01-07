using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MuteSoundEffectButton : MonoBehaviour
{
    public Sprite UnMutedFxSprite;
    public Sprite MutedFxSprite;

    private Button _button;
    SpriteState _state;

    void Start()
    {
        _button = GetComponent<Button>();
        if (GameSettings.Instance.IsSoundEffectMutedPermanently())
        {
            _state.pressedSprite = MutedFxSprite;
            _state.highlightedSprite = MutedFxSprite;
            _button.GetComponent<Image>().sprite = MutedFxSprite;
        }
        else
        {
            _state.pressedSprite = UnMutedFxSprite;
            _state.highlightedSprite = UnMutedFxSprite;
            _button.GetComponent<Image>().sprite = UnMutedFxSprite;
        }

    }

    private void OnGUI()
    {
        if (GameSettings.Instance.IsSoundEffectMutedPermanently())
        {
            _button.GetComponent<Image>().sprite = MutedFxSprite;
        }
        else
        {
            _button.GetComponent<Image>().sprite = UnMutedFxSprite;
        }
    }

    public void ToggleFxIcon()
    {
        if (GameSettings.Instance.IsSoundEffectMutedPermanently())
        {
            _state.pressedSprite = UnMutedFxSprite;
            _state.highlightedSprite = UnMutedFxSprite;
            GameSettings.Instance.MuteSoundEffectPermanently(false);
        }
        else
        {
            _state.pressedSprite = MutedFxSprite;
            _state.highlightedSprite = MutedFxSprite;
            GameSettings.Instance.MuteSoundEffectPermanently(true);
        }

        _button.spriteState = _state;
    }
}
