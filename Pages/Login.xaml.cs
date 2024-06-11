using Chat_App.Data;
using Chat_App.Functions;

namespace Chat_App.Pages;

public partial class Login : ContentPage
{
	public Login()
	{
		InitializeComponent();
	}

	private void LogIn(object sender, EventArgs e)
	{
        ErrorMessage.Text = "";

        Entry[] inputs = { Account, Password };

        //Valid inputs?
        if (!CheckInputs.TextInputs(inputs))
        {
            return;
        }

        using(var db = new Context())
        {
            // Get user data
            var _user = db.Users
                .Where(u => u.Account == Account.Text)
                .FirstOrDefault();

            if (_user != null)
            {
                // Verify encrypted password
                if(SecretHasher.Verify(Password.Text, _user.Password))
                {
                    _user.Status = true;
                    db.SaveChanges();

                    App.Enter(Account.Text);
                }
            }

            ErrorMessage.Text = "NOT FOUND";
        }

	}

	private void ToSignup(object sender, EventArgs e)
	{
		App.ToSignUp();
	}

}