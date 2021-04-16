using System.Collections;
using System.Collections.Generic;
using System.Multemic.Spectrum;

/*********************************************************************************
    Copyright (c) 2020 Undersoft

    System.Multemic.Spectrum
    
    Data structure based on van Emde Boas tree algorithm
    with constant maximum count of items in universum defined on the beginning. 
    Innovation is that all scopes have one global cluster registry (hash deck).
    Summary scopes(sigma scopes) are also in one global hash deck. 
    Another innovation is that tree branch ends with 4 leafs (values) instead of 2
    which are encoded in to global cluster registry. Achieved complexity of
    collection is Olog^log^(n/2). For dynamic resizing collection
    inside universum are used IDeck Collections assigned by interface 
    When Safe Thread parameter is set to true Board32 is assigned
    otherwise Deck32.        
        
    @authors PhD Radoslaw Rudek, Darius Hanc
    @project NETStandard.Undersoft.SDK                                    
    @version 0.8.D (Feb 7, 2020)                                            
    @licence MIT
 *********************************************************************************/
namespace System.Multemic
{
    public class Spectrum<V> : ISpectrum<V>
    {
        private int size;

        private IDeck<V> registry;
        private Scope root;

        private IDeck<Scope> scopes;

        private IDeck<Scope> sigmaScopes;

        private IList<vEBTreeLevel> levels;

        public int Count => registry.Count;

        public int Size { get; }

        public Spectrum() : this(int.MaxValue, false)
        { }
        public Spectrum(int size, bool safeThread)
        {
            Initialize(size);
        }

        private void BuildSigmaScopes(int range, byte level, byte nodeTypeIndex, int nodeCounter, int clusterSize)
        {
            int parentSqrt = ScopeValue.ParentSqrt(range);

            if (levels == null)
            {
                levels = new List<vEBTreeLevel>();
            }
            if (levels.Count <= level)
            {
                levels.Add(new vEBTreeLevel());
            }
            if (levels[level].Scopes == null)
            {
                levels[level].Scopes = new List<vEBTreeNode>();
                levels[level].Scopes.Add(new vEBTreeNode());
            }
            else
            {
                levels[level].Scopes.Add(new vEBTreeNode());
            }

            levels[level].Scopes[nodeTypeIndex].NodeCounter = nodeCounter;
            levels[level].Scopes[nodeTypeIndex].NodeSize = parentSqrt;

            if (parentSqrt > 4)
            {
                // sigmaNode
                BuildSigmaScopes(parentSqrt, (byte)(level + 1), (byte)(2 * nodeTypeIndex), nodeCounter, parentSqrt);
                // cluster
                BuildSigmaScopes(parentSqrt, (byte)(level + 1), (byte)(2 * nodeTypeIndex + 1), nodeCounter * parentSqrt, parentSqrt);
            }

        }

        private void CreateLevels(int range)
        {
            if (levels == null)
            {
                int parentSqrt = ScopeValue.ParentSqrt(size);
                BuildSigmaScopes(range, 0, 0, 1, parentSqrt);
            }

            int baseOffset = 0;
            for (int i = 1; i < levels.Count; i++)
            {
                levels[i].BaseOffset = baseOffset;
                for (int j = 0; j < levels[i].Scopes.Count - 1; j++) 
                {
                    levels[i].Scopes[j].IndexOffset = baseOffset;
                    baseOffset += levels[i].Scopes[j].NodeCounter * levels[i].Scopes[j].NodeSize;
                }
            }

        }

        public void Initialize(int range = 0)
        {
            scopes = new Deck64<Scope>();
            sigmaScopes = new Deck64<Scope>();

            if ((range == 0) || (range > int.MaxValue))
            {
                range = int.MaxValue;

                registry = new Deck64<V>();
            }
            else
            {
                registry = new Deck64<V>(range);
            }
            size = range;

            CreateLevels(range);   //create levels

            root = new ScopeValue(range, scopes, sigmaScopes, levels, 0, 0, 0);

                        
        }

        public int IndexMin
        {
            get { return root.IndexMin; }
        }

        public int IndexMax
        {
            get { return root.IndexMax; }
        }

        public bool Contains(int key)
        {
            return registry.ContainsKey(key);
        }
        
        public bool Add(int key, V obj)
        {
            if (registry.Add(key, obj))
            {
                root.Add(0, 1, 0, key);                
                return true;
            }
            return false;
        }

        public bool Remove(int key)
        {
            if (registry.TryRemove(key))
            {
                root.Remove(0, 1, 0, key);            
                return true;
            }
            return false;
        }

        public int Next(int key)
        {
            return root.Next(0, 1, 0, key);
        }

        public int Previous(int key)
        {
            return root.Previous(0, 1, 0, key);
        }

        public V Get(int key)
        {
            return registry.Get(key);
        }

        public bool Set(int key, V value)
        {
            return Add(key, value);
        }

        public IEnumerator<Card<V>> GetEnumerator()
        {
            return new SpectrumSeries<V>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new SpectrumSeries<V>(this);
        }

        public bool TestAdd(int key)
        {
            root.Add(0, 1, 0, key);
            return true;
        }
        public bool TestRemove(int key)
        {
            root.Remove(0, 1, 0, key);
            return true;
        }

        public bool TestContains(int key)
        {
            return root.Contains(0,1,0,key);
        }       
    }
}
