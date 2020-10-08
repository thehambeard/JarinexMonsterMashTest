using HarmonyLib;
using JMMT.Utilities;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Facts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using ModMaker;
using Kingmaker.PubSubSystem;
using Kingmaker;
using Kingmaker.Localization;

namespace JMMT.JMMT
{
    class FixHydra : IModEventHandler, IAreaLoadingStagesHandler //ModMaker event interface for handling mod enable/disable, IArea is for the onAreaLoad methods
    {
        static LibraryScriptableObject library => Main.Library;

        public int Priority => 200; //don't really worry about this.  Mainly helpful with menu creation. 

        static readonly string originHydraBP = "3b8ffcb67319ef94d88e1ed4c5a89b71";
        static readonly string copyHydraBP = "c170726da63c4b20a18c585623e28e98";
        static readonly string areaToReplaceBPIn = "973dacf5a7642a04183f0f10539ce093";

        static BlueprintUnit hydra;

        public static void Fix()
        {
            hydra = library.CopyAndAdd<BlueprintUnit>(originHydraBP, "JMMTHydra", copyHydraBP); //copies, not a deep copy
            hydra.LocalizedName = ScriptableObject.CreateInstance<SharedStringAsset>(); //Creates a new instance so the original hydra's name doesn't change too.
            hydra.LocalizedName.String = Helpers.CreateString("JMMTHydraName", "Big Dipshit"); //Changes the name of the custom hydra
            hydra.AddFacts = new BlueprintUnitFact[] //creates a new instance of AddFacts so the original hydra doesn't change, some things you can use new for, some things you need CreateInstance like above
            {
                library.Get<BlueprintBuff>("00402bae4442a854081264e498e7a833"), //displacement
                library.Get<BlueprintUnitFact>("f7f31752d838ff7429dbdbbd38e55a9d") //naturalArmor36
            };
        }

        public void HandleModDisable()
        {
            EventBus.Subscribe(this); //removes the event
        }

        public void HandleModEnable()
        {
            EventBus.Subscribe(this); //Adds the onAreaScenesLoaded event
        }

        public void OnAreaLoadingComplete() //once the area is loaded, check for the original hydra
        {
            if (Game.Instance.CurrentlyLoadedArea.AssetGuid.Equals(areaToReplaceBPIn))
            {
                foreach (var unit in Game.Instance.State.Units)
                {
                    if (unit.Blueprint.AssetGuidThreadSafe.Equals(originHydraBP))
                    {
                        Game.Instance.EntityCreator.SpawnUnit(hydra, unit.Position, Quaternion.LookRotation(unit.OrientationDirection), Game.Instance.CurrentScene.MainState);
                        unit.Destroy();
                    }
                }
            }
        }

        public void OnAreaScenesLoaded() //unused
        {
            
        }
    }
}
