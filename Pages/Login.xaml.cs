using Chat_App.Functions;
using Chat_App.Data.Repository;

namespace Chat_App.Pages;

public partial class Login : ContentPage
{
    #region < DATA MEMBERS >

    UserRepository userRepository;

    #endregion

    #region < CONSTRUCTORS >

    public Login()
	{
		InitializeComponent();

        this.userRepository = UserRepository.GetInstance();
	}

    #endregion

    #region < UI EVENTS >

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

    #endregion

}