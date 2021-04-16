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
    public class CardSeries<V> : IEnumerator<Card<V>>, IEnumerator<V>, IEnumerator
    {
        private IDeck<V> map;

        public CardSeries(Hashdeck<V> Map)
        {
            map = Map;        
            Entry = map.First;
        }
        public CardSeries(Tetradeck<V> Map)
        {
            map = Map;
            Entry = map.First;
        }

        public Card<V> Entry;

        public long Key { get { return Entry.Key; } }
        public V Value { get { return Entry.Value; } }

        public object Current => Entry.Value;

        Card<V> IEnumerator<Card<V>>.Current => Entry;

        V IEnumerator<V>.Current => Entry.Value;

        public bool MoveNext()
        {
            Entry = map.Next(Entry);
            if (Entry != null)
            {
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
