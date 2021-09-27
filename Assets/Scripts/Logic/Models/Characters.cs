using UnityEngine;

namespace Kaisa.Digivice {
    public class Characters {


private string name;

private Element element;

private Charstats stats;
public string [] spirits {get;  private set;}
private bool disabled;

public int order {get;  set;}
public int number {get;  private set;}

    public Characters(string name, Element element,Charstats stats, string [] spirits, bool disabled,int order,int number)
        {
            this.name=name.ToLower();
            this.element=element;
            this.stats=stats;
            this.spirits=spirits;
            this.disabled=disabled;
            this.order=order;
            this.number=number;
            




        }



    public MutableCharstats getCharStatsbyLevel(int playerLevel){

         int HP=0;
         int SP=0;
         int ST=0;
         int SK=0;

        return new MutableCharstats(HP, SP, ST, SK);

      }

    public string Name{
       get{
           return this.name;
       }
    }

    public Element elements{
        get{
            return this.element;
        }
    }
   

    
    

public bool Available{

    get {
        return this.disabled;
    }
}
public MutableCharstats GetRegularStats(){

       return new MutableCharstats(stats.HP, stats.SP, stats.ST, stats.SK);

      }


    }






 public class Charstats {
        public readonly int HP;
        public readonly int SP;
        public readonly int ST;
        public readonly int SK;

        public Charstats(int HP, int SP, int ST, int SK) {
            this.HP = HP;
            this.SP = SP;
            this.ST = ST;
            this.SK = SK;
        }

        public override string ToString() {
            return $"HP: {HP}, EN: {SP}, CR: {ST}, AB: {SK}.";
        }
    }
    public class MutableCharstats {
        public readonly int HP;
        public readonly int SP;
        public readonly int ST;
        public readonly int SK;

        

        public MutableCharstats(int HP, int SP, int ST, int SK) {
            this.HP = HP;
            this.SP = SP;
            this.ST = ST;
            this.SK = SK;
        }

        public override string ToString() {
            return $"HP: {HP}, EN: {SP}, CR: {ST}, AB: {SK}.";
        }
        }
  
 
}