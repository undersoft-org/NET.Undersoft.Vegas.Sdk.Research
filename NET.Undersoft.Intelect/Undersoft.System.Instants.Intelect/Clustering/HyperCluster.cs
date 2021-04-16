//-----------------------------------------------------------------------
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Instants.Intelect.Clustering
{

    public class HyperCluster
    {
        private bool ValidHyperClusterItemList;

        public double[] HyperClusterVector { get; set; }

        public List<Cluster> ClusterList { get; set; }

        public List<FeatureItem> HyperClusterItemList { get; set; } //redundant - analyse to delete it // should be private to have access by Get

        public double[] HyperClusterVectorSummary { get; set; }

        public HyperCluster(Cluster cluster)
        {
            //resulting values (tables) should be copied
            HyperClusterVector = new double[cluster.ClusterVector.Length];
            Array.Copy(cluster.ClusterVector, HyperClusterVector, cluster.ClusterVector.Length);
            HyperClusterVectorSummary = new double[cluster.ClusterVectorSummary.Length];
            Array.Copy(cluster.ClusterVectorSummary, HyperClusterVectorSummary, cluster.ClusterVectorSummary.Length);
            ClusterList = new List<Cluster>();
            ClusterList.Add(cluster);

            //ItemList in HyperCluster = ClusterList[i].ItemList[j], i=0,..., ; j = 0, ...
            //redundant - try reorganize code to remove it
            //HyperClusterItemList = new List<FeatureItem>();
            //for(int i=0; i< cluster.ClusterItemList.Count; i++)
            //{
            //    HyperClusterItemList.Add(cluster.ClusterItemList[i]);
            //}
            //ValidHyperClusterItemList = true;
        }

        public bool RemoveClusterFromHyperCluster(Cluster cluster)
        {
            if (ClusterList.Remove(cluster) == true)
            {
                if (ClusterList.Count > 0)
                {
                    AdaptiveIntersect.CalculateClusterIntersection(ClusterList, HyperClusterVector);
                    AdaptiveIntersect.CalculateClusterSummary(ClusterList, HyperClusterVectorSummary);

                    //TODO: redundant analyse to remove it
                    // nie ma senzu sa kazdym razem tworzyc listy, tylko wtedy gdy bedzie do niej potrzebny dostep
                    //HyperClusterItemList.Clear();
                    //for(int i=0; i< ClusterList.Count; i++)
                    //{
                    //    for(int j = 0; j < ClusterList[i].ClusterItemList.Count; j++)
                    //    {
                    //        HyperClusterItemList.Add(ClusterList[i].ClusterItemList[j]);
                    //    }
                    //}
                }
                ValidHyperClusterItemList = false;
            }
            return ClusterList.Count > 0;
        }

        public void AddClusterToHyperCluster(Cluster cluster)
        {
            ClusterList.Add(cluster);
            AdaptiveIntersect.UpdateClusterIntersectionByLast(ClusterList, HyperClusterVector);
            AdaptiveIntersect.UpdateClusterSummaryByLast(ClusterList, HyperClusterVectorSummary);

            ValidHyperClusterItemList = false;
            //TODO: rendutant analyse to remove it            
            //for (int i = 0; i < cluster.ClusterItemList.Count; i++)
            //{                
            //    HyperClusterItemList.Add(cluster.ClusterItemList[i]);
            //}
        }

        public List<FeatureItem> GetHyperClusterItemList()
        {
            //TODO: valid nie ma sensu, jezeli zewnetrzna metoda dodala do cluster, ktory juz jest wewnatrz hypercluster
            //chyba, zeby dac zmienna obiektu, ktora przy jakiejkolwiek zamienie o tym informuje
            //pytanie tylko czy to ma sens
            //if (ValidHyperClusterItemList == false)
            //{
            List<FeatureItem> updatedItemList = new List<FeatureItem>();

            for (int i = 0; i < ClusterList.Count; i++)
            {
                for (int j = 0; j < ClusterList[i].ClusterItemList.Count; j++)
                {
                    updatedItemList.Add(ClusterList[i].ClusterItemList[j]);
                }
            }
            HyperClusterItemList = updatedItemList;
            //ValidHyperClusterItemList = true;
            //}

            return HyperClusterItemList;
        }

    }
}


