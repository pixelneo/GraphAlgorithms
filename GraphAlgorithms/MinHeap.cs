using System;
using System.Collections.Generic;

namespace DataStructures
{
    public class MinHeap
    {

        private List<Tuple<int, int>> elements; //list of values and keys
        public Dictionary<int, int> indexInHeap { //get index in elements by nodeKey
            get;
            private set;
        }

        public MinHeap() {
            elements = new List<Tuple<int, int>>();
            indexInHeap = new Dictionary<int, int>();
        }

        public int Count {
            get {
                return elements.Count;
            }
        }

        public int Insert(int value, int key) {
            elements.Add(new Tuple<int, int>(value, key));
            indexInHeap.Add(key, elements.Count);
            int i = elements.Count;
            while (i != 0 && elements[parent(i)].Item1 > elements[i].Item1) {
                var temp = elements[i];
                elements[i] = elements[parent(i)];
                indexInHeap.Remove(elements[parent(i)].Item2);
                indexInHeap.Add(elements[parent(i)].Item2, i);
                elements[parent(i)] = temp;
                indexInHeap.Remove(temp.Item2);
                indexInHeap.Add(temp.Item2, parent(i));
                i = parent(i);
            }
            return i;
        }

        public Tuple<int, int> ExtractMin() {

            if (elements.Count <= 0)
                return null;
            Tuple<int, int> root;
            if (elements.Count == 1) {
                root = elements[0];
                elements.RemoveAt(elements.Count - 1);
                indexInHeap.Remove(elements[0].Item2);
                return root;
            }

            root = elements[0];
            elements[0] = elements[elements.Count - 1];
            elements.RemoveAt(elements.Count - 1);
            indexInHeap.Remove(root.Item2);

            indexInHeap.Remove(elements[elements.Count - 1].Item2);
            indexInHeap.Add(elements[elements.Count - 1].Item2, 0);
            heapify(0);

            return root;
        }

        public void DecreaseKey(int key, int value) {
            var index = indexInHeap[key];
            elements[index] = new Tuple<int, int>(value, key);
            indexInHeap.Remove(key);
            indexInHeap.Add(key, index);
            while (index != 0 && elements[parent(index)].Item1 > elements[index].Item1) {
                var temp = elements[index];
                elements[index] = elements[parent(index)];
                indexInHeap.Remove(elements[parent(index)].Item2);
                indexInHeap.Add(elements[parent(index)].Item2, index);
                elements[parent(index)] = temp;
                indexInHeap.Remove(temp.Item2);
                indexInHeap.Add(temp.Item2, parent(index));
                index = parent(index);
            }
        }

        private void heapify(int index) {
            var left = leftChild(index);
            var right = rightChild(index);
            var smallest = index;
            if (left < elements.Count && elements[left].Item1 < elements[smallest].Item1)
                smallest = left;
            if (right < elements.Count && elements[right].Item1 < elements[smallest].Item1)
                smallest = right;
            if (smallest != index) {
                var temp = elements[index];
                elements[index] = elements[smallest];
                indexInHeap.Remove(elements[smallest].Item2);
                indexInHeap.Add(elements[smallest].Item2, index);
                elements[smallest] = temp;
                indexInHeap.Remove(temp.Item2);
                indexInHeap.Add(temp.Item2, smallest);
                heapify(smallest);
            }
        }

        private int parent(int index) {
            return (index - 1) / 2;
        }

        private int leftChild(int index) {
            return (2 * index + 1);
        }
        private int rightChild(int index) {
            return (2 * index + 2);
        }

    }
}
