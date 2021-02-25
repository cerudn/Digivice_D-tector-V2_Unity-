using Kaisa.Digivice.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Kaisa.Digivice.Apps {
    public class StatusApp : DigiviceApp {
        private enum ScreenStatus { //TODO: Replace with an int
            Menu_Character,
            Gallery,
            Pages

        }

        Coroutine screenAnimation;
        Coroutine screenAnimation2;
        //Current screen
        private ScreenStatus currentScreen = ScreenStatus.Menu_Character;
        private int menuIndex = 0;

        //Gallery viewer
        
        private List<string> galleryList = new List<string>(); //Stores the names of all Pokémon that must be shown in this gallery.
        private int galleryIndex = 0;

        //Hybrid gallery menu
        private List<int> availableElements;
        private Dictionary<int,int> availableElementscharacter;
        private int elementIndex = 0; //This points which int in availableElements is used as the current element selected.
        private int elementCharacterIndex=0;
        private int SelectedElementCharacter => availableElementscharacter[elementCharacterIndex];

        private int SelectedElement => availableElements[elementIndex];

        //Data pages
        private int pageIndex = 0; //This is restricted to 0 or 1, or sometimes 2 when the player can see the code of the Digimon.
        private Characters pageCharacter;
        private GameObject characterNameSign;

    

        #region Input
        public override void InputA() {
            
                           
           

            if (currentScreen == ScreenStatus.Menu_Character) {
                
                       audioMgr.PlayButtonA();
                       OpenPages();
                
                
            }
            else if (currentScreen == ScreenStatus.Pages) {
                audioMgr.PlayButtonA();
               
              
            }
         
        }
        public override void InputB() {
           
            if (currentScreen == ScreenStatus.Menu_Character) {
                audioMgr.PlayButtonB();
                CloseApp();
            }
            else if (currentScreen == ScreenStatus.Pages) {
                audioMgr.PlayButtonB();
                ClosePages();
            }
        }
        
        public override void InputRight() {
           if (currentScreen == ScreenStatus.Menu_Character) {
                audioMgr.PlayButtonA();
                NavigateChartMenu(Direction.Right);
               
            }
            else if (currentScreen == ScreenStatus.Pages) {
                audioMgr.PlayButtonA();
                NavigatePages(Direction.Right);
            }
        }
    
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
           galleryList = gm.GetAllCharacterWithPlayer();
            OpenChartMenu();
        }

        private void DrawScreen() {

           
            //Stop all coroutines, except if the digimon name sign has a value and we are still in the 'Pages' screen.
            if(!(characterNameSign != null && currentScreen == ScreenStatus.Pages) ) {
                if (screenAnimation != null) StopCoroutine(screenAnimation);                
            }
            //Destroy all children, except the ones called 'NameSign' if we are in the 'Pages' screen.
            foreach (Transform child in screenDisplay.transform) {
                if (!(( (currentScreen == ScreenStatus.Pages) ) && child.name == "NameSign")) {
                    Destroy(child.gameObject);
               }
            }
           
            
            

           if (currentScreen == ScreenStatus.Menu_Character) {

                    SpriteBuilder sbCharacterElement = ScreenElement.BuildSprite("CharacterElement", screenDisplay.transform).SetSprite(gm.spriteDB.GetCharacterSprites((GameChar)SelectedElement)[0]);
                    SpriteBuilder arrows= ScreenElement.BuildSprite("Arrows", screenDisplay.transform).SetSprite(gm.spriteDB.arrows).SetY(4).SetTransparent(true);

                
              
            }
           
            else if (currentScreen == ScreenStatus.Pages) {
                
                if(characterNameSign == null) {
                    string name = string.Format("{0}", pageCharacter.Name);
                    TextBoxBuilder nameBuilder = ScreenElement.BuildTextBox("NameSign", screenDisplay.transform, DFont.Big).SetText(name).SetSize(32, 7).SetPosition(32, 0);
                    nameBuilder.SetFitSizeToContent(true);
                    characterNameSign = nameBuilder.gameObject;
                    screenAnimation = StartCoroutine(AnimateName(nameBuilder));
                }
                int playerLevel = gm.logicMgr.GetPlayerLevel();
                

                MutableCharstats stats;
                             
                
                stats = pageCharacter.GetRegularStats();


                

                if (pageIndex == 0) {
                    SpriteBuilder sbCharacterElement = ScreenElement.BuildSprite("HP", screenDisplay.transform).SetSprite(gm.spriteDB.database_pages[3]).SetTransparent(true);
                    ScreenElement.BuildTextBox("Level", screenDisplay.transform, DFont.Regular)
                        .SetText(gm.logicMgr.GetPlayerLevel().ToString()).SetSize(15, 5).SetPosition(16, 8).SetAlignment(TextAnchor.UpperRight);
                    ScreenElement.BuildTextBox("HP", screenDisplay.transform, DFont.Regular)
                        .SetText(stats.HP.ToString()).SetSize(15, 5).SetPosition(16, 22).SetAlignment(TextAnchor.UpperRight);
                }
                 else if (pageIndex == 1) {
                     
                    ScreenElement.BuildTextBox("Spirit", screenDisplay.transform, DFont.Small)
                        .SetText("spirit").SetSize(10, 5).SetPosition(13, 9).SetAlignment(TextAnchor.UpperRight);
                    
                    ScreenElement.BuildTextBox("SP", screenDisplay.transform, DFont.Regular)
                        .SetText(stats.SP.ToString()).SetSize(15, 5).SetPosition(16, 22).SetAlignment(TextAnchor.UpperRight);
                }
                else if (pageIndex == 2) {
                    ScreenElement.BuildTextBox("Stamina", screenDisplay.transform, DFont.Small)
                        .SetText("stamina").SetSize(10, 5).SetPosition(22, 9).SetAlignment(TextAnchor.UpperRight);
                    
                    ScreenElement.BuildTextBox("ST", screenDisplay.transform, DFont.Regular)
                        .SetText(stats.ST.ToString()).SetSize(15, 5).SetPosition(16, 22).SetAlignment(TextAnchor.UpperRight);
                } else if (pageIndex == 3) {
                    ScreenElement.BuildTextBox("Skill", screenDisplay.transform, DFont.Small)
                        .SetText("skill").SetSize(10, 5).SetPosition(10, 9).SetAlignment(TextAnchor.UpperRight);
                    
                    ScreenElement.BuildTextBox("SK", screenDisplay.transform, DFont.Regular)
                        .SetText(stats.ST.ToString()).SetSize(15, 5).SetPosition(16, 22).SetAlignment(TextAnchor.UpperRight);
                }
            }
        }

        private void NavigateStageMenu(Direction dir) {
            if (dir == Direction.Left) menuIndex = menuIndex.CircularAdd(-1, 7);
            else menuIndex = menuIndex.CircularAdd(1, 7);
            DrawScreen();
        }

        private void OpenChartMenu() {
            availableElements = new List<int>();
            // availableElementscharacter = new Dictionary<int,int>();
            HashSet<int> elementsFound = new HashSet<int>(); //a list of elements found that will contain only 1 of each.
            // HashSet<int> elementsCharacterFound = new HashSet<int>(); //a list of elements found that will contain only 1 of each.
            int i=0;
            foreach(string d in galleryList) {
                elementsFound.Add((int)Database.GetCharacter(d).number);
                // availableElementscharacter.Add(i,(int)Database.GetDigimon(d).element);
                i++;

            }
           

            availableElements = elementsFound.ToList();
            // availableElements.Sort();
            
            elementIndex = 0;
            elementCharacterIndex=0;
            currentScreen = ScreenStatus.Menu_Character;
            DrawScreen();
        }
        private void NavigateChartMenu(Direction dir) {
            if (dir == Direction.Left) elementIndex = elementIndex.CircularAdd(-1, availableElements.Count - 1);
            else elementIndex = elementIndex.CircularAdd(1, availableElements.Count - 1);
            //  if (dir == Direction.Left) elementCharacterIndex = elementCharacterIndex.CircularAdd(-1, availableElementscharacter.Count - 1);
            // else elementCharacterIndex = elementCharacterIndex.CircularAdd(1, availableElementscharacter.Count - 1);
            int maxIndex = galleryList.Count - 1;
            if (dir == Direction.Left) galleryIndex = galleryIndex.CircularAdd(-1, maxIndex);
            else galleryIndex = galleryIndex.CircularAdd(1, maxIndex);
            DrawScreen();
        }
       
       

        private void OpenGallery() {
          
            string displayCharacter = galleryList[galleryIndex];
            pageCharacter = Database.GetCharacter(displayCharacter);
            //galleryIndex = 0;
            currentScreen = ScreenStatus.Gallery;
            DrawScreen();
        }

        private void NavigateGallery(Direction dir) {
            int maxIndex = galleryList.Count - 1;
            if (dir == Direction.Left) galleryIndex = galleryIndex.CircularAdd(-1, maxIndex);
            else galleryIndex = galleryIndex.CircularAdd(1, maxIndex);
            DrawScreen();
        }

        private void CloseGallery() {
            
            currentScreen = ScreenStatus.Menu_Character;
            
            DrawScreen();
        }

        private void OpenPages() {
            currentScreen = ScreenStatus.Pages;
            pageIndex = 0;

            string displayCharacter = galleryList[elementIndex];
            pageCharacter = Database.GetCharacter(displayCharacter);

            DrawScreen();
        }

        private void NavigatePages(Direction dir) {
            
            if (dir == Direction.Left) pageIndex = pageIndex.CircularAdd(-1, 3);
            else pageIndex = pageIndex.CircularAdd(1, 3);

            DrawScreen();
        }

        private void ClosePages() {
            currentScreen = ScreenStatus.Menu_Character;
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