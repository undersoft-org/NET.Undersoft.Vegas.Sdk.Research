/*********************************************************************************
    Copyright (c) 2020 Undersoft

    System.Multemic.Spectrum.TetraValue

    Tree branch ending node with 4 leafs instead of 2
    which are encoded in to global cluster registry. 
    Achieved complexity of collection is Olog^log^(n/2).
    
    @authors PhD Radoslaw Rudek, Darius Hanc
    @project NETStandard.Undersoft.SDK                                    
    @version 0.8.D (Feb 7, 2020)                                            
    @licence MIT
 *********************************************************************************/
namespace System.Multemic.Spectrum
{   
    public class TetraValue : Scope
    {
        public int[] xValue;

        public TetraValue(int null_key)
        {
            xValue = new int[] { null_key, null_key, null_key, null_key };
        }

        public override int Size
        {
            get { return 4; }
        }

        public override int IndexMin
        {
            get { return xValue[0]; }
        }

        public override int IndexMax
        {
            get { return xValue[3]; }
        }

        public override bool Contains(int x)
        {
            if (xValue[x] == x)
            {
                return true;
            }

            return false;
        }
        public override bool Contains(int offsetBase, int offsetFactor, int indexOffset, int x)
        {
            if (xValue[x] == x)
            {
                return true;
            }

            return false;
        }

        public override int Next(int offsetBase, int offsetFactor, int indexOffset, int x)        
        {                        
//            if (x >= xValue[3]) return -1; //x == max or max == null 
//            if (xValue[x + 1] > x) return xValue[x + 1];
//            if (xValue[x + 2] > x) return xValue[x + 2];           
//            return xValue[3];

            if (x >= xValue[3]) return -1; //x == max or max == null 
            if (xValue[x + 1] != -1) return xValue[x + 1];    //0, 1, 2
            if (xValue[x + 2] != -1) return xValue[x + 2];    //0, 1
            return xValue[3]; //1


        }
        public override int Next(int x)
        {
            //dziala
            //if (x >= xValue[3]) return -1; //x == max or max == null 
            //if (xValue[x + 1] > x) return xValue[x + 1];
            //if (xValue[x + 2] > x) return xValue[x + 2];

            if (x >= xValue[3]) return -1; //x == max or max == null 
            if (xValue[x + 1] != -1) return xValue[x + 1];    //0, 1, 2
            if (xValue[x + 2] != -1) return xValue[x + 2];    //0, 1
            return xValue[3]; //1
        }

        public override int Previous(int offsetBase, int offsetFactor, int indexOffset, int x)
        {
            if (x <= xValue[0]) return -1; //x == min or min == null
            if (xValue[x - 1] != -1) return xValue[x - 1];    //1, 2, 3
            if (xValue[x - 2] != -1) return xValue[x - 2];    //2, 3
            return xValue[0];   //3
        }
        public override int Previous(int x)
        {
            if (x <= xValue[0]) return -1; //x == min or min == null
            if (xValue[x - 1] != -1) return xValue[x - 1];
            if (xValue[x - 2] != -1) return xValue[x - 2];            
            return xValue[0];
        }

        //if leaf is sigmaNode u=2 => //sigmaNode can have min=max=null; min=max=0; min=max=1; and min=0, max=1;   
        //if leaf is a sigmaNodeCluster item, it is always: min=max {null or 1}
        // min  | 0 | 1 | 2 | 3 | 0 | 0 | .. | 0 | .. | 0 |
        // mid0 | - | 1 | - | - | 1 | - | .. | - | .. | 1 |
        // mid1 | - | - | 2 | - | - | 2 | .. | 2 | .. | 2 |
        // max  | 0 | 1 | 2 | 3 | 1 | 2 | .. | 3 | .. | 3 |
        //
        public override void Add(int x)
        {
            if (xValue[x] == x)
            {
                return;
            }

            xValue[x] = x;
            if (x > xValue[3])
            {
                xValue[3] = x;
                return;
            }

            if (x < xValue[0])
            {
                xValue[0] = x;
                return;
            }
        }
        public override void Add(int offsetBase, int offsetFactor, int indexOffset, int x)
        {
            if (xValue[x] == x)
            {
                return;
            }

            xValue[x] = x;
            if (x > xValue[3])
            {
                xValue[3] = x;
                return;

            }
            if (x < xValue[0])
            {
                xValue[0] = x;
                return;
            }
        }

        public override void FirstAdd(int offsetBase, int offsetFactor, int indexOffset, int x)
        {
            xValue[0] = x;
            xValue[3] = x;
            xValue[x] = x;
        }
        public override void FirstAdd(int x) 
        {
            xValue[0] = x;
            xValue[3] = x;
            xValue[x] = x;
        }

        public override bool Remove(int offsetBase, int offsetFactor, int indexOffset, int x)
        {
            if (xValue[x] != x) return false; //(27 case)

            //update if x is min
            if (xValue[0] == x)
            {
                xValue[x] = -1;
                if (xValue[1] != -1) { xValue[0] = xValue[1]; return true; }
                if (xValue[2] != -1) { xValue[0] = xValue[2]; return true; }
                if (xValue[3] != -1 && xValue[3] != x) { xValue[0] = xValue[3]; return true; } //more often than min==max
                xValue[0] = -1;
                xValue[3] = -1;
                return true;
            }
            //update if x is max
            if (xValue[3] == x)
            {
                xValue[x] = -1;
                if (xValue[2] != -1) { xValue[3] = xValue[2]; return true; }
                if (xValue[1] != -1) { xValue[3] = xValue[1]; return true; }
                //knowing that xValue[0] != x and xValue[0] != -1, then
                xValue[3] = xValue[0];
                return true;
            }
            xValue[x] = -1;
            return true;    
        }
        public override bool Remove(int x)
        {
            if (xValue[x] != x) return false; //(27 case)

            //x is stored
            //if (xValue[0] == xValue[3]) { xValue[0] = -1; xValue[3] = -1; return true; }
           
            //update if x is min
            if (xValue[0] == x)
            {
                xValue[x] = -1;
                if (xValue[1] != -1) { xValue[0] = xValue[1]; return true; }
                if (xValue[2] != -1) { xValue[0] = xValue[2]; return true; }
                if (xValue[3] != -1 && xValue[3]!=x) { xValue[0] = xValue[3]; return true; } //more often than min==max
                xValue[0] = -1;
                xValue[3] = -1;
                return true;
            }
            //update if x is max
            if (xValue[3] == x)
            {
                xValue[x] = -1;
                if (xValue[2] != -1) { xValue[3] = xValue[2]; return true; }
                if (xValue[1] != -1) { xValue[3] = xValue[1]; return true; }
                //knowing that xValue[0] != x and xValue[0] != -1, then
                xValue[3] = xValue[0];
                return true;
            }
            xValue[x] = -1;
            return true;
        }  

   }   
  
}
