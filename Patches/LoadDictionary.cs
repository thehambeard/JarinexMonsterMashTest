﻿/* Remove this file form your project if you are not going use the EAHelpers.cs*/
using Kingmaker.Blueprints;
using Kingmaker.Localization;
using JMMT.Utilities;
using System;
using static JMMT.Utilities.SettingsWrapper;


namespace JMMT.Patches
{
    [HarmonyLib.HarmonyPatch(typeof(LibraryScriptableObject), "LoadDictionary")]
    [HarmonyLib.HarmonyPatch(typeof(LibraryScriptableObject), "LoadDictionary", new Type[0])]
    static class LibraryScriptableObject_LoadDictionary_Patch
    {
        static void Postfix(LibraryScriptableObject __instance)
        {
            var self = __instance;
            if (Main.Library != null) return;
            Main.Library = self;
            try
            {
#if DEBUG
                bool allow_guid_generation = true;
                
#else
                bool allow_guid_generation = false; //no guids should be ever generated in release
#endif
                Helpers.GuidStorage.load(Properties.Resources.blueprints, allow_guid_generation);
                JMMT.FixHydra.Fix(); //Load fix

#if DEBUG
                string guid_file_name = $@"{ModPath}blueprints.txt";
                Helpers.GuidStorage.dump(guid_file_name);
#endif
                Helpers.GuidStorage.dump($@"{ModPath}loaded_blueprints.txt");

                
            }
            catch (Exception ex)
            {
                Main.Mod.Error(ex);
            }
        }
    }
}
