using System.Windows.Controls;

namespace Client
{ 
    public enum ContNum : byte
    {
        StartImg,
        PlayersBoard,
        StartPanel,
    };
    public static class MyContentClassWindow
    {
        public static ContentControl ChangeContent(ContNum nr, byte numOfPlayers=2)
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
                    return new PlayersTableManager(numOfPlayers);
                }
                case ContNum.StartPanel:
                {
                    return new UCMainScreen();
                }
                default:
                {
                    return null;
                }
            }
        }
    }
}
