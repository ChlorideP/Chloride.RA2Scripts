using Chloride.RA2Scripts.Components;
using Chloride.RA2Scripts.Formats;
using Chloride.RA2Scripts.Utils;

namespace Chloride.RA2Scripts
{
    internal class TriggerMapScript
    {
        internal HashSet<string> ActionsWithTriggerParam = new() { "12", "22", "53", "54" };

        /// <summary>
        /// <para/>You have to instantiate these scripts for loading external data:
        /// <list type="bullet">
        /// <item>INI key <c>ExtActionsWithTrigger</c></item>
        /// </list>
        /// <para/>Hint: INI keys above should be placed in the head of config,
        /// without included in any sections of it.
        /// </summary>
        /// <param name="argsDict">The header of <c>config.ini</c>, just send <c>IniDoc.Default</c> property.</param>
        internal TriggerMapScript(IniSection argsDict)
        {
            if (argsDict.Contains("ExtActionsWithTrigger", out IniValue ext))
                ActionsWithTriggerParam = ActionsWithTriggerParam.Union(ext.Split()).ToHashSet();
        }

        internal void CheckoutNullReferences(IniDoc doc) => IniUtils.IteratePairs(doc, "Actions", (key, val) =>
        {
            /* TActions may have the following properties in game: 
            *      p1 as ValueType { int, string, Trigger, ... }
            *      p2 as Value (which is real p1)
            *      p3 as Param3, p4 as Param4, ...
            *      p7 as WaypointParam.
            * So the trigger references would only be in the p2.
            */
            TriggerActions ta = new(val.Split());
            for (; ta.Seekable; ta.Next())
            {
                if (!ActionsWithTriggerParam.Contains(ta.CurrentID))
                    continue;
                var triggerid = ta.GetCurrentParamX(2);
                if (doc.ContainsKey("Triggers", triggerid))
                    continue;
                Console.WriteLine($"Found null trigger reference {triggerid} in {key}");
            }
        });
    }
}
