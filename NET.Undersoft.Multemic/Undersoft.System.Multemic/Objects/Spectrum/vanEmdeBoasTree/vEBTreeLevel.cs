using System.Collections.Generic;

/*********************************************************************************       
    Copyright (c) 2020 Undersoft

    System.Multemic.Spectrum.vEBTreeLevel
    
    @authors PhD Radoslaw Rudek, Darius Hanc
    @project NETStandard.Undersoft.SDK                                    
    @version 0.8.D (Feb 7, 2020)                                           
    @licence MIT
 **********************************************************************************/
namespace System.Multemic.Spectrum
{
    public class vEBTreeLevel
    {
        public byte Level { get; set; }
        public byte Count { get; set; }
        public int BaseOffset { get; set; }
        public IList<vEBTreeNode> Scopes { get; set; }
        public vEBTreeLevel()
        {
            Level = 0;
            BaseOffset = 0;
            Scopes = null;
        }
    }
}
