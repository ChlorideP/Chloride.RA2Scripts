namespace Chloride.CCINIExt
{
    public class CCIni : Ini
    {
        private readonly FileInfo file;
        private readonly string codec;

        public CCIni(FileInfo ini, string encoding = "utf-8") : base()
        {
            file = ini;
            codec = encoding;
            if (!ini.Exists)
                throw new FileNotFoundException(ini.FullName);
            Load(new FileInfo[] { ini }, encoding);
        }

        public override string ToString() => file.Name;

        public override void Save(FileInfo? dest = null, string? codec = null, bool space = false)
        {
            if (string.IsNullOrEmpty(codec))
                codec = this.codec;
            dest ??= file;

            base.Save(dest, codec, space);
        }
    }
}
