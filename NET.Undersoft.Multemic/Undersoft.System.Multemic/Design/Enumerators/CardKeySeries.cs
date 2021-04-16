using System.Collections;
using System.Collections.Generic;
using System.Multemic.Basedeck;

/**********************************************
    Copyright (c) 2020 Undersoft

    System.Multemic.DeckSeries
        
    @author Darius Hanc                                                  
    @project NETStandard.Undersoft.SDK                                    
    @version 0.8.D (Feb 7, 2020)                                            
    @licence MIT                                     
 
*********************************************/
namespace System.Multemic
{      

    public class CardKeySeries<V> : IEnumerator<IUnique<V>>, IEnumerator
    {
        private IDeck<V> map;

        public CardKeySeries(Hashdeck<V> Map)
        {
            map = Map;
            Entry = map.First;
        }
        public CardKeySeries(Tetradeck<V> Map)
        {
            map = Map;
            Entry = map.First;
        }

        public Card<V> Entry;

        public long Key { get { return Entry.Key; } }
        public V Value { get { return Entry.Value; } }

        public object Current => Entry.Key;
       
        IUnique<V> IEnumerator<IUnique<V>>.Current  => Entry; 

        public bool MoveNext()
        {
            Entry = Entry.Next;
            if (Entry != null)
            {
                if (Entry.Removed)
                    return MoveNext();
                return true;
            }
            return false;
        }

        public void Reset()
        {
            Entry = map.First;
        }

        public void Dispose()
        {
            Entry = map.First;
        }
       
    }
}
