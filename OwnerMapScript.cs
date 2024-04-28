using Chloride.RA2Scripts.Components;
using Chloride.RA2Scripts.Formats;
using Chloride.RA2Scripts.Utils;
using static Chloride.RA2Scripts.Constants;

namespace Chloride.RA2Scripts;
internal class OwnerMapScript
{
    /// <summary>
    /// <para/>Key for ID, Value for house param index.
    /// <para/>Since actions are perfectly aligned (all have 7 params),
    /// the TriggerAction wrapper would consider p1 just as param.
    /// </summary>
    internal Dictionary<string, int> ActionsWithOwner = new();
    /// <summary>
    /// <para/>Key for ID, Value for house param index.
    /// <para/>Unlike the actions, events would have to take p1 as param tag,
    /// so right here the index actually begins from p2.
    /// </summary>
    internal Dictionary<string, int> EventsWithOwner = new();
    internal HashSet<string> ScriptsWithOwner = new();

    /// <summary>
    /// <para/>You have to instantiate these scripts for loading external data:
    /// <list type="bullet">
    /// <item>INI key <c>ExtOwnerScripts</c></item>
    /// <item>INI section <c>ExtOwnerEvents</c></item>
    /// <item>INI section <c>ExtOwnerActions</c></item>
    /// </list>
    /// <para/>Hint: INI keys above should be placed in the head of config,
    /// without included in any sections of it.
    /// </summary>
    internal OwnerMapScript(IniDoc config)
    {
        _ = config.Default.Contains("ExtOwnerScripts", out IniValue extScripts);
        ScriptsWithOwner = ScriptsWithOwner.Union(extScripts.Split()).ToHashSet();

        _ = config.Contains("ExtOwnerEvents", out IniSection? extEvents);
        foreach (var i in extEvents ?? new(string.Empty))
            EventsWithOwner.Add(i.Key, i.Value.Convert<int>());

        _ = config.Contains("ExtOwnerActions", out IniSection? extActions);
        foreach (var i in extActions ?? new(string.Empty))
            ActionsWithOwner.Add(i.Key, i.Value.Convert<int>());
    }

    /* step 1: check out alliance
     * step 2: check out easy refs
     *  - 2.1 teams (House=
     *  - 2.2 triggers (string[0])
     *  - 2.3 technos (string[0])
     *  
     * step 3: check out int args
     *  3.1 events, actions
     *  3.2 scripts (20
     */

    /// <summary>
    /// <para/>To keep original relationships if you want to.
    /// <para/>Hint: No need to input "XX House", just "XX".
    /// </summary>
    internal static void TransferOwnerAlliance(IniDoc doc, string old, string _new)
    {
        old = $"{old} House";
        _new = $"{_new} House";
        if (!doc.Contains(old, out IniSection? oldsect) || !doc.Contains(_new, out IniSection? newsect))
            return;

        // transfer old alliance
        List<string> newRelationships = newsect!["Allies"].Split().ToList();
        foreach (var i in oldsect!["Allies"].Split())
        {
            if (i == old)
                continue;
            if (newRelationships.Contains(i))
                continue;
            newRelationships.Add(i);
        }
        newsect["Allies"] = IniValue.Join<string>(newRelationships);

        // change all other houses alliance.
        foreach (var i in doc.GetTypeList("Houses"))
        {
            if (i == old)
                continue;
            if (!doc.Contains(i, out IniSection? iHouse))
                continue;
            iHouse!["Allies"].Value = iHouse!["Allies"].Value?.Replace(old, _new);
        }
    }

    /// <summary>
    /// Just copy the IQ, Color, Spawned aircrafts direction, etc.
    /// </summary>
    internal static void TransferOwnerConfigs(IniDoc doc, string oldHouse, string newHouse)
    {
        if (!doc.Contains(oldHouse, out IniSection? oldsect) || !doc.Contains(newHouse, out IniSection? newsect))
            return;
        doc[newHouse, "IQ"] = doc[oldHouse, "IQ"];
        doc[newHouse, "Edge"] = doc[oldHouse, "Edge"];
        doc[newHouse, "Color"] = doc[oldHouse, "Color"];
        doc[newHouse, "PlayerControl"] = doc[oldHouse, "PlayerControl"];
        if (doc[newHouse].Contains("PlayerControl", out IniValue pc) && pc.Convert())
        {
            doc[newHouse, "NodeCount"] = 0.ToString();
            doc[newHouse].Update(doc[newHouse].Where(i => !int.TryParse(i.Key, out _)));
        }
    }

    /// <summary>
    /// <para/>To replace all old house STRING occurrence with the new, like
    /// <list type="bullet">
    /// <item/>Team house
    /// <item/>Trigger house
    /// <item/>Techno (Infantry, Units, etc) owner
    /// </list>
    /// <para/>Hint: No need to input "XX House", just "XX".
    /// </summary>
    internal static void TransferOwnerStringRefs(IniDoc doc, string old, string _new)
    {
        var h_old = $"{old} House";
        var h_new = $"{_new} House";

        // by name
        foreach (var i in doc.GetTypeList("TeamTypes"))
        {
            if (!doc.Contains(i, out IniSection? team))
                continue;
            if (team!.Contains("House", out IniValue teamHouse) && teamHouse.ToString() != old)
                continue;
            team["House"] = _new;
        }

        IniUtils.ReplaceValue(doc, "Triggers", triggerinfo =>
        {
            if (triggerinfo[HouseIndex] == old)
                triggerinfo[HouseIndex] = _new;
        });

        IniUtils.ReplaceValue(doc, "Infantry", techno =>
        {
            if (techno[HouseIndex] == h_old)
                techno[HouseIndex] = h_new;
        });
        IniUtils.ReplaceValue(doc, "Units", techno =>
        {
            if (techno[HouseIndex] == h_old)
                techno[HouseIndex] = h_new;
        });
        IniUtils.ReplaceValue(doc, "Aircraft", techno =>
        {
            if (techno[HouseIndex] == h_old)
                techno[HouseIndex] = h_new;
        });
        IniUtils.ReplaceValue(doc, "Structures", techno =>
        {
            if (techno[HouseIndex] == h_old)
                techno[HouseIndex] = h_new;
        });
    }

    /// <summary>
    /// To replace all old house INDEXES with the new,
    /// like Events, Actions or Scripts house parameter(s).
    /// </summary>
    internal void TransferOwnerIntRefs(IniDoc doc, string oldHouse, string newHouse)
    {
        // by int args
        var houses = doc.GetTypeList("Houses");
        var idxOld = Array.IndexOf(houses, oldHouse);
        var idxNew = Array.IndexOf(houses, newHouse);

        TransferOwnerIntRefs(doc, idxOld, idxNew);
    }

    /// <summary>
    /// To replace all old house INDEXES with the new,
    /// like Events, Actions or Scripts house parameter(s).
    /// </summary>
    internal void TransferOwnerIntRefs(IniDoc doc, int idxOld, int idxNew)
    {
        IniUtils.ReplaceValue(doc, "Actions", tActions =>
        {
            TriggerActions ta = new(tActions);
            for (; ta.Seekable; ta.Next())
            {
                if (!ActionsWithOwner.TryGetValue(ta.CurrentID, out int idx))
                    continue;
                if (ta.GetCurParamX(idx) != idxOld.ToString())
                    continue;
                ta.SetCurParamX(idx, idxNew.ToString());
            }
        });

        IniUtils.ReplaceValue(doc, "Events", tEvents =>
        {
            TriggerEvents te = new(tEvents);
            for (; te.Seekable; te.Next())
            {
                if (!EventsWithOwner.TryGetValue(te.CurrentID, out int idx))
                    continue;
                if (te.GetCurParamX(idx) != idxOld.ToString())
                    continue;
                te.SetCurParamX(idx, idxNew.ToString());
            }
        });

        foreach (var i in doc.GetTypeList("ScriptTypes"))
        {
            IniUtils.ReplaceValue(doc, i, cur => // scriptid, scriptparam
            {
                if (cur.Length < 2)
                    return;
                if (ScriptsWithOwner.Contains(cur[0]) && cur[1] == idxOld.ToString())
                    cur[1] = idxNew.ToString();
            });
        }
    }
}
