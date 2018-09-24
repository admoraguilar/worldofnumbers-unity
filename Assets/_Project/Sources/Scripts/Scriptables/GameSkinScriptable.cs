using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;


public abstract class GameSkinScriptable : CustomScriptableObject {
    [Title("Defaults")]
    public Sprite RectangleButtonSprite;
    public Sprite NarrowRectangleButtonSprite;
    public Sprite WideRectangleButtonSprite;

    public Sprite BackToGameSprite;

    [Title("Main Menu")]
    public Sprite AudioOnSprite;
    public Sprite AudioOffSprite;

    [Title("Game Menu")]
    public Sprite HowToPlaySprite;
    public Sprite SkinsSprite;
    public Sprite SettingsSprite;
    public Sprite LeaderboardsSprite;
    public Sprite ShareSprite;


    public virtual void Init() { }
    public virtual void DeInit() { }
    public virtual void Start() { }
    public virtual void Update() { }

    public virtual void DoBackgroundsBehaviour(Image background1, Image background2) { }
}
