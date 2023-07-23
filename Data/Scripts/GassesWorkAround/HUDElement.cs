using System;
using Sandbox.ModAPI;
using VRage.Game;
using VRage.ModAPI;
using VRage.Utils;

namespace Digi.Examples
{
    // HudStat classes are used by the game to compute data to be written to the various HUD elements.
    // Implementing IMyHudStat would trigger the game to use your class for that purpose aswell, and the Id property determines the stat it overrides (or creates if it's unique).
    // You can make new Ids and use them in a HUD definition SBC to have custom behaviors on the HUD.
    // However, do note that currently the HUD definition is very unfriendly to multiple mods changing it, only one mod's edits will remain depending on mod order.
    // 
    public class Example_HudStatOverride : IMyHudStat
    {
        public MyStringHash Id { get; private set; } = MyStringHash.GetOrCompute("controlled_tritium_capacity"); // the stat's ID to override, this one is the ship's mass number
        public float MinValue => 0; // these being used or not depend on the how the stat is used in the HUD definition.
        public float MaxValue => 100;
        public float CurrentValue { get; private set; }
        public string GetValueString() => CurrentValue.ToString("0.00"); // NOTE: must never return null!

        public Example_HudStatOverride()
        {
            // initialization stuff
        }

        public void Update() // gets executed every tick (60/s)
        {
            try
            {
                
                // ModAPI.IMyEntity ent = MyAPIGateway.Gui.InteractedEntity;
                // if (ent is MyCubeGrid) {
                    
                //     MyCubeGrid grid = ent as MyCubeGrid;
                //     var allblocks = grid.GetBlocks();

                //     foreach (var block in allblocks) {
                //         if (block is IMyGasTank) {
                //             IMyGasTank tank =  block as IMyGasTank;
                //             if (tank.) {

                //             }
                //         }
                //     }

                // }

            }
            catch(Exception e)
            {
                MyLog.Default.WriteLineAndConsole($"{e.Message}\n{e.StackTrace}");

                if(MyAPIGateway.Session?.Player != null)
                    MyAPIGateway.Utilities.ShowNotification($"[ ERROR: {GetType().FullName}: {e.Message} | Send SpaceEngineers.Log to mod author ]", 10000, MyFontEnum.Red);
            }
        }
    }
}