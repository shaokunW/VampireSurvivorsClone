using UnityEngine;

namespace Vampire
{
    public class CollectItem: MonoBehaviour
    {
        public int id;
        public bool collected;
        
        public void Initialize(int id)
        {
            this.id = id;
            this.collected = false;
        }

        public int Collect()
        {
            collected = true;
            return id;
        }
    }
}