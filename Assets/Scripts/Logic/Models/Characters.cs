using UnityEngine;

namespace Kaisa.Digivice {
public class Characters {


private string name;

private Element element;

private Charstats stats;
private Charstats lastStat;
public string [] spirits {get;  private set;}
private bool disabled;

public int order {get;  set;}
public int number {get;  private set;}

    public Characters(string name, Element element,Charstats stats, Charstats lastStat,string [] spirits, bool disabled,int order,int number)
        {
            this.name=name.ToLower();
            this.element=element;
            this.stats=stats;
            this.lastStat=lastStat;
            this.spirits=spirits;
            this.disabled=disabled;
            this.order=order;
            this.number=number;
            




        }



    public MutableCharstats getCharStatsbyLevel(int playerLevel){

        int [,] actual= LogicManager.randStat();
        int [,] anterior = LogicManager.beforeRanstat();

        int HP1=((playerLevel*2+stats.HP)-actual[this.number,0])<=lastStat.HP ? (playerLevel*2+stats.HP)-actual[this.number,0]: lastStat.HP;
        int SP1=(playerLevel*2+stats.SP)-actual[this.number,1]<=lastStat.SP ? (playerLevel*2+stats.SP)-actual[this.number,1]: lastStat.SP;
        int ST1=(playerLevel*2+stats.ST)-actual[this.number,2]<=lastStat.ST ? (playerLevel*2+stats.ST)-actual[this.number,2]: lastStat.ST;
        int SK1=(playerLevel*2+stats.SK)-actual[this.number,3] <=lastStat.SK ? (playerLevel*2+stats.SK)-actual[this.number,3]: lastStat.SK;

        if(playerLevel!=1 && playerLevel!=2){
        int  HP=((HP1>((playerLevel-1)*2+stats.HP)-anterior[this.number,0]) || HP1==lastStat.HP) ? HP1:((playerLevel-1)*2+stats.HP)-anterior[this.number,0] ;
        int  SP=SP1>((playerLevel-1)*2+stats.SP)-anterior[this.number,1] || SP1 == lastStat.SP? SP1:((playerLevel-1)*2+stats.SP)-anterior[this.number,1] ;
        int  ST=ST1>((playerLevel-1)*2+stats.ST)-anterior[this.number,2] || ST1== lastStat.ST ? ST1:((playerLevel-1)*2+stats.ST)-anterior[this.number,2] ;
        int  SK=SK1>((playerLevel-1)*2+stats.SK)-anterior[this.number,3] || SK1 == lastStat.SK ? SK1:((playerLevel-1)*2+stats.SK)-anterior[this.number,3] ;

        return new MutableCharstats(HP, SP, ST, SK);
        }
        else if (playerLevel==2){
         int HP=HP1>stats.HP ? HP1 :stats.HP ;
         int SP=SP1>stats.SP ? SP1:stats.SP ;
         int ST=ST1>stats.ST ? ST1:stats.ST ;
         int SK=SK1>stats.SK ? SK1:stats.SK;

         return new MutableCharstats(HP, SP, ST, SK);

            }else{return GetRegularStats();
        }

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
  
  public class MutableSpiritStats {
        public readonly int HP;
        public readonly int SP;
        public readonly int ST;
        public readonly int SK;

        

        public MutableSpiritStats(int HP, int SP, int ST, int SK) {
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