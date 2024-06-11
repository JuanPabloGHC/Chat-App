using Chat_App.Data;
using Chat_App.Data.Entities;
using CommunityToolkit.Maui.Views;
using System.IO;

namespace Chat_App.Views;

public partial class ChangePhoto : Popup
{
    User _user;
    public MemoryStream? _Mstream;

    public ChangePhoto(User u)
	{
		InitializeComponent();

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

    private void Save(object sender, EventArgs e)
    {
        // Update user data
        using (var db = new Context())
        {
            var u = db.Users
                .Where(u => u.Id == _user.Id)
                .First();

            // Photo ?
            if (SelectedImage.Source != null)
            {
                u.Photo = _Mstream.ToArray();
            }
            else
            {
                u.Photo = null;
            }

            db.SaveChanges();
        }

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