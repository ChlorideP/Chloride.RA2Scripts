using Chloride.RA2Scripts.Formats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chloride.RA2Scripts
{
    internal static class CellTagMapScript
    {
        internal static void ShowReferredTrigger(IniDoc doc)
        {
            if (!doc.Contains("CellTags", out IniSection? celltags))
                return;
            var refs = new HashSet<string>();
            foreach (var i in celltags!)
            {
                refs.Add(i.Value.ToString());
            }

            foreach (var i in refs)
            {
                Console.WriteLine($"{i} - {doc["Tags"][i]}");
            }
        }
    }
}
