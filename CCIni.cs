using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Chloride.CCINIExt
{
    public class CCIni : Ini
    {
        private string filePath;
        private string codec;

        public CCIni(FileInfo ini, string encoding = "utf-8") : base()
        {
            filePath = ini.FullName;
            codec = encoding;
            if (!ini.Exists)
                throw new FileNotFoundException(ini.FullName);
            Load(new FileInfo[] { ini }, encoding);
        }

        public override string ToString() => Path.GetFileName(filePath);

        public new void Save(string? dest = null, string? codec = null, bool space = false)
        {
            if (string.IsNullOrEmpty(codec))
                codec = this.codec;
            if (string.IsNullOrEmpty(dest))
                dest = filePath;

            base.Save(dest, codec, space);
        }
    }
}
