using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kaisa.Digivice.Apps
{
    public class Finder : DigiviceApp
    {
        public override void InputB()
        {
            if (result == 0 || result == 2)
            {
                audioMgr.PlayButtonB();
                CloseApp(Screen.MainMenu2);
            }
        }
        public override void InputA()
        {
            if (result == 2)
            {
                audioMgr.PlayButtonA();
                result = 0;
            }
        }
        public override void InputLeftDown()
        {
            //Remove error screen.

            if (result != 3 && result != 2)
            {
                audioMgr.PlayButtonA();
                tries = 0;
                StartLoadingBar();
            }
        }
        public override void InputLeftUp()
        {
            if (result != 3 && result != 2)
            {
                result = 0;
                StopLoadingBar();
            }
        }
        public override void StartApp()
        {
            InvokeRepeating("DisplayPressASprite", 0f, 0.45f);
        }

        private bool state = false;
        private void DisplayPressASprite()
        {
            if (result == 0)
            {
                if (sbError != null) sbError.Dispose();
                if (state) SetScreen(gm.spriteDB.pressAButton[3]);
                else SetScreen(gm.spriteDB.pressAButton[2]);
                state = !state;
            }
        }

        SpriteBuilder sbHourglass;
        RectangleBuilder rbBlackScreen;
        SpriteBuilder sbLoading;
        Coroutine loadingCoroutine;
        SpriteBuilder sbError;
        private int tries = 0;
        private int result = 0; //0: nothing, 1: loading, 2: failure, 3: succeed.
        private void StartLoadingBar()
        {
            if (sbError != null) sbError.Dispose();
            result = 1;
            rbBlackScreen = ScreenElement.BuildRectangle("BlackScreen0", Parent).SetSize(32, 32);
            sbLoading = ScreenElement.BuildSprite("Loading", Parent).SetSprite(gm.spriteDB.loading).PlaceOutside(Direction.Up);
            loadingCoroutine = StartCoroutine(AnimateLoadingBar());
        }
        private void StopLoadingBar()
        {
            if (loadingCoroutine != null)
            {
                StopCoroutine(loadingCoroutine);
                if (rbBlackScreen != null) rbBlackScreen.Dispose();
                if (sbLoading != null) sbLoading.Dispose();
                if (sbHourglass != null) sbHourglass.Dispose();
            }
        }

        private IEnumerator AnimateLoadingBar()
        {
            sbHourglass = ScreenElement.BuildSprite("Hourglass", Parent).SetSprite(gm.spriteDB.hourglass);
            yield return new WaitForSeconds(0.5f);
            sbHourglass.Dispose();

            while (result == 1)
            {
                if (tries == 4)
                {
                    int thisRoundRNG = Random.Range(0, 2);
                    VisualDebug.WriteLine($"RNG: {thisRoundRNG}");
                    if (thisRoundRNG == 0)
                    {
                        result = 3;
                        break;
                    }
                    else
                    {

                        result = 2;
                        break;
                    }
                }


                sbLoading.PlaceOutside(Direction.Up);
                for (int i = 0; i < 64; i++)
                {
                    sbLoading.Move(Direction.Down);
                    yield return new WaitForSeconds(1.75f / 64);
                }
                tries++;
            }
            if (result == 2)
            {

                sbError = ScreenElement.BuildSprite("Error", screenDisplay.transform).Center().SetSprite(gm.spriteDB.error_finder);



                rbBlackScreen.Dispose();
                sbLoading.Dispose();
            }
            else if (result == 3)
            {
                StartCoroutine(AnimateSuccessBar());
            }
        }

        private IEnumerator AnimateSuccessBar()
        {
            SpriteBuilder sbSuccessBar = ScreenElement.BuildSprite("LoadingSuccessful", Parent)
                .SetSize(32, 82)
                .SetSprite(gm.spriteDB.loadingComplete)
                .PlaceOutside(Direction.Up);
            for (int i = 0; i < 82 + 32; i++)
            {
                sbSuccessBar.Move(Direction.Down);
                yield return new WaitForSeconds(1.75f / 64);
            }

            Reward reward = GetRandomReward(Random.Range(1, 4));
            //Reward reward =Reward.ForceLevelUp;



            if (reward == Reward.LevelDown && gm.logicMgr.GetPlayerLevelProgression() > 0.5f
                || reward == Reward.ForceLevelDown && gm.logicMgr.GetPlayerLevelProgression() == 0f)
            {
                reward = Reward.IncreaseDistance500;
            }
            else if (reward == Reward.LevelUp && gm.logicMgr.GetPlayerLevelProgression() < 0.5f
                || reward == Reward.ForceLevelUp && gm.logicMgr.GetPlayerLevelProgression() == 0f)
            {
                reward = Reward.ReduceDistance500;
            }
            else if (reward == Reward.RewardDigimon)
            {
                gm.logicMgr.ApplyReward(reward, null, out object resultBefore, out object resultAfter);

                // }
                // else if (reward == Reward.UnlockDigicodeOwned) {
                //     gm.logicMgr.ApplyReward(reward, null, out object resultBefore, out object resultAfter);
                //     //gm.EnqueueRewardAnimation(reward, null, resultBefore, resultAfter);
                // }
                // else if (reward == Reward.UnlockDigicodeNotOwned) {
                //     gm.logicMgr.ApplyReward(reward, null, out object resultBefore, out object resultAfter);
                //    // gm.EnqueueRewardAnimation(reward, null, resultBefore, resultAfter);
            }
            else if (reward == Reward.TriggerBattle)
            {
                CloseApp();
                if (controller is LogicManager logicMgr)
                {
                    logicMgr.CallRandomBattle(true);
                }
                yield return null;
            }
            else
            {
                gm.logicMgr.ApplyReward(reward, null, out object resultBefore, out object resultAfter);
                gm.EnqueueRewardAnimation(reward, null, resultBefore, resultAfter);




            }
            CloseApp(Screen.Character);
        }

        private Reward GetRandomReward(int category)
        {
            float rng = Random.Range(0f, 1f);
            switch (category)
            {
                //case 0:
                //     if (rng < 0.40f) return Reward.IncreaseDistance500;
                //     else if (rng < 0.60f) return Reward.PunishDigimon;
                //     else if (rng < 0.70f) return Reward.DataStorm;
                //     else if (rng < 0.80f) return Reward.LoseSpiritPower10;
                //     else if (rng < 0.90f) return Reward.ForceLevelDown;
                //     else if (rng < 0.95f) return Reward.PunishDigimon;
                //     else return Reward.IncreaseDistance2000;
                case 1:
                    if (rng < 0.40f) return Reward.IncreaseDistance300;
                    else if (rng < 0.65f) return Reward.TriggerBattle;
                    // else if (rng < 0.75f) return Reward.PunishDigimon;
                    else if (rng < 0.75f) return Reward.GainSpiritPower10;
                    else if (rng < 0.85f) return Reward.DataStorm;
                    // else if (rng < 0.95f) return Reward.LoseSpiritPower10;
                    else return Reward.TriggerBattle;
                case 2:
                    if (rng < 0.35f) return Reward.ReduceDistance500;
                    else if (rng < 0.65f) return Reward.TriggerBattle;
                    else if (rng < 0.80f) return Reward.IncreaseDistance300;
                    else if (rng < 0.90f) return Reward.GainSpiritPower10;
                    else return Reward.DataStorm;
                case 3:
                    if (rng < 0.30f) return Reward.ReduceDistance500;
                    else if (rng < 0.55f) return Reward.GainSpiritPower10;
                    else if (rng < 0.80f) return Reward.RewardDigimon;
                    else if (rng < 0.95f) return Reward.LevelUp;
                    else return Reward.RewardDigimon;
                case 4:
                    if (rng < 0.55f) return Reward.RewardDigimon;
                    else if (rng < 0.65f) return Reward.ReduceDistance500;
                    else if (rng < 0.75f) return Reward.ForceLevelUp;
                    else if (rng < 0.85f) return Reward.GainSpiritPower10;
                    else return Reward.RewardDigimon;

                default: return Reward.IncreaseDistance300;
            }
        }
    }
}