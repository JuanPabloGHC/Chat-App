using Chat_App.Pages;

namespace Chat_App
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new Login();
        }

        public static void Enter(string userAccount)
        {
            ((App)App.Current).MainPage = new MainPage(userAccount);
        }

        public static void Exit()
        {
            ((App)App.Current).MainPage = new Login();
        }

        public static void ToSignUp()
        {
            ((App)App.Current).MainPage = new Signup();
        }
    }
}
