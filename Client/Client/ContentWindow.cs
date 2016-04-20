using System.Windows.Controls;

namespace Client
{ 
    public enum ContNum : byte
    {
        StartImg,
        PlayersBoard
    };
    public static class MyContentClassWindow
    {
        public static ContentControl ChangeContent(ContNum nr)
        {
            switch (nr)
            {
                    case ContNum.StartImg:
                {
                    return new LogoPicture();
                    //break;
                }
                    case ContNum.PlayersBoard:
                {
                    return new PlayersTableManager();
                }
                default:
                {
                    return null;
                }
            }
        }
    }
}
