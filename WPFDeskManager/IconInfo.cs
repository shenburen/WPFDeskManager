using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WPFDeskManager
{
    public class IconInfo
    {
        public required string Name { get; set; }

        public required string TargetPath { get; set; }

        public required BitmapSource Icon { get; set; }
    }
}
