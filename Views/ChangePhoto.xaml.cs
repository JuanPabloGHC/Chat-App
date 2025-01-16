using Chat_App.Data.Entities;
using Chat_App.Data.Repository;
using CommunityToolkit.Maui.Views;

namespace Chat_App.Views;

public partial class ChangePhoto : Popup
{
    User _user;
    public MemoryStream? _Mstream;
    private UserRepository userRepository;

    public ChangePhoto(User u)
	{
		InitializeComponent();

        userRepository = UserRepository.GetInstance();

        _user = u;
        if (u.Photo != null)
        {
            _Mstream = new MemoryStream(u.Photo);
            SelectedImage.Source =ImageSource.FromStream(() => _Mstream);
        }
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

    private async void Save(object sender, EventArgs e)
    {
        // Update user data
        // Photo ?
        byte[]? photo = SelectedImage.Source != null ? _Mstream?.ToArray() : null;

        await userRepository.ModifyPhoto(_user.Id, photo);

        Close();
    }
    
    private void Delete(object sender, EventArgs e)
    {
        SelectedImage.Source = null;
    }

    private void Cancel(object sender, EventArgs e)
    {
        Close();
    }

}