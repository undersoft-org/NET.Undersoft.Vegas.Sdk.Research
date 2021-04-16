using System.Runtime.InteropServices;
using System.Extract;
using System.Uniques;
using System.Collections.Generic;
using System.Multemic;

/******************************************************************
    Copyright (c) 2020 Undersoft

    System.Figures.Catalog64
    
    Implementation of CardBook abstract class
    using 64 bit hash code and long representation;  
        
    @author Darius Hanc                                                  
    @project Computing Wheel Advancements                                   
    @version 0.8.D (Feb 7, 2020)                                            
    @licence MIT                                             
 
 ******************************************************************/
namespace System.Instants
{  
    public class FigureSharedBook : SharedAlbum<IFigure>
    {
        public FigureSharedBook() : base(16, HashBits.bit64)
        {          
        }
        public FigureSharedBook(int _cardSize = 16) : base(_cardSize, HashBits.bit64)
        {
        }
        public FigureSharedBook(ICollection<IFigure> collections, int _cardSize = 16) : base(collections, _cardSize, HashBits.bit64)
        {
        }
        public FigureSharedBook(IEnumerable<IFigure> collections, int _cardSize = 16) : base(collections, _cardSize, HashBits.bit64)
        {
        }

        public override Card<IFigure> EmptyCard()
        {
            return new FigureCard();
        }

        public override Card<IFigure> NewCard(long key, IFigure value)
        {
            return new FigureCard(key, value);
        }
        public override Card<IFigure> NewCard(object key, IFigure value)
        {
           return new FigureCard(key, value);
        }
        public override Card<IFigure> NewCard(Card<IFigure> value)
        {
            return new FigureCard(value);
        }
        public override Card<IFigure> NewCard(IFigure value)
        {
            return new FigureCard(value);
        }

        public override Card<IFigure>[] EmptyCardTable(int size)
        {
            return new FigureCard[size];
        }

        public override Card<IFigure>[] EmptyCardList(int size)
        {
            cards = new FigureCard[size];
            return cards;
        }
      
        private FigureCard[] cards;
        public FigureCard[] Cards => cards;
    }
}
