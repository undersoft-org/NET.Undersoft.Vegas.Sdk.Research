using System.Collections.Generic;
using System.Multemic;
using System.Uniques;
using System;
using System.Linq;
using Xunit;

namespace Undersoft.Tests.System.Multemic
{
    public class AlbumTestHelper
    {
        public IDeck<string> registry { get; set; }

        public IList<KeyValuePair<object, string>> stringKeyTestCollection { get; set; }
        public IList<KeyValuePair<object, string>> intKeyTestCollection { get; set; }
        public IList<KeyValuePair<object, string>> longKeyTestCollection { get; set; }
        public IList<KeyValuePair<object, string>> identifierKeyTestCollection { get; set; }     

        public AlbumTestHelper()
        {
            stringKeyTestCollection = PrepareTestListings.prepareStringKeyTestCollection();
            intKeyTestCollection = PrepareTestListings.prepareIntKeyTestCollection();
            longKeyTestCollection = PrepareTestListings.prepareLongKeyTestCollection();
            identifierKeyTestCollection = PrepareTestListings.prepareIdentifierKeyTestCollection();
        }
       
        public void Album_Integrated_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            Album_Add_Test(testCollection);
            Album_Count_Test(100000);
            Album_First_Test(testCollection[0].Value);
            Album_Last_Test(testCollection[99999].Value);
            Album_Get_Test(testCollection);
            Album_GetCard_Test(testCollection);
            Album_Remove_Test(testCollection);
            Album_Count_Test(70000);
            Album_Enqueue_Test(testCollection);
            Album_Count_Test(70005);
            Album_Dequeue_Test(testCollection);
            Album_Contains_Test(testCollection);
            Album_ContainsKey_Test(testCollection);
            Album_Put_Test(testCollection);
            Album_Count_Test(100000);           
            Album_Clear_Test();
            Album_Add_V_Test(testCollection);
            Album_Count_Test(100000);
            Album_Remove_V_Test(testCollection);
            Album_Count_Test(70000);
            Album_Put_V_Test(testCollection);
            Album_IndexOf_Test(testCollection);
            Album_GetByIndexer_Test(testCollection);
            Album_Count_Test(100000);
        }

        public void SharedAlbum_Integrated_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            Album_Add_Test(testCollection);
            Album_Get_Test(testCollection);
            Album_GetCard_Test(testCollection);
            Album_Remove_Test(testCollection);
            //Album_Enqueue_Test(testCollection);
            //Album_Dequeue_Test(testCollection);
            Album_Contains_Test(testCollection);
            Album_ContainsKey_Test(testCollection);
            Album_Put_Test(testCollection);
            Album_Add_V_Test(testCollection);
           // Album_Remove_V_Test(testCollection);
            Album_Put_V_Test(testCollection);
            Album_GetByIndexer_Test(testCollection);
        }

        private void Album_First_Test(string firstValue)
        {
            Assert.Equal(registry.Next(registry.First).Value, firstValue);
        }

        private void Album_Last_Test(string lastValue)
        {
            Assert.Equal(registry.Last.Value, lastValue);
        }

        private void Album_Count_Test(int count)
        {
            Assert.Equal(count, registry.Count);
        }

        private void Album_Add_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            foreach (var item in testCollection)
            {
                registry.Add(item.Key, item.Value);
            }           
        }

        private void Album_Add_V_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            foreach (var item in testCollection)
            {
                registry.Add(item.Value);
            }
        }

        private void Album_Clear_Test()
        {
            registry.Clear();
            Assert.Empty(registry);
        }

        private void Album_Contains_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            List<bool> items = new List<bool>();
            foreach (var item in testCollection)
            {
                if(registry.Contains(registry.NewCard(item.Key, item.Value)))
                    items.Add(true);
            }
            Assert.Equal(70000, items.Count);
        }

        private void Album_ContainsKey_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            List<bool> items = new List<bool>();
            foreach (var item in testCollection)
            {
                if (registry.ContainsKey(item.Key))
                    items.Add(true);
            }
            Assert.Equal(70000, items.Count);
        }

        private void Album_CopyTo_Test()
        {
            
        }

        private void Album_Dequeue_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            List<string> items = new List<string>();
            for (int i = 0; i < 5; i++)
            {
                string output = null;
                if (registry.TryDequeue(out output))
                    items.Add(output);
            }
            Assert.Equal(5, items.Count);
        }

        private void Album_Enqueue_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            List<bool> items = new List<bool>();
            foreach (var item in testCollection.Skip(70000).Take(5))
            {
                if(registry.Enqueue(item.Key, item.Value))                
                    items.Add(true);
            }
            Assert.Equal(5, items.Count);
        }

        private void Album_Get_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            List<string> items = new List<string>();
            foreach (var item in testCollection)
            {
                string r = registry.Get(item.Key);
                if (r != null)
                    items.Add(r);
                else
                    r = "wrong";
            }
            Assert.Equal(100000, items.Count);

        }

        private void Album_GetCard_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            List<Card<string>> items = new List<Card<string>>();
            foreach (var item in testCollection)
            {
                var r = registry.GetCard(item.Key);
                if (r != null)
                    items.Add(r);
            }
            Assert.Equal(100000, items.Count);
        }

        private void Album_Put_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            foreach (var item in testCollection)
            {
               registry.Put(item.Key, item.Value);
            }
        }

        private void Album_Put_V_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            foreach (var item in testCollection)
            {
                registry.Put(item.Value);
            }
        }

        private void Album_Remove_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            List<string> items = new List<string>();
            foreach (var item in testCollection.Skip(70000))
            {

                var r = registry.Remove(item.Key);
                if (r != null)
                    items.Add(r);
            }
            Assert.Equal(30000, items.Count);
        }

        private void Album_Remove_V_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            List<string> items = new List<string>();
            foreach (var item in testCollection.Skip(70000))
            {

                string r = registry.Remove(item.Value);
                items.Add(r);
            }
            Assert.Equal(30000, items.Count);
        }

        private void Album_IndexOf_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            List<int> items = new List<int>();          
            foreach (var item in testCollection.Take(5000))
            {
                int r = registry.IndexOf(item.Value);
                items.Add(r);
            }
           

        }

        private void Album_GetByIndexer_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            List<string> items = new List<string>();
            int i = 0;
            foreach (var item in testCollection)
            {
                string a = registry[i];
                string b = item.Value;
            }


        }

    }
}
