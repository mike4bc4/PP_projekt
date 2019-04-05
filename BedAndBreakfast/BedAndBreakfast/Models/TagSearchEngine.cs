using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BedAndBreakfast.Models
{


    public class TagSearchEngine
    {
        public struct Item {
            public Item(int id, string value) {
                this.id = id;
                this.value = value;
            }
            public int id;
            public string value;
        }

        public struct Relation {
            public Relation(int itemA_id, int itemB_id) {
                this.itemA_id = itemA_id;
                this.itemB_id = itemB_id;
            }
            public int itemA_id;
            public int itemB_id;
        };


        private string query;
        private List<string> queryTags;
        private List<Item> browsedTags;
        private List<Item> browsedItems;
        private List<Relation> itemTagRelation;

        private Dictionary<int, int> itemsScore;

        public TagSearchEngine(string query, List<Item> browsedTags, List<Item> browsedItems, List<Relation> itemTagRelation) {
            this.query = query;
            this.browsedTags = browsedTags;
            this.browsedItems = browsedItems;
            this.itemTagRelation = itemTagRelation;
            this.itemsScore = new Dictionary<int, int>();
        }

        public List<int> search()
        {
            if (string.IsNullOrEmpty(query)) {
                return null;
            }

            queryTags = StringManager.RemoveSpecials(query.ToUpper()).Split(' ').ToList();
            foreach (Item item in browsedItems) {
                itemsScore.Add(item.id, 0);
            }
            foreach (string queryTag in queryTags) {
                foreach (Item browsedTag in browsedTags) {
                    if (browsedTag.value.ToUpper().Contains(queryTag)) {
                        foreach (Relation relation in itemTagRelation) {
                            if (relation.itemB_id == browsedTag.id) {
                                itemsScore[relation.itemA_id] += 1;
                            }
                        }

                    }

                }

            }
            var dictionaryList = itemsScore.ToList();
            dictionaryList.Sort((x,y)=>y.Value.CompareTo(x.Value));
            List<int> browsedItemsByScore = new List<int>();
            foreach (var score in dictionaryList) {
                if (score.Value > 0) {
                    browsedItemsByScore.Add(score.Key);
                }
            }
            return browsedItemsByScore;

        }

    }
}
