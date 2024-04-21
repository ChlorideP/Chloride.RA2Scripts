using Chloride.RA2Scripts.Formats;
using Chloride.RA2Scripts.Utils;

namespace Chloride.RA2Scripts
{
    internal static class CellTagMapScript
    {
        internal static void ShowReferredTrigger(IniDoc doc)
        {
            var refs = new HashSet<string>();
            IniUtils.IteratePairs(doc, "CellTags", (key, val) => refs.Add(val.ToString()));
            foreach (var i in refs)
                Console.WriteLine($"{i} - {doc["Tags"][i]}");
        }
    }
}
