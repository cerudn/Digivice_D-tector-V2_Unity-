﻿using Kaisa.Digivice.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kaisa.Digivice {
    /// <summary>
    /// A class that manages the world, area, distance and other stuff related to the progress within the adventure.
    /// Any interaction with those parameters should be done through this class, rather than directly altering the SavedGame class.
    /// </summary>
    public class WorldManager {
        private GameManager gm;

        public WorldManager(GameManager gm) {
            this.gm = gm;
        }

        /// <summary>
        /// Sets up various parameters for the worlds for this game in the save file, such as randomization of some bosses.
        /// </summary>
        public void SetupWorlds() {
            //First assign the world-related arrays of the saved game an array of the proper size.
            SavedGame.SemibossGroupForEachMap = new int[Database.Worlds.Length];

            bool[][] completedAreas = new bool[Database.Worlds.Length][];
            for (int i = 0; i < completedAreas.Length; i++) {
                int areaCount = Database.Worlds[i].AreaCount;
                completedAreas[i] = new bool[areaCount];
            }
            SavedGame.CompletedAreas = completedAreas;

            //Then fill in some data:

            string[][] assignedBosses = new string[Database.Worlds.Length][];
            foreach(World w in Database.Worlds) {
                List<string> bosses = w.bosses.ToList();
                string playerSpirit = Database.PlayerSpirit[SavedGame.PlayerChar];

                int chosenSemibossGroup = Random.Range(0, w.SemibossGroupsCount ?? 0);

                if (w.semibossMode == SemibossMode.Fill) {
                    foreach(string semiboss in w.semibosses[chosenSemibossGroup]) {
                        bosses.Add(semiboss);
                    }
                }
                else {
                    SavedGame.SemibossGroupForEachMap[w.number] = chosenSemibossGroup;
                }

                if (w.removePlayer) {
                   
                    if (bosses.Remove(playerSpirit)) {
                        
                        
                        VisualDebug.WriteLine($"Removed {playerSpirit} from the list of bosses.");
                    }
                    else {
                        VisualDebug.WriteLine("No suitable Digimon was found to be removed from the first list of bosses. This should never happen.");
                    }
                }

                if(w.shuffle) {
                    bosses.Shuffle();
                   
                    // List<Digimon> spirits=
                    // Dictionary<string,string> spboos=new Dictionary<string,string> ();

                    // int i =0;
                    // foreach(string a in bosses){
                    //     if(spirits[i].name.ToLower()!=playerSpirit.ToLower() && a.ToLower()!="azulongmon" && a.ToLower()!="baihumon" && a.ToLower()!="ebonwumon" && a.ToLower()!="zhuqiaomon" ){
                    //     spboos.Add(a,spirits[i].name.ToLower());
                    //      i++;
                    //     }

                       
                    // }
                    // SavedGame.SpiritBoss=spboos;
                }

                assignedBosses[w.number] = bosses.ToArray();

                VisualDebug.WriteLine($"Finished parsing world {w.number}");
            }
            SavedGame.Bosses = assignedBosses;
            SavedGame.SpiritBoss= Database.GetSpiritsStartName();
            SavedGame.SpiritBoss.Remove(Database.PlayerSpirit[SavedGame.PlayerChar]);
            
        }

        /// <summary>
        /// Returns the bosses for a world, randomized as they are in the saved game.
        /// </summary>
        public string[] GetBossesForWorld(int world) {
            return SavedGame.Bosses[world];
        }

        /// <summary>
        /// Returns the current distance of the area the player is in.
        /// </summary>
        public int CurrentDistance {
            get => SavedGame.CurrentDistance;
            set => SavedGame.CurrentDistance = value;
        }
        /// <summary>
        /// Returns the current world the player is in.
        /// </summary>
        public int CurrentWorld {
            get => SavedGame.CurrentWorld;
            set => SavedGame.CurrentWorld = value;
        }
        /// <summary>
        /// Returns the map within the World the player is in.
        /// </summary>
        public int CurrentMap => Database.Worlds[CurrentWorld].areas[CurrentArea].map;
        /// <summary>
        /// Returns the current area the player is in.
        /// </summary>
        public int CurrentArea {
            get => SavedGame.CurrentArea;
            set => SavedGame.CurrentArea = value;
        }
        public World CurrentWorldData => Database.Worlds[CurrentWorld];

        /// <summary>
        /// Returns the total amount of steps taken by the player.
        /// </summary>
        public int TotalSteps => SavedGame.Steps;
        /// <summary>
        /// Returns true if the area is already completed.
        /// </summary>
        public bool GetAreaCompleted(int world, int area) => SavedGame.CompletedAreas[world][area];
        /// <summary>
        /// Sets whether the area is completed or not.
        /// </summary>
        public void SetAreaCompleted(int world, int area, bool completed) => SavedGame.CompletedAreas[world][area] = completed;

        public string GetBossOfCurrentArea() {
            return SavedGame.Bosses[CurrentWorld][CurrentArea];
        }

        /// <summary>
        /// Returns a list of all the areas in a world that haven't been completed yet.
        /// </summary>
        public List<int> GetUncompletedAreas(int world) {
            List<int> uncompletedAreas = new List<int>();
            for(int i = 0; i < Database.Worlds[world].AreaCount; i++) {
                if (!GetAreaCompleted(world, i)) uncompletedAreas.Add(i);
            }
            return uncompletedAreas;
        }
        public void setAllAreaUncomplete(int world){

             for(int i = 0; i < Database.Worlds[world].AreaCount; i++) {
                SavedGame.CompletedAreas[world][i]=false;
            }
        }
        /// <summary>
        /// Moves the player to a new map and area, and sets the current distance to the default distance for that map and area.
        /// </summary>
        public void MoveToArea(int world, int area) {
            MoveToArea(world, area, Database.Worlds[world].areas[area].distance);
        }
        /// <summary>
        /// Moves the player to a new map and area, and sets the current distance for that new area.
        /// </summary>
        public void MoveToArea(int map, int area, int distance) {
            SavedGame.CurrentWorld = map;
            SavedGame.CurrentArea = area;
            SavedGame.CurrentDistance = distance;
        }
        /// <summary>
        /// Takes a number of steps. This does not actually reduce distance. Returns true if the player takes enough steps to trigger an event, 
        /// and, in that case, resets the next event counter.
        /// </summary>
        /// <param name="steps"></param>
        public void TakeSteps(int steps = 1) {
           
            SavedGame.StepsToNextEvent -= steps;
            SavedGame.Steps += steps;

            if(SavedGame.StepsToNextEvent <= 0 && CurrentDistance > 1) {
                SavedGame.StepsToNextEvent = Random.Range(3, 6) * 100;
                SavedGame.SavedEvent = 1;
            }
        }

        //TODO: Test if semibosses work properly.
        /// <summary>
        /// Tries to reduce the distance by an amount, and outputs the actual distance reduced.
        /// Returns the actual distance reduced.
        /// </summary>
        /// <returns></returns>
        public int ReduceDistance(int distance) {
            int nextStop = 1; //Indicates the distance at which an event will trigger (and no extra distance will be removed).

            //TODO: REMOVE THIS.
            //Check semibosses for maps 4 and 8:
            /*if(SavedGame.CurrentWorld == 4) {
                int firstDeva = (int)((Distances[4][SavedGame.CurrentArea] / 4f) * 3);
                int secondDeva = (int)((Distances[4][SavedGame.CurrentArea] / 4f) * 2);
                int thirdDeva = (int)(Distances[4][SavedGame.CurrentArea] / 4f);
                if (SavedGame.CurrentDistance > firstDeva) nextStop = firstDeva + 1;
                else if (SavedGame.CurrentDistance > secondDeva) nextStop = secondDeva + 1;
                else if (SavedGame.CurrentDistance > thirdDeva) nextStop = thirdDeva + 1;
            }
            else if(SavedGame.CurrentWorld == 8) {
                int murmukusmon = (int)(Distances[8][SavedGame.CurrentArea] / 2f);
                if (SavedGame.CurrentDistance > murmukusmon) nextStop = murmukusmon + 1;
            }*/

            if (SavedGame.CurrentDistance - distance <= nextStop) {
                SavedGame.CurrentDistance = nextStop;
                //loadedGame.SavedEvent = 2; Do not trigger boss events unless the player shakes the device or presses B.
                return SavedGame.CurrentDistance - 1 - nextStop;
            }
            else {
                SavedGame.CurrentDistance -= distance;
                return distance;
            }
        }

        /// <summary>
        /// Forcibly reduces distance by an amount. This method will bypass boss encounters.
        /// </summary>
        /// <param name="distance"></param>
        public void ForceReduceDistance(int distance) => SavedGame.CurrentDistance -= distance;
        /// <summary>
        /// Increases the distance by an amount.
        /// </summary>
        /// <param name="distance"></param>
        public void IncreaseDistance(int distance) => SavedGame.CurrentDistance += distance;
    }
}