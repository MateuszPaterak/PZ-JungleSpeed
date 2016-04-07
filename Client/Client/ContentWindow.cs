using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Client
{ 
    public enum ContNum : byte
    {
        StartImg,
        Win2Player
    };
    public class ContentWindow
    {
        public ContentControl ChangeContent(ContNum nr)
        {
            switch (nr)
            {
                    case ContNum.StartImg:
                {
                    return new LogoPicture();
                    //break;
                }
                    case ContNum.Win2Player:
                {
                    return new Win2Player();
                }
                default:
                {
                    return null;
                }
            }
        }
    }
}
