using Kaisa.Digivice.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Kaisa.Digivice.Apps {
    public class SpiritApp : DigiviceApp {
        private enum ScreenDatabase { //TODO: Replace with an int
            Menu_Spirit,
            Gallery,
            Pages

        }

        Coroutine screenAnimation;
        Coroutine screenAnimation2;
        //Current screen
        private ScreenDatabase currentScreen = ScreenDatabase.Menu_Spirit;
        private int menuIndex = 0;

        //Gallery viewer
        
        private List<string> galleryList = new List<string>(); //Stores the names of all Pokémon that must be shown in this gallery.
        private int galleryIndex = 0;

        //Hybrid gallery menu
        private List<int> availableElements;
        private Dictionary<int,int> availableElementsSipirt;
        private int elementIndex = 0; //This points which int in availableElements is used as the current element selected.
        private int elementSpiritIndex=0;
        private int SelectedElementSpirit => availableElementsSipirt[elementSpiritIndex];

        private int SelectedElement => availableElements[elementIndex];

        //Data pages
        private int pageIndex = 0; //This is restricted to 0 or 1, or sometimes 2 when the player can see the code of the Digimon.
        private Digimon pageDigimon;
        private GameObject digimonNameSign;

        //DDock list/display
        private int ddockIndex = 0;

        #region Input
        public override void InputA() {
            
                           
           

            if (currentScreen == ScreenDatabase.Menu_Spirit) {
                if (SelectedElement <23) {
                    //galleryList = gm.GetAllUnlockedHumanAndAnimalSpirits();
                    //galleryList = gm.GetAllUnlockedSpiritsOfHumanAndAnimal();
                    if (galleryList.Count > 0) {
                        audioMgr.PlayButtonA();
                        OpenGallery();
                    }
                    else {
                        audioMgr.PlayButtonB();
                    }
                }
                /**else {
                    galleryList = gm.GetAllUnlockedFusionDigimon();
                    if (galleryList.Count > 0) {
                        audioMgr.PlayButtonA();
                        OpenGallery();
                    }
                    else {
                        audioMgr.PlayButtonB();
                    }
                }**/
            }
            else if (currentScreen == ScreenDatabase.Gallery) {
                audioMgr.PlayButtonA();
                if(gm.IsSpiritCharacterAccesible(galleryList[galleryIndex])){
                OpenPages();}
            }
            else if (currentScreen == ScreenDatabase.Pages) {
                audioMgr.PlayButtonA();
               
              
            }
         
        }
        public override void InputB() {
           
            if (currentScreen == ScreenDatabase.Menu_Spirit) {
                audioMgr.PlayButtonB();
                CloseApp();
            }
            else if (currentScreen == ScreenDatabase.Gallery) {
                audioMgr.PlayButtonB();
                CloseGallery();
            }
            else if (currentScreen == ScreenDatabase.Pages) {
                audioMgr.PlayButtonB();
                ClosePages();
            }
        }
        
        public override void InputRight() {
           if (currentScreen == ScreenDatabase.Menu_Spirit) {
                audioMgr.PlayButtonA();
                NavigateSpiritMenu(Direction.Right);
               
            }
            else if (currentScreen == ScreenDatabase.Pages) {
                audioMgr.PlayButtonA();
                NavigatePages(Direction.Right);
            }
        }
       /** public override void InputRightDown() {
            if (currentScreen == ScreenDatabase.Gallery) {
                if (galleryList.Count <= 1) {
                    audioMgr.PlayButtonB();
                }
                else {
                    StartNavigation(Direction.Right);
                }
            }
        }
        public override void InputRightUp() {
            StopNavigation();
        }**/
        protected override IEnumerator AutoNavigateDir(Direction dir) {
            audioMgr.PlayButtonA();
            NavigateGallery(dir);
            yield return new WaitForSeconds(0.35f);
            while (true) {
                yield return new WaitForSeconds(0.12f);
                audioMgr.PlayButtonA();
                NavigateGallery(dir);
            }
        }
        #endregion

        public override void StartApp() {
          
           galleryList = gm.GetAllUnlockedSpiritsOfHumanAndAnimal();
           if(galleryList.Count==0){
                CloseApp();
           }else{
                 OpenSpiritMenu();
           }
           
        }

        private void DrawScreen() {

           
            //Stop all coroutines, except if the digimon name sign has a value and we are still in the 'Pages' screen.
            if(!(digimonNameSign != null && (currentScreen == ScreenDatabase.Pages || currentScreen == ScreenDatabase.Gallery))) {
                if (screenAnimation != null) StopCoroutine(screenAnimation);
                if (screenAnimation2 !=null) StopCoroutine(screenAnimation2);
            }else{
                if (screenAnimation2 !=null) StopCoroutine(screenAnimation2);
            }
            //Destroy all children, except the ones called 'NameSign' if we are in the 'Pages' screen.
            foreach (Transform child in screenDisplay.transform) {
                if (!((currentScreen == ScreenDatabase.Gallery || (currentScreen == ScreenDatabase.Pages) ) && child.name == "NameSign")) {
                    Destroy(child.gameObject);
               }
            }
           
            
            

           if (currentScreen == ScreenDatabase.Menu_Spirit) {
                if ((SelectedElement) < 23) {
                    SpriteBuilder arrows= ScreenElement.BuildSprite("Arrows", screenDisplay.transform).SetSprite(gm.spriteDB.arrows);
                    SpriteBuilder sbSpirit = ScreenElement.BuildSprite("Element", screenDisplay.transform).SetSize(22, 22).Center().SetY(1).SetSprite(gm.spriteDB.Spirits[SelectedElement-11]);
                    SpriteBuilder sbSpiritElement = ScreenElement.BuildSprite("SpiritElement", screenDisplay.transform).SetSize(32,5).Center().SetY(24).SetSprite(gm.spriteDB.elementNames[SelectedElementSpirit]);

                     
                }
              /**  else {
                    screenDisplay.sprite = gm.spriteDB.database_spirit_fusion;
                }**/
            }
            else if (currentScreen == ScreenDatabase.Gallery) {
                if(digimonNameSign == null) {
                    string name = string.Format("{0:000} {1}", pageDigimon.number, pageDigimon.name);
                    TextBoxBuilder nameBuilder = ScreenElement.BuildTextBox("NameSign", screenDisplay.transform, DFont.Big).SetText(name).SetSize(32, 7).SetPosition(32, 0);
                    nameBuilder.SetFitSizeToContent(true);
                    digimonNameSign = nameBuilder.gameObject;
                    screenAnimation = StartCoroutine(AnimateName(nameBuilder));
                }
                string displayDigimon = galleryList[galleryIndex];
              
                //screenDisplay.sprite =  gm.spriteDB.arrows;

                Sprite spriteRegular = gm.spriteDB.GetDigimonSprite(displayDigimon, SpriteAction.Default);
                Sprite spriteAlt = gm.spriteDB.GetDigimonSprite(displayDigimon, SpriteAction.Attack);

                SpriteBuilder empty = ScreenElement.BuildSprite("DigimonDisplay", screenDisplay.transform).SetSize(22, 22).SetY(8).SetSprite(gm.spriteDB.emptySprite);


                SpriteBuilder builder = ScreenElement.BuildSprite("DigimonDisplay", screenDisplay.transform).SetSize(22, 22).Center().SetY(8).SetSprite(spriteRegular);

                screenAnimation2 = StartCoroutine(AnimateSprite(builder, spriteRegular, spriteAlt));
            }
            else if (currentScreen == ScreenDatabase.Pages) {
                
               
                int playerLevel = gm.logicMgr.GetPlayerLevel();
                int digimonExtraLevel = gm.logicMgr.GetDigimonExtraLevel(pageDigimon.name);
                int realLevel;

                MutableCombatStats stats;
                             
                realLevel = pageDigimon.GetBossLevel(playerLevel);
                stats = pageDigimon.GetBossStats(playerLevel);


                int element = (int)pageDigimon.element;

                if (pageIndex == 0) {
                    //SpriteBuilder arrows= ScreenElement.BuildSprite("Arrows", screenDisplay.transform).SetSprite(gm.spriteDB.database_pages[0]); 
                     screenDisplay.sprite=gm.spriteDB.database_pages[0];
                    ScreenElement.BuildTextBox("Level", screenDisplay.transform, DFont.Regular)
                        .SetText(realLevel.ToString()).SetSize(15, 5).SetPosition(16, 9).SetAlignment(TextAnchor.UpperRight);
                    ScreenElement.BuildTextBox("HP", screenDisplay.transform, DFont.Regular)
                        .SetText(stats.HP.ToString()).SetSize(15, 5).SetPosition(16, 17).SetAlignment(TextAnchor.UpperRight);
                    //ScreenElement.BuildSprite("Element", screenDisplay.transform).SetSize(30, 5).SetPosition(1, 25).SetSprite(gm.spriteDB.elementNames[element]);
                }
                else if (pageIndex == 1) {
                     screenDisplay.sprite=gm.spriteDB.database_pages[1];
                    //SpriteBuilder arrows= ScreenElement.BuildSprite("Arrows", screenDisplay.transform).SetSprite(gm.spriteDB.database_pages[1]);                   
                    ScreenElement.BuildTextBox("Energy", screenDisplay.transform, DFont.Regular)
                        .SetText(stats.EN.ToString()).SetSize(15, 5).SetPosition(16, 9).SetAlignment(TextAnchor.UpperRight);
                    ScreenElement.BuildTextBox("Crush", screenDisplay.transform, DFont.Regular)
                        .SetText(stats.CR.ToString()).SetSize(15, 5).SetPosition(16, 17).SetAlignment(TextAnchor.UpperRight);
                    ScreenElement.BuildTextBox("Ability", screenDisplay.transform, DFont.Regular)
                        .SetText(stats.AB.ToString()).SetSize(15, 5).SetPosition(16, 25).SetAlignment(TextAnchor.UpperRight);
                }
                else if (pageIndex == 2) {
                    screenDisplay.sprite=gm.spriteDB.database_pages[2];
                   // SpriteBuilder arrows= ScreenElement.BuildSprite("Arrows", screenDisplay.transform).SetSprite(gm.spriteDB.database_pages[2]);
                    ScreenElement.BuildTextBox("Code", screenDisplay.transform, DFont.Big)
                        .SetText(pageDigimon.code).SetSize(30, 8).SetPosition(2, 23).SetAlignment(TextAnchor.UpperRight);
                }
            }
        }

        private void NavigateStageMenu(Direction dir) {
            if (dir == Direction.Left) menuIndex = menuIndex.CircularAdd(-1, 7);
            else menuIndex = menuIndex.CircularAdd(1, 7);
            DrawScreen();
        }

        private void OpenSpiritMenu() {
            availableElements = new List<int>();
            availableElementsSipirt = new Dictionary<int,int>();
            HashSet<int> elementsFound = new HashSet<int>(); //a list of elements found that will contain only 1 of each.
            HashSet<int> elementsSpiritFound = new HashSet<int>(); //a list of elements found that will contain only 1 of each.
            int i=0;
            
            foreach(string d in galleryList) {

                elementsFound.Add((int)Database.GetDigimon(d).order);
                availableElementsSipirt.Add(i,(int)Database.GetDigimon(d).element);
                i++;

            }
            /**if(gm.GetAllUnlockedFusionDigimon().Count > 0) {
                elementsFound.Add(10);
            }**/

            availableElements = elementsFound.ToList();
           
            
            elementIndex = 0;
            elementSpiritIndex=0;
            currentScreen = ScreenDatabase.Menu_Spirit;
            DrawScreen();
        }
        private void NavigateSpiritMenu(Direction dir) {
            if (dir == Direction.Left) elementIndex = elementIndex.CircularAdd(-1, availableElements.Count - 1);
            else elementIndex = elementIndex.CircularAdd(1, availableElements.Count - 1);
             if (dir == Direction.Left) elementSpiritIndex = elementSpiritIndex.CircularAdd(-1, availableElementsSipirt.Count - 1);
            else elementSpiritIndex = elementSpiritIndex.CircularAdd(1, availableElementsSipirt.Count - 1);
            int maxIndex = galleryList.Count - 1;
            if (dir == Direction.Left) galleryIndex = galleryIndex.CircularAdd(-1, maxIndex);
            else galleryIndex = galleryIndex.CircularAdd(1, maxIndex);
            DrawScreen();
        }
       
       

        private void OpenGallery() {
          
                //If an element is chosen.
               /** if(elementIndex <= 10) {
                    foreach (Digimon d in Database.Digimons) {
                        if ((int)d.stage == menuIndex && (int)d.element == elementIndex && d.spiritType != SpiritType.Fusion && d.spiritType != SpiritType.Ancient && gm.logicMgr.GetDigimonUnlocked(d.name)) {
                            galleryList.Add(d.name);
                        }
                    }**/
                
                //If fusion is chosen.
              /**if (elementIndex == 10) {
                    foreach (Digimon d in Database.Digimons) {
                        if ((int)d.stage == menuIndex && d.spiritType == SpiritType.Fusion && gm.logicMgr.GetDigimonUnlocked(d.name)) {
                            galleryList.Add(d.name);
                        }
                    }
                }
            }**/
            string displayDigimon = galleryList[galleryIndex];
            pageDigimon = Database.GetDigimon(displayDigimon);
            //galleryIndex = 0;
            currentScreen = ScreenDatabase.Gallery;
            DrawScreen();
        }

        private void NavigateGallery(Direction dir) {
            int maxIndex = galleryList.Count - 1;
            if (dir == Direction.Left) galleryIndex = galleryIndex.CircularAdd(-1, maxIndex);
            else galleryIndex = galleryIndex.CircularAdd(1, maxIndex);
            DrawScreen();
        }

        private void CloseGallery() {
            
            currentScreen = ScreenDatabase.Menu_Spirit;
            
            DrawScreen();
        }

        private void OpenPages() {
            currentScreen = ScreenDatabase.Pages;
            pageIndex = 0;

            string displayDigimon = galleryList[galleryIndex];
            pageDigimon = Database.GetDigimon(displayDigimon);

            DrawScreen();
        }

        private void NavigatePages(Direction dir) {
            int upperBound = (gm.logicMgr.GetDigicodeUnlocked(pageDigimon.name)) ? 2 : 1;

            if (dir == Direction.Left) pageIndex = pageIndex.CircularAdd(-1, upperBound);
            else pageIndex = pageIndex.CircularAdd(1, upperBound);

            DrawScreen();
        }

        private void ClosePages() {
            currentScreen = ScreenDatabase.Gallery;
            DrawScreen();
        }


        private void NavigateDDock(Direction dir) {
            if (dir == Direction.Left) ddockIndex = ddockIndex.CircularAdd(-1, 3);
            else ddockIndex = ddockIndex.CircularAdd(1, 3);

            DrawScreen();
        }

      

        #region Animations
        private IEnumerator AnimateSprite(SpriteBuilder builder, Sprite spriteRegular, Sprite spriteAlt) {
            while (true) {
                yield return new WaitForSeconds(2.5f);
                builder.SetSprite(spriteAlt);
                yield return new WaitForSeconds(0.4f);
                builder.SetSprite(spriteRegular);
                yield return new WaitForSeconds(0.4f);
                builder.SetSprite(spriteAlt);
                yield return new WaitForSeconds(0.4f);
                builder.SetSprite(spriteRegular);
            }
        }

        private IEnumerator AnimateName(TextBoxBuilder builder) {
            yield return null; //Wait for the next frame so the builder's text fitter has adjusted the component's Width.
            int goWidth = builder.Width;

            while (true) {
                builder.SetPosition(32, 0);
                for (int i = 0; i < goWidth + 32; i++) {
                    builder.Move(Direction.Left);
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(1.5f);
            }
        }
        #endregion
    }
}