using Sandbox.Common.ObjectBuilders;
using Sandbox.Game.Entities;
using Sandbox.Game.EntityComponents;
using System;
using Sandbox.ModAPI;
using VRage.Game.Components;
using VRage.Game;
using VRage.ModAPI;
using VRage.Utils;

namespace Horizon.TritHUD
{
    // HudStat classes are used by the game to compute data to be written to the various HUD elements.
    // Implementing IMyHudStat would trigger the game to use your class for that purpose aswell, and the Id property determines the stat it overrides (or creates if it's unique).
    // You can make new Ids and use them in a HUD definition SBC to have custom behaviors on the HUD.
    // However, do note that currently the HUD definition is very unfriendly to multiple mods changing it, only one mod's edits will remain depending on mod order.
    // 
    public class Trit_HudStatOverride : IMyHudStat
    {
        public MyStringHash Id { get; private set; } = MyStringHash.GetOrCompute("controlled_tritium_capacity"); // the stat's ID to override, this one is the ship's mass number
        public float MinValue => 0; // these being used or not depend on the how the stat is used in the HUD definition.
        public float MaxValue => 100;
        public float CurrentValue { get; private set; }
        public string GetValueString() => ((int)CurrentValue).ToString(); // NOTE: must never return null!

        public Trit_HudStatOverride()
        {
            CurrentValue = 0;
        }

        public void Update() // gets executed every tick (60/s)
        {
            IMyCubeGrid grid;

            if(MyAPIGateway.Session == null)
                return;

            IMyEntity entity = MyAPIGateway.Session.Player?.Controller?.ControlledEntity?.Entity;
            if (entity is IMyCubeBlock) {
                grid = (entity as IMyCubeBlock).CubeGrid;
            }
            else
            {
                return;
            }

            CurrentValue = 0;
            float total = 0;
            float fill = 0;

            MyDefinitionId TritiumDefId = MyDefinitionId.Parse("MyObjectBuilder_GasProperties/Tritium");

            foreach (var block in grid.GetFatBlocks<IMyGasTank>()) {
                // Need to consider blocks can have different capacities
                var sink = block.Components.Get<MyResourceSinkComponent>();
                if (sink != null && sink.AcceptedResources.IndexOf(TritiumDefId) != -1)
                {
                    total += block.Capacity;
                    fill += block.Capacity * (float)block.FilledRatio;
                }
            }

            // If total is 0, just set 0 and return early
            if (total == 0) {
                CurrentValue = 0;
            }
            else
            {
                // Calculate final capacity
                CurrentValue = (fill / total) * 100;
            }

            // Should never be above 100
            if (CurrentValue > 100)
            {
                CurrentValue = 100;
            }

            // Should never be below 0
            if (CurrentValue < 0)
            {
                CurrentValue = 0;
            }
        }
    }


    // If CurrentValue = 1, then blink the Trit UI (because you are 25% fuel or less). But we need to check if they even had Trit tanks, otherwise dont make it red for a hydro only ship
    public class Trit_HudStatWarning : IMyHudStat
    {
        public MyStringHash Id { get; private set; } = MyStringHash.GetOrCompute("tritium_warning_on"); // the stat's ID to override, this one is the ship's mass number
        public float MinValue => 0; // these being used or not depend on the how the stat is used in the HUD definition.
        public float MaxValue => 1;
        public float CurrentValue { get; private set; }
        public string GetValueString() => ((int)CurrentValue).ToString(); // NOTE: must never return null!

        public Trit_HudStatWarning()
        {
            CurrentValue = 0;
        }

        public void Update() // gets executed every tick (60/s)
        {
            IMyCubeGrid grid;

            if(MyAPIGateway.Session == null)
                return;

            IMyEntity entity = MyAPIGateway.Session.Player?.Controller?.ControlledEntity?.Entity;
            if (entity is IMyCubeBlock) {
                grid = (entity as IMyCubeBlock).CubeGrid;
            }
            else
            {
                return;
            }

            CurrentValue = 0;
            float current = 0;
            float total_capacity = 0;
            float fill_amount = 0;

            MyDefinitionId TritiumDefId = MyDefinitionId.Parse("MyObjectBuilder_GasProperties/Tritium");

            foreach (var block in grid.GetFatBlocks<IMyGasTank>()) {
                // Need to consider blocks can have different capacities
                var sink = block.Components.Get<MyResourceSinkComponent>();
                if (sink != null && sink.AcceptedResources.IndexOf(TritiumDefId) != -1)
                {
                    total_capacity += block.Capacity;
                    fill_amount += block.Capacity * (float)block.FilledRatio;
                }
            }

            // If total is less than 1 - that means no Trit tanks, so dont blink
            if (total_capacity < 1) {
                CurrentValue = 0;
            }
            else
            {
                // Calculate final capacity
                current = (fill_amount / total_capacity) * 100;

                // If current less than 25% - then blink (since we have Trit tanks)
                if (current < 25)
                {
                    CurrentValue = 1;
                }
                else
                {
                    CurrentValue = 0;
                }
            }


        }
    }
}
