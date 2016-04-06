using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Client
{
    class ContentWindow
    {
        public ContentControl ChangeContent(byte nr)
        {
            switch (nr)
            {
                case 1:
                {
                    return new LogoPicture();
                    break;
                }
                default:
                {
                    return null;
                }
            }
        }
    }
}
