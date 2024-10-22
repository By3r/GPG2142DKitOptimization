using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


// Can be optimized 100%%

namespace Gamekit2D
{
    public class InventoryController : MonoBehaviour, IDataPersister
    {
        [System.Serializable]
        public class InventoryEvent
        {
            public string key;
            public UnityEvent OnAdd, OnRemove;
        }


        [System.Serializable]
        public class InventoryChecker
        {
            public string[] inventoryItems;
            public UnityEvent OnHasItem, OnDoesNotHaveItem;

            public virtual bool CheckInventory(InventoryController inventory) //-------------- D immediately stop the function if an item is not found instead of ignoring each line then stopping.
            {
                if (inventory != null)
                {
                    for (var i = 0; i < inventoryItems.Length; i++)
                    {
                        if (!inventory.HasItem(inventoryItems[i]))
                        {
                            OnDoesNotHaveItem.Invoke();
                            return false;
                        }
                    }
                    OnHasItem.Invoke();
                    return true;
                }
                return false;
            }
        }


        public InventoryEvent[] inventoryEvents;
        public event System.Action OnInventoryLoaded;

        public DataSettings dataSettings;

        HashSet<string> m_InventoryItems = new HashSet<string>();

        void OnEnable()
        {
            PersistentDataManager.RegisterPersister(this);
        }

        void OnDisable()
        {
            PersistentDataManager.UnregisterPersister(this);
        }

        public void AddItem(string key)
        {
            if (!m_InventoryItems.Contains(key))
            {
                m_InventoryItems.Add(key);
                var ev = GetInventoryEvent(key);
                if (ev != null) ev.OnAdd.Invoke();
            }
        }

        public void RemoveItem(string key)
        {
            if (m_InventoryItems.Contains(key))
            {
                var ev = GetInventoryEvent(key);
                if (ev != null) ev.OnRemove.Invoke();
                m_InventoryItems.Remove(key);
            }
        }

        public bool HasItem(string key)
        {
            return m_InventoryItems.Contains(key);
        }

        public void Clear()
        {
            m_InventoryItems.Clear();
        }

        InventoryEvent GetInventoryEvent(string key)
        {
            foreach (var iv in inventoryEvents)
            {
                if (iv.key == key) return iv;
            }
            return null;
        }

        public DataSettings GetDataSettings()
        {
            return dataSettings;
        }

        public void SetDataSettings(string dataTag, DataSettings.PersistenceType persistenceType)
        {
            dataSettings.dataTag = dataTag;
            dataSettings.persistenceType = persistenceType;
        }

        public Data SaveData()
        {
            return new Data<HashSet<string>>(m_InventoryItems);
        }

        public void LoadData(Data data)
        {
            Data<HashSet<string>> inventoryData = (Data<HashSet<string>>)data;
            foreach (var i in inventoryData.value)
                AddItem(i);
            if (OnInventoryLoaded != null) OnInventoryLoaded();
        }
    }
}