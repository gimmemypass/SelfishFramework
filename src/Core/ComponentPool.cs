using System;

namespace SelfishFramework.Core
{
    public interface IComponentPool
    {
        void Resize(int capacity);
    }
    public class ComponentPool<T> : IComponentPool where T : struct, IComponent
    {
        private readonly World world;

        private T[] denseItems;
        private int denseCount;
        //todo make resize
        private int[] sparseItems;
        private int sparseCount;
        private int[] recycledItems;
        private int recycledCount;

        public ComponentPool(World world, int length)
        {
            this.world = world;
            denseCount = 1;
            denseItems = new T[length];
            sparseItems = new int[length];
            recycledItems = new int[length];
        }

        public ref T Add(int actorId)
        {
            if(Has(actorId)) throw new Exception("Already added");
            int idx;
            if (recycledCount > 0)
            {
                idx = recycledItems[--recycledCount];
            }
            else
            {
                if (denseCount == denseItems.Length)
                {
                    Array.Resize(ref denseItems, denseCount << 1);
                }
                idx = denseCount++;
            }
            sparseItems[actorId] = idx;
            return ref denseItems[idx];
        }
        
        public ref T Get(int actorId)
        {
            return ref denseItems[sparseItems[actorId]];
        }

        public void Remove(int id)
        {
            if (!Has(id))
                return;
            if (recycledItems.Length == recycledCount)
            {
                Array.Resize(ref recycledItems, recycledCount << 1);
            }

            var idx = sparseItems[id];
            sparseItems[id] = 0;
            recycledItems[recycledCount++] = idx;
            denseItems[sparseItems[id]] = default;
        }

        public bool Has(int id)
        {
            return sparseItems[id] > 0;
        }

        public void Resize(int capacity)
        {
            Array.Resize(ref sparseItems, capacity); 
        }
    }
}