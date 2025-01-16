
using Chat_App.Functions;
using Chat_App.Data.Entities;
using Chat_App.Data.Repository;

namespace Chat_App.Pages;

public partial class Signup : ContentPage
{
    #region < DATA MEMBERS >

    // Photo
    public MemoryStream? _Mstream;
    private bool withPhoto = false;
    // Colors
    private string[] colors = { "#ff0000", "#00ff00", "#0000ff", "#ff00ff", "#00ffff", "#ffff00" };

    private string? userAccount;

    private UserRepository userRepository;

    #endregion

    #region < CONSTRUCTORS >

    public Signup()
	{
		InitializeComponent();

        this.userRepository = UserRepository.GetInstance();
	}

    #endregion

    #region < UI EVENTS >

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

	private async void SignUp(object sender, EventArgs e)
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

        User? u = await userRepository.GetUser(Account.Text);
            
        // This account already exists
        if ( u != null )
        {
            ErrorMessage.Text = "Try another account";
            return;
        }

        // Create a new user
        var random  = new Random();

        int index = random.Next(colors.Length);

        byte[]? photo = withPhoto ? _Mstream?.ToArray() : null;

        User user = await userRepository.Create(Name.Text, LastName.Text, Account.Text, Password.Text, colors[index], photo);

        // Go to main page
        App.Enter(user?.Account);
	}

    private void ToLogin(object sender, EventArgs e)
    {
        App.Exit();
    }

    #endregion

}