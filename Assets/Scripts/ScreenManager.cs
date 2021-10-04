using Kaisa.Digivice.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kaisa.Digivice
{
    public class ScreenManager : MonoBehaviour
    {
        private SpriteDatabase spriteDB;
        private GameManager gm;
        private AudioManager audioMgr;
        private LogicManager logicMgr;

        public Transform RootParent => screenDisplay.transform;
        public Transform animParent;

        private Queue<IEnumerator> animationQueue = new Queue<IEnumerator>();

        private SpriteBuilder defeatedLayer;

        private SpriteBuilder defeatedLayer2;
        private SpriteBuilder eventLayer;
        private SpriteBuilder eyesLayer;
        private SpriteBuilder eyesLayer2;

        private SpriteBuilder sbClouds1;
        private SpriteBuilder sbClouds2;
        private SpriteBuilder sbTrailmon;
        private SpriteBuilder textdb;
        private SpriteBuilder blackscreen;
        private SpriteBuilder goodLayer;

        public bool PlayingAnimations { get; private set; }

        public void Initialize(GameManager gm)
        {
            this.gm = gm;
            audioMgr = gm.audioMgr;
            logicMgr = gm.logicMgr;
            spriteDB = gm.spriteDB;
            UpdateColors();
        }
        [Header("UI Elements")]
        public Image screenBackground;
        public Image screenDisplay;

        public void UpdateColors()
        {
            screenBackground.color = Preferences.BackgroundColor;
            screenDisplay.color = Preferences.ActiveColor;
        }

        public Transform ScreenParent => screenDisplay.transform;

        /// <summary>
        /// Adds a new animation the queue.
        /// </summary>
        public void EnqueueAnimation(IEnumerator animation)
        {
            animationQueue.Enqueue(animation);
            if (!PlayingAnimations) StartCoroutine(ConsumeQueue());
        }

        private void Start()
        {
            //InvokeRepeating("UpdateDisplay", 0f, 0.05f);
            eyesLayer2 = ScreenElement.BuildSprite("Eyes", RootParent.transform).SetTransparent(true).SetActive(false);

            defeatedLayer = ScreenElement.BuildSprite("Defeated", RootParent.transform).SetSize(6, 7)
                .SetPosition(1, 1).SetTransparent(true).SetActive(false);
            defeatedLayer2 = ScreenElement.BuildSprite("Defeated", RootParent.transform).SetSize(6, 7)
            .SetPosition(1, 1).SetTransparent(true).SetActive(false);
            goodLayer = ScreenElement.BuildSprite("good", RootParent.transform).SetActive(false);
            eventLayer = ScreenElement.BuildSprite("Event", RootParent.transform).SetTransparent(true).SetActive(false);
            eyesLayer = ScreenElement.BuildSprite("Eyes", RootParent.transform).SetTransparent(true).SetActive(false);

            blackscreen = ScreenElement.BuildSprite("end", RootParent.transform).SetSprite(spriteDB.blackScreen).InvertColors(true).SetTransparent(true).SetActive(false);

            sbClouds1 = ScreenElement.BuildSprite("end", RootParent.transform).SetSize(76, 8).SetY(24).SetSprite(spriteDB.clouds_foot).SetX(-46).SetActive(false);
            sbClouds2 = ScreenElement.BuildSprite("end", RootParent.transform).SetSize(76, 8).SetY(24).SetSprite(spriteDB.clouds_foot).SetX(-122).SetActive(false);
            sbTrailmon = ScreenElement.BuildSprite("end", RootParent.transform).SetSize(32, 15).SetSprite(spriteDB.end_trailmon).SetY(9).SetActive(false);
            textdb = ScreenElement.BuildSprite("end", RootParent.transform).SetSize(60, 8).SetSprite(spriteDB.FINAL_TEXT).PlaceOutside(Direction.Right).SetActive(false);



            StartCoroutine(PAFlashEyesEffect());
            StartCoroutine(PAFlashDefeatedEffect());
            StartCoroutine(PAFlashDefeatedEffectandeyes());
            StartCoroutine(PAFlashEventEffect());
            StartCoroutine(PAFlashfgoodEffect());

            StartCoroutine(EndGame());

            StartCoroutine(ConsumeQueue());
        }

        private IEnumerator PAFlashfgoodEffect()
        {

            goodLayer.transform.SetAsFirstSibling();
            while (true)
            {
                goodLayer.SetSprite(spriteDB.GetCharacterSprites(gm.PlayerChar)[0]);
                yield return new WaitForSeconds(0.2f);
                goodLayer.SetSprite(spriteDB.GetCharacterSprites(gm.PlayerChar)[6]);
                yield return new WaitForSeconds(0.2f);
            }
        }

        private IEnumerator EndGame()
        {

            sbClouds1.transform.SetAsFirstSibling();
            sbClouds2.transform.SetAsFirstSibling();
            textdb.transform.SetAsFirstSibling();
            sbTrailmon.transform.SetAsFirstSibling();
            blackscreen.transform.SetAsFirstSibling();


            int i = 0;


            while (true)
            {


                if (sbClouds1.Position.x == 32)
                {
                    sbClouds1.SetX(-122);

                }
                if (sbClouds2.Position.x == 32)
                {
                    sbClouds2.SetX(-122);

                }

                if (textdb.Position.x == -60)
                {
                    textdb.PlaceOutside(Direction.Right);

                }
                if (i % 2 == 0) { sbClouds1.Move(Direction.Right); sbClouds2.Move(Direction.Right); i = 0; textdb.Move(Direction.Left); }
                i++;
                // 

                yield return new WaitForSeconds(4.2f / 65);
            }


        }
        private IEnumerator PAFlashDefeatedEffect()
        {
            defeatedLayer.transform.SetAsFirstSibling();
            while (true)
            {
                defeatedLayer.SetSprite(spriteDB.defeatedSymbol);
                yield return new WaitForSeconds(0.5f);
                defeatedLayer.SetSprite(spriteDB.emptySprite);
                yield return new WaitForSeconds(0.5f);
            }
        }
        private IEnumerator PAFlashDefeatedEffectandeyes()
        {
            defeatedLayer2.transform.SetAsFirstSibling();
            eyesLayer2.transform.SetAsFirstSibling();
            //baduserLayer.transform.SetAsFirstSibling();

            while (true)
            {

                defeatedLayer2.SetSprite(spriteDB.defeatedSymbol);
                eyesLayer2.SetSprite(spriteDB.eyes[0]);
                yield return new WaitForSeconds(0.5f);
                eyesLayer2.SetSprite(spriteDB.eyes[1]);
                defeatedLayer2.SetSprite(spriteDB.emptySprite);
                yield return new WaitForSeconds(0.5f);


            }
        }
        private IEnumerator PAFlashEventEffect()
        {
            eventLayer.transform.SetAsFirstSibling();
            while (true)
            {
                eventLayer.SetSprite(spriteDB.triggerEvent);
                yield return new WaitForSeconds(0.2f);
                eventLayer.SetSprite(spriteDB.emptySprite);
                yield return new WaitForSeconds(0.2f);
            }
        }
        private IEnumerator PAFlashEyesEffect()
        {
            eyesLayer.transform.SetAsFirstSibling();
            while (true)
            {
                eyesLayer.SetSprite(spriteDB.eyes[0]);
                yield return new WaitForSeconds(Random.Range(0.25f, 1f));
                eyesLayer.SetSprite(spriteDB.eyes[1]);
                yield return new WaitForSeconds(Random.Range(0.25f, 1f));
            }
        }

        private IEnumerator ConsumeQueue()
        {
            PlayingAnimations = true;
            while (animationQueue.Count > 0)
            {
                gm.LockInput();
                animParent = ScreenElement.BuildContainer("Anim Parent", ScreenParent, false).SetSize(32, 32).transform;
                yield return animationQueue.Dequeue();
                Destroy(animParent.gameObject);
                gm.UnlockInput();
            }
            PlayingAnimations = false;
            gm.CheckPendingEvents();
        }
        private void ClearAnimParent()
        {
            foreach (Transform child in animParent) Destroy(child.gameObject);
        }

        private void Update()
        {
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            foreach (Transform child in RootParent)
            {
                if (child.gameObject.tag.ToLower() == "disposable")
                {
                    Destroy(child.gameObject);
                }
            }

            int showLayer = 0; //0: none, 1: defeated, 2: event, 3: eyes.
            if (logicMgr.currentScreen == Screen.Character)
            {
                if (gm.IsCharacterDefeated && gm.showEyes) showLayer = 6;
                else if (gm.IsCharacterDefeated) showLayer = 1;

                else if (gm.IsEventActive && !gm.IsEventRecovery) showLayer = 2;
                else if (gm.IsEventRecovery) showLayer = 5;
                else if (gm.showEyes) showLayer = 3;
                else if (gm.showEnd) showLayer = 4;

            }
            if (logicMgr.currentScreen == Screen.CharSelection)
            {
                if (gm.showEnd) showLayer = 4;
            }
            defeatedLayer.SetActive(showLayer == 1);
            eventLayer.SetActive(showLayer == 2);
            eyesLayer.SetActive(showLayer == 3);
            blackscreen.SetActive(showLayer == 4);
            sbClouds1.SetActive(showLayer == 4);
            sbClouds2.SetActive(showLayer == 4);
            textdb.SetActive(showLayer == 4);
            sbTrailmon.SetActive(showLayer == 4);
            goodLayer.SetActive(showLayer == 5);
            eyesLayer2.SetActive(showLayer == 6);
            defeatedLayer2.SetActive(showLayer == 6);
            // baduserLayer.SetActive(showLayer == 6);


            int index;
            Sprite sprite;
            switch (logicMgr.currentScreen)
            {
                case Screen.CharSelection:
                    if (!gm.showEnd)
                    {
                        SpriteBuilder sb = ScreenElement.BuildSprite("ArrowsChar", screenDisplay.transform).SetY(4).SetSprite(spriteDB.arrows).SetTransparent(true);
                        sb.gameObject.tag = "Disposable";
                        sb.transform.SetAsFirstSibling();

                        SetScreenSprite(spriteDB.GetCharacterSprites((GameChar)logicMgr.charSelectionIndex)[0]);
                    }
                    break;
                case Screen.Character:

                    SetScreenSprite(gm.PlayerCharSprites[gm.CurrentPlayerCharSprite]);
                    break;
                case Screen.MainMenu:
                    index = (int)logicMgr.currentMainMenu;
                    sprite = spriteDB.mainMenu[index];
                    SetScreenSprite(sprite);
                    break;

                case Screen.MainMenu2:
                    index = (int)logicMgr.currentMainMenu2;
                    sprite = spriteDB.mainMenu2[index];
                    SetScreenSprite(sprite);
                    break;
                case Screen.GamesMenu:
                    index = logicMgr.gamesMenuIndex;
                    sprite = spriteDB.game_sections[index];
                    SetScreenSprite(sprite);
                    break;
                case Screen.GamesRewardMenu:
                    index = logicMgr.gamesRewardMenuIndex;
                    sprite = spriteDB.games_reward[index];
                    SetScreenSprite(sprite);
                    break;
                case Screen.GamesTravelMenu:
                    index = logicMgr.gamesTravelMenuIndex;
                    sprite = spriteDB.games_travel[index];
                    SetScreenSprite(sprite);
                    break;
                default:
                    SetScreenSprite(spriteDB.emptySprite);
                    break;
            }
        }

        private void SetScreenSprite(Sprite sprite)
        {
            screenDisplay.sprite = sprite;
        }
    }
}