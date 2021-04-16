using System.Collections.Generic;

/*********************************************************************************       
    Copyright (c) 2020 Undersoft

    System.Multemic.Spectrum.ScopeValue
    
    @authors PhD Radoslaw Rudek, Darius Hanc
    @project NETStandard.Undersoft.SDK                                    
    @version 0.8.D (Feb 7, 2020)                                           
    @licence MIT
 **********************************************************************************/
namespace System.Multemic.Spectrum
{
    public class ScopeValue : Scope
    {
        private static int MINIMUM_UNIVERSE_SIZE_U4 = 4;
        public  static int NULL_KEY = -1;

        private int size;

        private int parentSqrt;
        private int childSqrt;

        private int min;
        private int max;

        Scope sigmaNode;
        private IDeck<Scope> sigmaScopes;  
        private IDeck<Scope> scopes;  

        private IList<vEBTreeLevel> levels;
        private byte nodeId;
        private byte level;
        private int  registryId;

        public ScopeValue(int Size, IDeck<Scope> Scopes, IDeck<Scope> SigmaScopes, IList<vEBTreeLevel> Levels, byte Level, byte NodeIndex, int DeckIndex)
        {
            this.min = NULL_KEY;
            this.max = NULL_KEY;
            this.sigmaNode = null;

            scopes = Scopes;
            sigmaScopes = SigmaScopes;
            levels = Levels;

            this.size = Size;
            parentSqrt = ParentSqrt(size);
            childSqrt = ChildSqrt(size);

            this.nodeId = NodeIndex;
            this.level = Level;
            this.registryId = DeckIndex;
        }

        public override int Size
        {
            get { return size; }
        }

        public override int IndexMin
        {
            get { return min; }
        }

        public override int IndexMax
        {
            get { return max; }
        }

        public override bool Contains(int x)
        {
            return Contains(0, 1, 0, x);
        }
        public override bool Contains(int offsetBase, int offsetFactor, int indexOffset, int x)
        {
            if (x == min || x == max)
            {
                return true;
            }
            else
            {
                if ((size == MINIMUM_UNIVERSE_SIZE_U4) || (x < min) || (x > max))
                {
                    return false;
                }
                else
                {
                    int scopesKey = offsetBase + indexOffset * parentSqrt + highest(x);
                    Scope scopesItem;
                    if (!scopes.TryGet(scopesKey, out scopesItem)) return false;
                    return scopesItem.Contains(offsetBase + offsetFactor * parentSqrt, offsetFactor * parentSqrt, indexOffset * parentSqrt + highest(x), lowest(x));
                }
            }
        }

        public override int Next(int offsetBase, int offsetFactor, int indexOffset, int x)
        {
            if (min != NULL_KEY && x < min)
            {
                return min;
            }

            Scope scopesItem;
            int x_highest = highest(x);
            int scopesKey;

            scopesKey = offsetBase + (indexOffset * parentSqrt) + x_highest;

            int maximumLow = NULL_KEY;
            if (scopes.TryGet(scopesKey, out scopesItem))
            {
                maximumLow = scopesItem.IndexMax;
            }

            if (maximumLow != NULL_KEY && lowest(x) < maximumLow)
            {
                int _offset = scopesItem.Next(offsetBase + (offsetFactor * parentSqrt), (offsetFactor * parentSqrt), (indexOffset * parentSqrt) + x_highest, lowest(x));

                return index(x_highest, _offset);
            }

            if (sigmaNode == null)
            {
                return NULL_KEY;
            }

            //======================//
            //from sigmaNode part 
            //======================//
            int successorCluster = sigmaNode.Next(x_highest); 

            if (successorCluster == NULL_KEY)
            {
                return NULL_KEY;
            }

            scopesKey = offsetBase + (indexOffset * parentSqrt) + successorCluster;

            scopes.TryGet(scopesKey, out scopesItem);
            int offset = scopesItem.IndexMin;

            return index(successorCluster, offset);
        }
        public override int Next(int x)
        {
            if (min != NULL_KEY && x < min) 
            {
                return min;
            }

            int x_highest = highest(x);

            //--- sigmaScopes common for all sigmaNode scopes ---->                
            Scope sigmaScopesItem;

            
            vEBTreeNode nodeTypeInfo = levels[level].Scopes[nodeId];
            int sigmaScopesKey =
                nodeTypeInfo.IndexOffset    
                + nodeTypeInfo.NodeSize 
                * registryId     
                + x_highest; 
            //<-----sigmaNode key --- global

            int maximumLow = NULL_KEY;

            if (sigmaScopes.TryGet(sigmaScopesKey, out sigmaScopesItem))
            {
                maximumLow = sigmaScopesItem.IndexMax;
            }

            if (maximumLow != NULL_KEY && lowest(x) < maximumLow)
            {
                int _offset = sigmaScopesItem.Next(lowest(x));
                return index(x_highest, _offset);
            }

            if (sigmaNode == null)
            {
                return NULL_KEY;
            }

            //======================//
            //from sigmaNode part 
            //======================//

            int successorCluster = sigmaNode.Next(x_highest);
            if (successorCluster == NULL_KEY)
            {
                return NULL_KEY;
            }

            //--- sigmaScopes common for all sigmaNode scopes ----> 
            sigmaScopesKey = nodeTypeInfo.IndexOffset + nodeTypeInfo.NodeSize * registryId + successorCluster;
            //<-----sigmaNode key --- global

            sigmaScopes.TryGet(sigmaScopesKey, out sigmaScopesItem);
            int offset = sigmaScopesItem.IndexMin;
            return index(successorCluster, offset);
        }    // used only for sigmaNode

        public override int Previous(int offsetBase, int offsetFactor, int indexOffset, int x)
        {
            if (max != NULL_KEY && x > max)
            {
                return max;
            }

            Scope scopesItem;
            int x_highest = highest(x);
            int scopesKey;

            scopesKey = offsetBase + (indexOffset * parentSqrt) + x_highest;

            int minimumLow = NULL_KEY;
            if (scopes.TryGet(scopesKey, out scopesItem))
            {
                minimumLow = scopesItem.IndexMin;
            }

            if (minimumLow != NULL_KEY && lowest(x) > minimumLow)
            {
                int _offset = scopesItem.Previous(offsetBase + (offsetFactor * parentSqrt), 
                                                (offsetFactor * parentSqrt), 
                                                (indexOffset * parentSqrt) + x_highest, 
                                                lowest(x));
                return index(x_highest, _offset);
            }

            if (sigmaNode == null)
            {
                return NULL_KEY;
            }

            //======================//
            //from sigmaNode part 
            //======================//
            int predecessorCluster = sigmaNode.Previous(x_highest);
            if (predecessorCluster == NULL_KEY)  
            {
                if (min != NULL_KEY && x > min)
                {
                    return min;
                }

                return NULL_KEY;
            }
            scopesKey = offsetBase + (indexOffset * parentSqrt) + predecessorCluster;

            scopes.TryGet(scopesKey, out scopesItem);
            int offset = scopesItem.IndexMax;
            return index(predecessorCluster, offset);
        }
        public override  int Previous(int x)
        {

            if (max != NULL_KEY && x > max)
            {
                return max;
            }

            int x_highest = highest(x);

            //--- sigmaScopes common for all sigmaNode scopes ---->                
            Scope sigmaScopesItem;

           
            vEBTreeNode nodeTypeInfo = levels[level].Scopes[nodeId];
            int sigmaScopesKey =
                nodeTypeInfo.IndexOffset   
                + nodeTypeInfo.NodeSize 
                * registryId       
                + x_highest; 
            //<-----sigmaNode key --- global

            int minimumLow = NULL_KEY;
            if (sigmaScopes.TryGet(sigmaScopesKey, out sigmaScopesItem))
            {
                minimumLow = sigmaScopesItem.IndexMin;
            }

            if (minimumLow != NULL_KEY && lowest(x) > minimumLow)
            {
                int _offset = sigmaScopesItem.Previous(lowest(x));
                return index(x_highest, _offset);
            }

            if (sigmaNode == null)
            {
                return NULL_KEY;
            }

            //======================//
            //from sigmaNode part 
            //======================//

            int predecessorCluster = sigmaNode.Previous(x_highest);
            if (predecessorCluster == NULL_KEY)     
            {
                if (min != NULL_KEY && x > min)
                {
                    return min;
                }

                return NULL_KEY;
            }

            //--- sigmaScopes common for all sigmaNode scopes ----> 
            sigmaScopesKey = nodeTypeInfo.IndexOffset + nodeTypeInfo.NodeSize * registryId + predecessorCluster; 
            //<-----sigmaNode key --- global

            sigmaScopes.TryGet(sigmaScopesKey, out sigmaScopesItem);
            int offset = sigmaScopesItem.IndexMax;
            return index(predecessorCluster, offset);
        }

        public override void Add(int offsetBase, int offsetFactor, int indexOffset, int x)
        {
            if ((min == x) || (max == x))
            {
                return;
            }

            if (min == NULL_KEY)
            {
                FirstAdd(offsetBase + offsetFactor * parentSqrt, offsetFactor * parentSqrt, indexOffset * parentSqrt + highest(x), x);
                return;
            }

            if (x < min)
            {
                int tmp = x;
                x = min;
                min = tmp;
            }

            if (size != MINIMUM_UNIVERSE_SIZE_U4)
            {
                Scope scopesItem;
                int x_highest = highest(x);

                int scopesKey = offsetBase + indexOffset * parentSqrt + x_highest;

                if (!scopes.TryGet(scopesKey, out scopesItem))
                {
                    if (parentSqrt == MINIMUM_UNIVERSE_SIZE_U4)
                    {
                        if (sigmaNode == null)    //sigmaNode of the current level (u>4, e.g., u=16)
                        {
                            sigmaNode = new TetraValue(-1);
                            sigmaNode.FirstAdd(x_highest);
                        }
                        else
                        {
                            sigmaNode.Add(x_highest); //tutaj zrobic else
                        }
                        scopesItem = new TetraValue(-1);
                        scopes.Add(scopesKey, scopesItem);
                        scopesItem.FirstAdd(lowest(x));
                    }
                    else //create new node (add next level)
                    {
                        if (sigmaNode == null)
                        {
                            sigmaNode = new ScopeValue(parentSqrt, scopes, sigmaScopes, levels, (byte)(level + 1), (byte)(2 * nodeId), registryId);
                            sigmaNode.FirstAdd(x_highest);
                        }
                        else
                        {
                            sigmaNode.Add(x_highest); //tutaj zrobic else
                        }

                        scopesItem = new ScopeValue(parentSqrt, scopes, sigmaScopes, levels, (byte)(level + 1), (byte)(2 * nodeId + 1),
                            registryId * levels[level].Scopes[nodeId].NodeSize + x_highest);
                        scopes.Add(scopesKey, scopesItem);
                        scopesItem.FirstAdd(offsetBase + offsetFactor * parentSqrt, offsetFactor * parentSqrt, indexOffset * parentSqrt + x_highest, lowest(x));
                    }
                }
                else
                {
                    scopesItem.Add(offsetBase + offsetFactor * parentSqrt, offsetFactor * parentSqrt, indexOffset * parentSqrt + x_highest, lowest(x));
                }
            }

            if (max < x)
            {
                max = x;
            }
        }
        public override void Add(int x)
        {
            if ((min == x) || (max == x))
            {
                return;
            }

            if (x < min)
            {
                int tmp = x;
                x = min;
                min = tmp;
            }

            if (size != MINIMUM_UNIVERSE_SIZE_U4)
            {
                int x_highest = highest(x);

                //--- sigmaScopes common for all sigmaNode scopes ---->                
                Scope sigmaScopesItem;
                
                int sigmaScopesKey =
                    levels[level].Scopes[nodeId].IndexOffset 
                    + levels[level].Scopes[nodeId].NodeSize 
                    * registryId      
                    + x_highest;
                              //<-----sigmaNode key --- global

             
                if (!sigmaScopes.TryGet(sigmaScopesKey, out sigmaScopesItem)) 
                {
                    if (parentSqrt == MINIMUM_UNIVERSE_SIZE_U4)
                    {
                        if (sigmaNode == null)
                        {
                            sigmaNode = new TetraValue(-1);
                            sigmaNode.FirstAdd(x_highest);
                        }
                        else
                        {
                            sigmaNode.Add(x_highest);   
                        }

                        //--- sigmaScopes common for all sigmaNode scopes ---->
                        sigmaScopesItem = new TetraValue(-1);
                        sigmaScopes.Add(sigmaScopesKey, sigmaScopesItem);
                        sigmaScopesItem.FirstAdd(lowest(x));
                        //<-----sigmaNode key --- global
                    }
                    else //create new branch
                    {
                        if (sigmaNode == null)
                        {
                            sigmaNode = new ScopeValue(parentSqrt, scopes, sigmaScopes, levels, (byte)(level + 1), (byte)(2 * nodeId), registryId);
                            sigmaNode.FirstAdd(x_highest);
                        }
                        else
                        {
                            sigmaNode.Add(x_highest);
                        }

                        //--- sigmaScopes common for all sigmaNode scopes ---->
                        sigmaScopesItem = new ScopeValue(parentSqrt, scopes, sigmaScopes, levels, (byte)(level + 1), (byte)(2 * nodeId + 1),
                            registryId * levels[level].Scopes[nodeId].NodeSize + x_highest);
                        sigmaScopes.Add(sigmaScopesKey, sigmaScopesItem);
                        sigmaScopesItem.FirstAdd(lowest(x));
                        //<-----sigmaNode key --- global

                    }
                }
                else
                {
                    //--- sigmaScopes common for all sigmaNode scopes ---->
                    sigmaScopesItem.Add(lowest(x));
                    //<-----sigmaNode key --- global
                }
            }

            if (max < x)
            {
                max = x;
            }
        }   

        public override void FirstAdd(int offsetBase, int offsetFactor, int indexOffset, int x)
        {
            min = x;
            max = x;
        }
        public override void FirstAdd(int x)
        {
            min = x;
            max = x;
        }

        public override bool Remove(int offsetBase, int offsetFactor, int indexOffset, int x)
        {
            if (min == max)
            {
                if (min != x) return false; 
                min = NULL_KEY;
                max = NULL_KEY;
                sigmaNode = null;
                return true;
            }

            Scope scopesItem;
            int x_highest;
            int scopesKey;

            if (min == x)   
            {
                int firstCluster = sigmaNode.IndexMin;

                scopesKey = offsetBase + (indexOffset * parentSqrt) + firstCluster;
                scopes.TryGet(scopesKey, out scopesItem);
                x = index(firstCluster, scopesItem.IndexMin); 
                
                min = x;

            }

            x_highest = highest(x);
            scopesKey = offsetBase + (indexOffset * parentSqrt) + x_highest;

            if (!scopes.TryGet(scopesKey, out scopesItem)) return false;    

            scopesItem.Remove(offsetBase + (offsetFactor * parentSqrt), (offsetFactor * parentSqrt), (indexOffset * parentSqrt) + x_highest, lowest(x));         

            if (scopesItem.IndexMin == NULL_KEY) 
            {                                           
                scopes.Remove(scopesKey);

                sigmaNode.Remove(highest(x));   

                if (x == max)  
                {
                    int sigmaNodeMaximum = sigmaNode.IndexMax; 

                    if (sigmaNodeMaximum == NULL_KEY) 
                    {
                        max = min;
                        sigmaNode = null;
                    }
                    else 
                    {
                        scopesKey = offsetBase + (indexOffset * parentSqrt) + sigmaNodeMaximum;
                        scopes.TryGet(scopesKey, out scopesItem);

                        int maximumKey = scopesItem.IndexMax;
                        max = index(sigmaNodeMaximum, maximumKey);
                    }
                }
            }
            else if (x == max)     
            {           
                scopesKey = offsetBase + (indexOffset * parentSqrt) + highest(x); 
                scopes.TryGet(scopesKey, out scopesItem);
                int maximumKey = scopesItem.IndexMax;

                max = index(highest(x), maximumKey); 
            }
            return true;
        }

        public override bool Remove(int x)
        {
            if (min == max)
            {
                if (min != x) return true;  
                min = NULL_KEY;
                max = NULL_KEY;
                sigmaNode = null;           
                return true;
            }

            //--- sigmaScopes common for all sigmaNode scopes ---->                            
            Scope sigmaScopesItem;
            vEBTreeNode nodeTypeInfo = levels[level].Scopes[nodeId]; ;
            int sigmaScopesKey;
            //<-----sigmaNode key --- global
            int x_highest;
       
            if (min == x)
            {
                int first = sigmaNode.IndexMin;

                //--- sigmaScopes common for all sigmaNode scopes ---->                            
                sigmaScopesKey = nodeTypeInfo.IndexOffset + nodeTypeInfo.NodeSize * registryId + first;
                sigmaScopes.TryGet(sigmaScopesKey, out sigmaScopesItem);   
                x = index(first, sigmaScopesItem.IndexMin); 
                //<-----sigmaNode key --- global
                min = x;             
            }

            x_highest = highest(x);
            //--- sigmaScopes common for all sigmaNode scopes ---->
            sigmaScopesKey = nodeTypeInfo.IndexOffset + nodeTypeInfo.NodeSize * registryId + x_highest;
            if (!sigmaScopes.TryGet(sigmaScopesKey, out sigmaScopesItem)) return false;   
            sigmaScopesItem.Remove(lowest(x));
            //<-----sigmaNode key --- global

            if (sigmaScopesItem.IndexMin == NULL_KEY)   
            {
                //--- sigmaScopes common for all sigmaNode scopes ---->
                sigmaScopes.Remove(sigmaScopesKey);
                //<-----sigmaNode key --- global

                sigmaNode.Remove(highest(x));   

                if (x == max) 
                {
                    int sigmaNodeMaximum = sigmaNode.IndexMax; 

                    if (sigmaNodeMaximum == NULL_KEY)
                    {
                        max = min;
                        sigmaNode = null; 
                    }
                    else  
                    {

                        //--- sigmaScopes common for all sigmaNode scopes ---->
                        sigmaScopesKey = nodeTypeInfo.IndexOffset + nodeTypeInfo.NodeSize * registryId + sigmaNodeMaximum;
                        sigmaScopes.TryGet(sigmaScopesKey, out sigmaScopesItem);
                        int maximumKey = sigmaScopesItem.IndexMax;
                        //<-----sigmaNode key --- global

                        max = index(sigmaNodeMaximum, maximumKey);
                    }
                }
            }
            else if (x == max)     
            {
                //--- sigmaScopes common for all sigmaNode scopes ---->
                sigmaScopesKey = nodeTypeInfo.IndexOffset + nodeTypeInfo.NodeSize * registryId + highest(x);
                sigmaScopes.TryGet(sigmaScopesKey, out sigmaScopesItem);
                int maximumKey = sigmaScopesItem.IndexMax;
                //<-----sigmaNode key --- global

                max = index(highest(x), maximumKey); 
            }
            return true;
        }    

        private int highest(int x)
        {
            return x / childSqrt;
        }

        private int lowest(int x)
        {
            return x & (childSqrt - 1);
        }

        private int index(int x, int y)
        {
            return (x * childSqrt + y);
        }

        public static int ParentSqrt(int number)
        {
            double exponent = Math.Ceiling(Math.Log(number) / Math.Log(2.0) / 2.0);
            return (int)Math.Pow(2.0, exponent);
        }
        public static int ChildSqrt(int number)
        {
            double exponent = Math.Floor(Math.Log(number) / Math.Log(2.0) / 2.0);
            return (int)Math.Pow(2.0, exponent);
        }
    }
}