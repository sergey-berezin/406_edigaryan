using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF2
{
    public class ImageEntry
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public string Hash { get; set; }
        public byte[] Embedding { get; set; }
        public ImageData Details { get; set; }
    }
}
