using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuroWnd
{
    public struct DataNode<T>
    {
        public string Key;
        public T Data;
    }

    public class DataContainer<T>
    {
        private List<DataNode<T>> dataList;

        public DataContainer()
        {
            dataList = new List<DataNode<T>>();
        }
        public void AddData(string key, T data)
        {
            foreach (DataNode<T> item in dataList)
            {
                if (String.Compare(item.Key, key) == 0)
                    throw new Exception("This key is already used");
            }

            DataNode<T> dataNode = new DataNode<T>();
            dataNode.Key = key;
            dataNode.Data = data;

            dataList.Add(dataNode);
        }
        public T FindData(string key)
        {
            foreach (DataNode<T> item in dataList)
            {
                if (String.Compare(item.Key, key) == 0)
                    return item.Data;
            }
            return default(T);
        }
        public void DeleteData(string key)
        {
            foreach (DataNode<T> item in dataList)
            {
                if (String.Compare(item.Key, key) == 0)
                    dataList.Remove(item);
            }
        }
        public void Clear()
        {
            dataList.Clear();
        }
    }
}
