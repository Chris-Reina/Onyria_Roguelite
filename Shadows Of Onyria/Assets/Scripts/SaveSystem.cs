using System.IO;
using UnityEngine;
using DoaT.UI;

namespace DoaT.Save
{
    public class SaveSystem : MonoBehaviour
    {
        private static SaveSystem Current { get; set; }
        private const int SAVE_SLOTS_AMOUNT = 3;

        private SaveDataStructure _data;

        [SerializeField] private string _saveRoot = "/Resources";
        [SerializeField] private string _saveFileName = "SaveFile";
        [SerializeField] private string _extension = "dat";

        [Space(10)] 
        [SerializeField] private string _confirmationDeleteSaveMessage = "Do you really want to delete this save file?";

        public static bool FileDetected
        {
            get
            {
                if (Current == null) return false;
                
                for (int i = 0; i < SAVE_SLOTS_AMOUNT; i++)
                {
                    if (Current.FileExists(i))
                        return true;
                }

                return false;
            }
        }

        private void Awake()
        {
            if (Current == null)
                Current = this;
            else if (Current != this)
            {
                Destroy(this);
                return;
            }
        }
        
        public bool FileExists(int slot) => File.Exists(GetSlotPath(slot));
        private string GetSlotPath(int slot) => $"{Application.dataPath}{_saveRoot}/{_saveFileName}_{slot}.{_extension}";
        private void ClearDataStructure() => _data = new SaveDataStructure();


        public static void SaveGame(int slot) => Current.SaveGameImpl(slot);
        private void SaveGameImpl(int slot)
        {
            ClearDataStructure();

            _data.metaData = new SaveMetaData().SetLevel(PersistentData.Player.Level)
                                               .SetZone("Headquarters")
                                               .SetPlayedTime(0);
            _data.gameFlags.SaveFromAsset(GameState.Current);
            _data.player.SaveFromAsset(PersistentData.Player);
            _data.soulInventory.SaveFromAsset(PersistentData.SoulInventory.data);

            _data.SaveBinary(GetSlotPath(slot));
        }

        public static void LoadGame(int slot) => Current.LoadGameImpl(slot);
        private void LoadGameImpl(int slot)
        {
            if (!FileExists(slot)) return;

            ClearDataStructure();

            _data = BinarySerializer.LoadBinary<SaveDataStructure>(GetSlotPath(slot));

            _data.gameFlags.LoadIntoAsset(GameState.Current);
            _data.player.LoadIntoAsset(PersistentData.Player);
            _data.soulInventory.LoadIntoAsset(PersistentData.SoulInventory.data);
        }

        public static bool GetSavedMetaData(int slot, out SaveMetaData data) => Current.GetSavedMetaDataImpl(slot, out data);
        private bool GetSavedMetaDataImpl(int slot, out SaveMetaData data)
        {
            var exists = FileExists(slot);

            data = exists ? BinarySerializer.LoadBinary<SaveDataStructure>(GetSlotPath(slot)).metaData : null;
            return exists;
        }

        public static void DeleteSaveFile(int slot) => Current.DeleteSaveFileImpl(slot);
        private void DeleteSaveFileImpl(int slot)
        {
            ConfirmationWindow.Create(_confirmationDeleteSaveMessage, () => DeleteFile(slot));
        }

        private void DeleteFile(int slot)
        {
            //TODO: Implement
        }
    }
}
