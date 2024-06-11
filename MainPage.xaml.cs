using Chat_App.Data;
using Chat_App.Data.Entities;
using Chat_App.Functions;
using Chat_App.Views;
using CommunityToolkit.Maui.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel;
using System.Diagnostics;
namespace Chat_App
{
    public partial class MainPage : ContentPage
    {
        // Me
        private User? _user;

        // My photo
        private Stream? stream;

        // Selected friend chat
        private int idUserChat = -1;

        public MainPage(string userAccount)
        {
            InitializeComponent();

            // Get User Data
            using (var db = new Context())
            {
                _user = db.Users
                    .Where(u => u.Account == userAccount)
                    .FirstOrDefault();
               if(_user == null)
                {
                    App.Exit();
                }
            }
            
            // Profile photo
            if (_user?.Photo != null)
            {
                stream = new MemoryStream(_user.Photo);
                ProfileImageButton.IsVisible = true;
                ProfileButton.IsVisible = false;
                ProfileImageButton.Source = ImageSource.FromStream(() => stream);
            }
            else
            {
                ProfileImageButton.IsVisible = false;
                ProfileButton.IsVisible = true;
                ProfileButton.Text = string.Concat(_user?.Name?[0], _user?.LastName?[0]);
                ProfileButton.BackgroundColor = Color.FromArgb(_user?.Color);
            }

            // Profile account
            UserAccount.Text = string.Concat(_user.Name, " ", _user.LastName);

            LoadPage("");

        }

        private void RefreshProfile()
        {
            using (var db = new Context())
            {
                var _u = db.Users
                    .Where(u => u.Id == _user.Id)
                    .First();

                _user = _u;

                // Profile photo?
                if (_u.Photo != null)
                {
                    stream = new MemoryStream(_u.Photo);
                    ProfileImageButton.IsVisible = true;
                    ProfileButton.IsVisible = false;
                    ProfileImageButton.Source = ImageSource.FromStream(() => stream);
                }
                else
                {
                    ProfileImageButton.IsVisible = false;
                    ProfileButton.IsVisible = true;
                    ProfileButton.Text = string.Concat(_u.Name?[0], _u.LastName?[0]);
                    ProfileButton.BackgroundColor = Color.FromArgb(_u.Color);
                }

            }
        }

        private void RefreshContacts(string filter)
        {
            // Clear screen
            ChatsStack.Clear();

            using (var db = new Context())
            {
                User[] usersList = [];
                // All users but you
                if (filter == "")
                {
                    usersList = db.Users
                        .Where(u => u.Account != _user.Account)
                        .ToArray();
                }
                // Using the filter to search
                else
                {
                    usersList = db.Users
                        .Where(u => u.Account != _user.Account)
                        .Where(u => u.Name.ToLower().Contains(filter) || u.LastName.ToLower().Contains(filter))
                        .ToArray();
                }
                
                foreach (var u in usersList)
                {
                    // Frame of the contact
                    Frame frame = new Frame
                    {
                        BorderColor = Colors.Transparent,
                        BackgroundColor = Color.FromArgb("#eefdfd"),
                        HeightRequest = 100,
                        CornerRadius = 0
                    };

                    Grid grid = new Grid
                    {
                        ColumnDefinitions =
                        {
                            new ColumnDefinition { Width = new GridLength(60) },
                            new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                            new ColumnDefinition { Width = new GridLength(30) }
                        },
                        RowDefinitions =
                        {
                            new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                            new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
                        }
                    };

                    // Photo
                    if (u.Photo != null)
                    {
                        MemoryStream _st = new MemoryStream(u.Photo);
                        ImageButton UserButton = new ImageButton
                        {
                            Source = ImageSource.FromStream(() => _st),
                            BorderColor = Color.FromArgb("#1a2c32"),
                            BorderWidth = 2,
                            CornerRadius = 50,
                            WidthRequest = 55,
                            HeightRequest = 55,
                            VerticalOptions = LayoutOptions.Center,
                            HorizontalOptions = LayoutOptions.Center,
                            AutomationId = u.Id.ToString()
                        };
                        UserButton.Clicked += OnChatSelected;
                        Grid.SetColumn(UserButton, 0);
                        Grid.SetRowSpan(UserButton, 2);
                        grid.Children.Add(UserButton);
                    }
                    else
                    {
                        Button UserButton = new Button
                        {
                            Text = string.Concat(u?.Name?[0], u?.LastName?[0]),
                            TextColor = Color.FromArgb("#1a2c32"),
                            FontSize = 10,
                            BackgroundColor = Color.FromArgb(u?.Color),
                            BorderColor = Color.FromArgb("#1a2c32"),
                            BorderWidth = 2,
                            CornerRadius = 50,
                            WidthRequest = 55,
                            HeightRequest = 55,
                            VerticalOptions = LayoutOptions.Center,
                            HorizontalOptions = LayoutOptions.Center,
                            AutomationId = u.Id.ToString()
                        };
                        UserButton.Clicked += OnChatSelected;
                        Grid.SetColumn(UserButton, 0);
                        Grid.SetRowSpan(UserButton, 2);
                        grid.Children.Add(UserButton);
                    }

                    // Name
                    Label nameLabel = new Label
                    {
                        Text = string.Concat(u.Name, " ", u.LastName),
                        FontSize = 16,
                        TextColor = Color.FromArgb("#1a2c32"),
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Start,
                        Margin = new Thickness(15, 0, 0, 0)
                    };
                    Grid.SetColumn(nameLabel, 1);
                    Grid.SetRow(nameLabel, 0);
                    grid.Children.Add(nameLabel);

                    // Last messagge time
                    Message? lastMessage = db.Chats
                        .Where(c => (c.User1Id == _user.Id && c.User2Id == u.Id) || (c.User1Id == u.Id && c.User2Id == _user.Id))
                        .SelectMany(c => c.Messages)
                        .OrderByDescending(m => m.Date)
                        .FirstOrDefault();
                    string lastMessageDate = "";
                    if (lastMessage != null)
                    {
                        lastMessageDate = lastMessage.Date.Value.ToString("HH:mm");
                    }
                    Label timeLabel = new Label
                    {
                        Text = lastMessageDate,
                        FontSize = 10,
                        TextColor = Color.FromArgb("#1a2c32"),
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center
                    };
                    Grid.SetColumn(timeLabel, 2);
                    Grid.SetRow(timeLabel, 0);
                    grid.Children.Add(timeLabel);

                    // Last message text
                    //Limite de caracteres: 38
                    //si es mayor de 38 entonces: 35 + ...
                    string lastMessageText = "";
                    if (lastMessage != null)
                    {
                        if (lastMessage.Text.Length > 38)
                        {
                            lastMessageText = lastMessage.Text.Substring(0,35) + "...";
                        }
                        else
                        {
                            lastMessageText = lastMessage.Text;
                        }
                    }
                    Label lastMessageLabel = new Label
                    {
                        Text = lastMessageText,
                        FontSize = 12,
                        TextColor = Color.FromArgb("#7e9192"),
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Start,
                        Margin = new Thickness(15, 0, 0, 0)
                    };
                    Grid.SetColumn(lastMessageLabel, 1);
                    Grid.SetRow(lastMessageLabel, 1);
                    grid.Children.Add(lastMessageLabel);

                    // Status of the message
                    var chat = db.Chats
                        .Where(c => (c.User1Id == _user.Id && c.User2Id == u.Id) || (c.User1Id == u.Id && c.User2Id == _user.Id))
                        .FirstOrDefault();
                    if (chat != null)
                    {
                        bool seen;
                        if(chat.User1Id == _user.Id)
                        {
                            seen = chat.Seen1;
                        }
                        else
                        {
                            seen = chat.Seen2;
                        }
                        Frame statusFrame = new Frame
                        {
                            BackgroundColor = seen ? Colors.Transparent : Color.FromArgb("#38cbd8"),
                            BorderColor = Colors.Transparent,
                            WidthRequest = 12,
                            HeightRequest = 12,
                            VerticalOptions = LayoutOptions.Center,
                            HorizontalOptions = LayoutOptions.Center
                        };
                        Grid.SetColumn(statusFrame, 2);
                        Grid.SetRow(statusFrame, 1);
                        grid.Children.Add(statusFrame);
                    }

                    frame.Content = grid;

                    ChatsStack.Add(frame);

                }//end foreach users
            }//end using db
        }

        private void RefreshChat()
        {
            // Friend selected?
            if (idUserChat != -1)
            {
                // Clear screen
                ChatMessages.Clear();
                using (var db = new Context())
                {
                    // Get Chat with a friend
                    var chat = db.Chats
                        .Where(c => (c.User1Id == _user.Id && c.User2Id == idUserChat) ||
                        (c.User1Id == idUserChat && c.User2Id == _user.Id))
                        .FirstOrDefault();

                    // Chat existst?
                    if (chat != null)
                    {
                        // Get all messages
                        Message[] messagesList = db.Messages
                            .Where(m => m.ChatId == chat.Id)
                            .OrderBy(m => m.Date)
                            .ToArray();

                        foreach (var message in messagesList)
                        {
                            // Frame of the message
                            Frame frame = new Frame
                            {
                                BorderColor = Colors.Transparent,
                                BackgroundColor = message.UserId == _user.Id ? Color.FromArgb("#5faab1") : Color.FromArgb("#7e9192"),
                                MinimumHeightRequest = 80,
                                MinimumWidthRequest = 0,
                                MaximumWidthRequest = 500,
                                CornerRadius = 20,
                                HorizontalOptions = message.UserId == _user.Id ? LayoutOptions.End : LayoutOptions.Start,
                                Margin = 20,
                            };

                            Grid grid = new Grid
                            {
                                ColumnDefinitions =
                                {
                                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                                    new ColumnDefinition { Width = new GridLength(15) }
                                },
                                RowDefinitions =
                                {
                                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                                    new RowDefinition { Height = new GridLength(20) }
                                }
                            };

                            // Text message
                            Label text = new Label
                            {
                                Text = message.Text,
                                TextColor = Color.FromArgb("#1a2c32"),
                                HorizontalOptions = LayoutOptions.Start,
                                VerticalOptions = LayoutOptions.Center
                            };
                            grid.SetColumnSpan(text, 2);
                            grid.SetRow(text, 0);
                            grid.Add(text);

                            // Time of the message
                            DateTime date = message.Date.Value;
                            Label time = new Label
                            {
                                Text = date.ToString("HH:mm"),
                                TextColor = Color.FromArgb("#1a2c32"),
                                HorizontalOptions = LayoutOptions.End,
                                VerticalOptions = LayoutOptions.Center
                            };
                            grid.SetColumn(time, 0);
                            grid.SetRow(time, 1);
                            grid.Add(time);

                            // Status of the message
                            Frame status = new Frame
                            {
                                BorderColor = Colors.Blue,
                                BackgroundColor = message.Status ? Colors.Blue : Colors.Transparent,
                                VerticalOptions = LayoutOptions.Center,
                                MinimumWidthRequest = 10,
                                MaximumWidthRequest = 10,
                                MinimumHeightRequest = 10,
                                MaximumHeightRequest = 10,
                                CornerRadius = 5
                            };
                            grid.SetColumn(status, 1);
                            grid.SetRow(status, 1);
                            grid.Add(status);

                            frame.Content = grid;

                            ChatMessages.Add(frame);
                        }
                    }

                }

            }
            
            ScrollToBottom();
        }

        public void LoadPage(string filter)
        {

            RefreshContacts(filter);

            RefreshChat();

        }

        private async void OnMenuClicked(object sender, EventArgs e)
        {
            var action = await DisplayActionSheet("Menu", "Cancel", null, "Exit", "Change photo");

            switch (action)
            {
                case "Exit":
                    using (var db = new Context())
                    {
                        var u = db.Users
                            .Where(u => u.Id == _user.Id)
                            .First();

                        u.Status = false;
                        db.SaveChanges();
                    }
                    App.Exit();
                    break;
                case "Change photo":
                    var popup = new ChangePhoto(_user);
                    await this.ShowPopupAsync(popup);
                    RefreshProfile();
                    break;
            }
        }

        private void OnFilterChanged(object sender, TextChangedEventArgs e)
        {

            LoadPage(Filter.Text);
        }

        private void OnChatSelected(object? sender, EventArgs e)
        {
            // Photo / button of user is selected
            int id = -1;
            ImageButton b = sender as ImageButton;

            if (b == null)
            {
                Button b2 = sender as Button;
                id = Convert.ToInt32(b2.AutomationId);
            }
            else
            {
                id = Convert.ToInt32(b.AutomationId);
            }

            using (var db = new Context())
            {
                // Get user data
                var u = db.Users
                    .Where(u => u.Id == id)
                    .FirstOrDefault();

                // Update the current the friend chat
                idUserChat = u.Id;

                if (u != null)
                {
                    // Photo?
                    if (u.Photo != null)
                    {
                        ChatButtonProfile.IsVisible = false;
                        ChatButtonImage.IsVisible = true;
                        MemoryStream s = new MemoryStream(u.Photo);
                        ChatButtonImage.Source = ImageSource.FromStream(() => s);
                    }
                    else
                    {
                        ChatButtonImage.IsVisible = false;
                        ChatButtonProfile.IsVisible = true;
                        ChatButtonProfile.BackgroundColor = Color.FromArgb(u.Color);
                        ChatButtonProfile.Text = string.Concat(u?.Name?[0], u?.LastName?[0]);
                    }

                    // Friend name
                    ChatNameProfile.Text = string.Concat(u?.Name, " ", u?.LastName);

                    // Friend status
                    ChatStatusProfile.BackgroundColor = u.Status ? Colors.Green : Colors.Gray;
                    ChatStatusProfile.BorderColor = u.Status ? Colors.Green : Colors.Gray;

                    // Update screen
                    LandingChat.IsVisible = false;
                    ChatOpen.IsVisible = true;
                }
            }
            SeenMessage();
            LoadPage("");
        }

        private void SendMessage(object sender, EventArgs e)
        {
            // Valid message?
            if (!CheckInputs.TextInputs([MessageInput]))
            {
                return;
            }

            using (var db = new Context())
            {
                // Get chat data
                int idChat;
                var chatEx = db.Chats
                    .Where(c => (c.User1Id == _user.Id && c.User2Id == idUserChat) ||
                    (c.User1Id == idUserChat && c.User2Id == _user.Id))
                    .FirstOrDefault();

                // New chat
                if ( chatEx == null)
                {
                    Chat _chat = new Chat();
                    _chat.User1Id = _user.Id;
                    _chat.User2Id = idUserChat;
                    _chat.Seen1 = true;
                    _chat.Seen2 = false;

                    db.Chats.Add(_chat);
                    db.SaveChanges();
                    idChat = db.Chats.Count();
                }
                // Update the chat
                else
                {
                    idChat = chatEx.Id;
                    if (chatEx.User1Id == _user.Id)
                    {
                        chatEx.Seen1 = true;
                        chatEx.Seen2 = false;
                    }
                    else
                    {
                        chatEx.Seen2 = true;
                        chatEx.Seen1 = false;
                    }
                }
                // Create the new message
                Message _message = new Message();
                _message.UserId = _user.Id;
                _message.ChatId = idChat;
                _message.Text = MessageInput.Text;
                _message.Date = DateTime.Now;
                _message.Status = false;

                db.Messages.Add(_message);
                db.SaveChanges();

                MessageInput.Text = "";
                
                LoadPage("");
            }

        }

        private void SeenMessage()
        {
            // Get messages
            using (var db = new Context())
            {
                // Get chat data
                var chat = db.Chats
                    .Where(c => (c.User1Id == _user.Id && c.User2Id == idUserChat) || (c.User1Id == idUserChat && c.User2Id == _user.Id))
                    .FirstOrDefault();
                if (chat != null)
                {
                    // Update status
                    if (chat.User1Id == _user.Id)
                    {
                        chat.Seen1 = true;
                    }
                    else
                    {
                        chat.Seen2 = true;
                    }

                    // Get all unseen messages
                    var messages = db.Messages
                        .Where(m => m.ChatId == chat.Id)
                        .Where(m => m.UserId == idUserChat && m.Status == false)
                        .ToArray();

                    // Update status of each message
                    foreach (var message in messages)
                    {
                        message.Status = true;
                    }

                    db.SaveChanges();
                }
            }
        }
        
        private async void ScrollToBottom()
        {
            await ChatScrollView.ScrollToAsync(0, ChatMessages.Height, true);
        }

        private async void OnChatOptionsClicked(object sender, EventArgs e)
        {
            var action = await DisplayActionSheet("Options", "Cancel", null, "Delete chat", "Close chat");

            switch (action)
            {
                case "Delete chat":
                    using (var db = new Context())
                    {
                        var chat = db.Chats
                            .Where(c => (c.User1Id == _user.Id && c.User2Id == idUserChat) ||
                            (c.User1Id == idUserChat && c.User2Id == _user.Id))
                            .Include("Messages")
                            .First();

                        foreach(var m in chat.Messages)
                        {
                            db.Remove(m);
                        }
                        db.SaveChanges();
                    }
                    LoadPage("");
                    break;
                case "Close chat":
                    idUserChat = -1;
                    ChatOpen.IsVisible = false;
                    LandingChat.IsVisible = true;
                    LoadPage("");
                    break;
            }
        }
    }

}
