
using Chat_App.Data;
using Chat_App.Data.Entities;
using Chat_App.Functions;
using System.Diagnostics;

namespace Chat_App.Pages;

public partial class Signup : ContentPage
{
    // Photo
    public MemoryStream? _Mstream;
    private bool withPhoto = false;
    // Colors
    private string[] colors = { "#ff0000", "#00ff00", "#0000ff", "#ff00ff", "#00ffff", "#ffff00" };

    private string? userAccount;
    public Signup()
	{
		InitializeComponent();
	}
    private async void OnPickFileClicked(object sender, EventArgs e)
    {
        try
        {
            // Select a file from your dictory
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Select an image"
            });

            if (result != null)
            {
                // Save it as a MemoryStream to keep it opened
                if (result.FileName.EndsWith("jpg", StringComparison.OrdinalIgnoreCase) ||
                    result.FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase))
                {
                    var stream = await result.OpenReadAsync();

                    _Mstream = new MemoryStream();

                    await stream.CopyToAsync(_Mstream);
                    _Mstream.Position = 0;

                    // Show it and change the color status of the button
                    SelectedImage.Source = ImageSource.FromStream(() => _Mstream);

                    ImageButton.BorderColor = Colors.Green;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private void OnChecked(object sender, CheckedChangedEventArgs e)
	{
        // With photo?
		if (e.Value == false)
		{
            PhotoInput.IsVisible = false;
            withPhoto = false;
        }
		else
		{
			PhotoInput.IsVisible = true;
            withPhoto = true;
		}
	}

	private void SignUp(object sender, EventArgs e)
	{
        ErrorMessage.Text = "";

        Entry[] inputs = { Account, Name, LastName, Password, RepeatPassword };

        // Valid inputs?
        if (!CheckInputs.TextInputs(inputs))
        {
            return;
        }

        // Same password
        if (Password.Text != RepeatPassword.Text)
        {
            ErrorMessage.Text = "Check your password";
            return;
        }

        // Valid photo?
        if ( withPhoto && SelectedImage.Source is null)
        {
            ImageButton.BorderColor = Colors.Red;
            return;
        }

        using (var db = new Context())
        {
            var u = db.Users
                .Where(u => u.Account == Account.Text)
                .FirstOrDefault();

            // This account already exists
            if ( u != null )
            {
                ErrorMessage.Text = "Try another account";
                return;
            }

            // Create a new user
            var random  = new Random();
            int index = random.Next(colors.Length);
            userAccount = Account.Text;

            // Encrypt the password
            string passCryp = SecretHasher.Hash(Password.Text);

            // Create a new user
            User user = new User();
            user.Name = Name.Text;
            user.LastName = LastName.Text;
            user.Account = Account.Text;
            user.Password = passCryp;
            user.Status = true;
            if( withPhoto )
            {
                user.Photo = _Mstream?.ToArray();
            }
            else
            {
                user.Photo = null;
            }
            user.Color = colors[index];

            // Save user
            db.Add(user);
            db.SaveChanges();
        }

        // Go to main page
        App.Enter(userAccount);
	}

    private void ToLogin(object sender, EventArgs e)
    {
        App.Exit();
    }

}