namespace Chloride.CCINIExt
{
    public class CCIni : Ini
    {
        private readonly FileInfo File;
        public readonly string Encoding;

        public CCIni(FileInfo ini, string encoding = "utf-8") : base()
        {
            File = ini;
            Encoding = encoding;
            if (!ini.Exists)
                throw new FileNotFoundException(ini.FullName);
            Load(new FileInfo[] { ini }, encoding);
        }

        public override string ToString() => File.Name;

        public override void Save(FileInfo? dest = null, string? codec = null, bool space = false)
        {
            if (string.IsNullOrEmpty(codec))
                codec = Encoding;

            base.Save(dest ?? File, codec, space);
        }
    }
}
