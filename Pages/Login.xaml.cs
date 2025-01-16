using Chat_App.Functions;
using Chat_App.Data.Repository;

namespace Chat_App.Pages;

public partial class Login : ContentPage
{
    UserRepository userRepository;

	public Login()
	{
		InitializeComponent();

        this.userRepository = UserRepository.GetInstance();
	}

	private async void LogIn(object sender, EventArgs e)
	{
        ErrorMessage.Text = "";

        Entry[] inputs = { Account, Password };

        //Valid inputs?
        if (!CheckInputs.TextInputs(inputs))
        {
            return;
        }

        // Verify encrypted password
        bool log = await userRepository.Login(Account.Text, Password.Text);

        if(log)
        {
            App.Enter(Account.Text);
        }
        else
        {
            ErrorMessage.Text = "NOT FOUND";
        }
	}

	private void ToSignup(object sender, EventArgs e)
	{
		App.ToSignUp();
	}

}