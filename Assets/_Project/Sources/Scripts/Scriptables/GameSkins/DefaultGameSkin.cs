using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using WEngine;
using Sirenix.OdinInspector;


[CreateAssetMenu(menuName = "Scriptables/GameSkin/Default")]
public class DefaultGameSkin : GameSkinScriptable {
    [Title("Background Settings")]
    [SerializeField] private float backgroundSwapSpeed;
    [SerializeField] private float backgroundSwapInterval;

    private Image background1;
    private Image background2;
    private Image currentBackground;

    private ActionQueue swapBackgroundAction;


    public override void DoBackgroundsBehaviour(Image background1, Image background2) {
        this.background1 = background1;
        this.background2 = background2;

        background1.color = Color.white;
        background2.color = new Color(1f, 1f, 1f, 0f);
        currentBackground = background1;

        swapBackgroundAction = new ActionQueue("ClassicGameSkin: SwapBackground");
        swapBackgroundAction.AddAction(SwapBackground());
        swapBackgroundAction.Start();
    }

    // This function is a side-effect because with it now I can
    // change the backgroundspeed through the editor on the fly
    // And besides I'm not gonna use it outside of this object,
    // we have the DoBackgroundsBehaviour for that.
    private IEnumerator SwapBackground() {
        while(true) {
            Image lastBackground = currentBackground;
            yield return null;

            // Swap backgrounds
            currentBackground = currentBackground == background1 ? background2 : background1;
            currentBackground.sprite = Settings.BackgroundSprites[UnityEngine.Random.Range(0, Settings.BackgroundSprites.Length)];

            yield return null;

            // Lerp effect for the background swap
            while(true) {
                lastBackground.color = Color.Lerp(lastBackground.color, new Color(1f, 1f, 1f, 0f), backgroundSwapSpeed * Time.deltaTime);
                currentBackground.color = Color.Lerp(currentBackground.color, Color.white, backgroundSwapSpeed * Time.deltaTime);
                yield return null;

                if(lastBackground.color.a <= .01f && currentBackground.color.a >= .99f)
                    break;
            }
            yield return null;

            // Interval before next swapping
            yield return new WaitForSeconds(backgroundSwapInterval);
        }
    }

    public override void Init() {
        
    }

    public override void DeInit() {
        swapBackgroundAction.Stop();
    }

    public override void Start() {
        
    }

    public override void Update() {
        
    }
}
