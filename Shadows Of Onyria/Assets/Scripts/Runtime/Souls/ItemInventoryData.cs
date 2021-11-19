using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DoaT.Vendor
{
    [Serializable, CreateAssetMenu(menuName = "Data/Items/Inventory", fileName = "Items Inventory Data")]
    public class ItemInventoryData : ScriptableObject
    {
        public const int GOLD_CAP = 9999999;
        public event Action<int> OnGoldValueChanged;
        public event Action<int, Item> OnItemUpdate;

        [SerializeField] private int _gold;
        
        private Item _firstSlot;
        private Item _secondSlot;
        private Item _thirdSlot;
        private Item _fourthSlot;

        private bool _firstSlotOccupied = false;
        private bool _secondSlotOccupied = false;
        private bool _thirdSlotOccupied = false;
        private bool _fourthSlotOccupied = false;
        
        public int Gold
        {
            get => _gold;
            set
            {
                if (value == _gold) return;

                if (value > GOLD_CAP)
                {
                    OnGoldValueChanged?.Invoke(GOLD_CAP);
                    _gold = GOLD_CAP;
                    return;
                }

                if (value < 0)
                {
                    OnGoldValueChanged?.Invoke(0);
                    _gold = 0;
                    return;
                }
                
                OnGoldValueChanged?.Invoke(value);
                _gold = value;
            }
        }

        public Item FirstSlot
        {
            get => _firstSlot;
            set
            {
                _firstSlotOccupied = value != null;
                _firstSlot = value;
                OnItemUpdate?.Invoke(0, _firstSlot);
            }
        }
        public Item SecondSlot
        {
            get => _secondSlot;
            set
            {
                _secondSlotOccupied = value != null;
                _secondSlot = value;
                OnItemUpdate?.Invoke(1, _secondSlot);
            }
        }
        public Item ThirdSlot
        {
            get => _thirdSlot;
            set
            {
                _thirdSlotOccupied = value != null;
                _thirdSlot = value;
                OnItemUpdate?.Invoke(2, _thirdSlot);
            }
        }
        public Item FourthSlot
        {
            get => _fourthSlot;
            set
            {
                _fourthSlotOccupied = value != null;
                _fourthSlot = value;
                OnItemUpdate?.Invoke(3, _fourthSlot);
            }
        }
        
        public bool FirstSlotOccupied => _firstSlotOccupied;
        public bool SecondSlotOccupied => _secondSlotOccupied;
        public bool ThirdSlotOccupied => _thirdSlotOccupied;
        public bool FourthSlotOccupied => _fourthSlotOccupied;

        public bool HasRoom()
        {
            return !_firstSlotOccupied
                || !_secondSlotOccupied
                || !_thirdSlotOccupied
                || !_fourthSlotOccupied;
        }

        public void Clear()
        {
            Gold = 0;
            
            FirstSlot = null;
            SecondSlot = null;
            ThirdSlot = null;
            FourthSlot = null;

            OnGoldValueChanged = default;
            OnItemUpdate = default;
        }

        public void SetItemInFirstOpenSlot(Item item)
        {
            if (!_firstSlotOccupied)
            {
                FirstSlot = item;
                return;
            }
            
            if (!_secondSlotOccupied)
            {
                SecondSlot = item;
                return;
            }
            
            if (!_thirdSlotOccupied)
            {
                ThirdSlot = item;
                return;
            }
            
            if (!_fourthSlotOccupied)
            {
                FourthSlot = item;
                return;
            }

            throw new Exception("No slot is empty and you tried to allocate an item");
        }

        public Item GetItem(int index)
        {
            return index switch
            {
                0 => FirstSlot,
                1 => SecondSlot,
                2 => ThirdSlot,
                3 => FourthSlot,
                _ => throw new IndexOutOfRangeException()
            };
        }

        public void ClearItem(int index)
        {
            switch (index)
            {
                case 0:
                    FirstSlot = null;
                    break;
                case 1:
                    SecondSlot = null;
                    break;
                case 2:
                    ThirdSlot = null;
                    break;
                case 3:
                    FourthSlot = null;
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }
        }
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(ItemInventoryData))]
    public class ItemInventoryDataInspector : Editor
    {
        private ItemInventoryData _target;

        private void OnEnable()
        {
            _target = (ItemInventoryData)target;
        }

        public override void OnInspectorGUI()
        {
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script:", MonoScript.FromScriptableObject(_target), typeof(ItemInventoryData), false);
            GUI.enabled = true;
            
            //EditorUtility.SetDirty(target);

            GUILayout.Space(10);

            _target.Gold = EditorGUILayout.IntField("Gold", _target.Gold);
            
            GUILayout.Space(10);

            var one = (Item) EditorGUILayout.ObjectField("Item Slot 1", _target.FirstSlot, typeof(Item), true);
            var two = (Item) EditorGUILayout.ObjectField("Item Slot 2", _target.SecondSlot, typeof(Item), true);
            var three = (Item) EditorGUILayout.ObjectField("Item Slot 3", _target.ThirdSlot, typeof(Item), true);
            var four = (Item) EditorGUILayout.ObjectField("Item Slot 4", _target.FourthSlot, typeof(Item), true);
            
            if(one != _target.FirstSlot) 
            {
                EditorUtility.SetDirty(_target);
                _target.FirstSlot = one;
            }
            if(two != _target.SecondSlot) 
            {
                EditorUtility.SetDirty(_target);
                _target.SecondSlot = two;
            }
            if(three != _target.ThirdSlot) 
            {
                EditorUtility.SetDirty(_target);
                _target.ThirdSlot = three;
            }
            if(four != _target.FourthSlot) 
            {
                EditorUtility.SetDirty(_target);
                _target.FourthSlot = four;
            }
            
            GUILayout.Space(10);

            if (GUILayout.Button("Clear"))
            {
                _target.Clear();
                EditorUtility.SetDirty(_target);
            }
            
            if (EditorUtility.IsDirty(_target)) AssetDatabase.SaveAssets();
            
            // if (!EditorUtility.IsDirty(_target)) GUI.enabled = false;
            // if (GUILayout.Button("Save"))
            // {
            //     AssetDatabase.SaveAssets();
            // }
            // if (!EditorUtility.IsDirty(_target)) GUI.enabled = true;
        }
    }
#endif
}