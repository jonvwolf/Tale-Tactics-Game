using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Models
{
    public class ReadImageModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public uint Width { get; set; }
        public uint Height { get; set; }
        public FileFormatEnum Format { get; set; }
        public string AbsoluteUrl { get; set; }
        public bool IsScanned { get; set; }
        public long Size { get; set; }
    }
}
