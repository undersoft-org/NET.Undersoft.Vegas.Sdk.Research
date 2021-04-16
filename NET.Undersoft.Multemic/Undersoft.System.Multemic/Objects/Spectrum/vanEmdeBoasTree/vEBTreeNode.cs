/*********************************************************************************       
    Copyright (c) 2020 Undersoft

    System.Multemic.Spectrum.vEBTreeNode
    
    @authors PhD Radoslaw Rudek, Darius Hanc
    @project NETStandard.Undersoft.SDK                                    
    @version 0.8.D (Feb 7, 2020)                                           
    @licence MIT
 **********************************************************************************/
namespace System.Multemic.Spectrum
{
    public class vEBTreeNode
    {        
        public int IndexOffset { get; set; }        //start index in the global cluster of this type
        public int NodeCounter { get; set; }        //number of elements of given type
        public int NodeSize { get; set; }        // cluster size of the given type of a node      
    }
}
