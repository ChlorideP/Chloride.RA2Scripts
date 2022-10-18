namespace Chloride.CCiniExt.Scripts
{
    public class INI
    {
        public IniDoc ini;
        public FileInfo file;

        public INI(string path)
        {
            file = new FileInfo(path);
            ini = new IniDoc();
            ini.Deserialize(file);
        }

        internal IniSection this[string section] { get => ini[section]; set => ini[section] = value; }
        internal string? this[string sect, string key] { get => ini[sect, key]; set => ini[sect, key] = value; }

        public void Load(params FileInfo[] files) => files.Where(i => i.Exists).ToList().ForEach(i => ini.Deserialize(i));

        public void Save(FileInfo? file = null, string encoding = "utf-8") => ini.Serialize(file ?? this.file, encoding);
    }
}
