using Chloride.RA2Scripts.Formats;
using Chloride.RA2Scripts.Utils;

namespace Chloride.RA2Scripts
{
    public static class MainConsole
    {
        private static readonly IniDoc Config = new();
        internal static IniSection Arguments = Config.Default;

        static MainConsole()
        {
            // = =
            var path = Path.Join(Environment.CurrentDirectory, "config.ini");

            Config.Deserialize(new FileInfo(path));
        }
        public static void Main(string[] args)
        {
            var file = new FileInfo(GetArg("FilePath").ToString());
            var doc = IniUtils.ReadIni(file);
            new OwnerMapScript(Config).TransferOwnerReference(
                doc,
                GetArg("Old").ToString(),
                GetArg("New").ToString());
            doc.Serialize(file);
        }

        public static IniValue GetArg(string key)
        {
            _ = Arguments.Contains(key, out IniValue ret);
            return ret;
        }
    }
}
