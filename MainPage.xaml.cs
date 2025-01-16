using Chat_App.Views;
using Chat_App.Functions;
using Chat_App.Data.Entities;
using Chat_App.Data.Repository;
using CommunityToolkit.Maui.Views;

namespace Chat_App
{
    public partial class MainPage : ContentPage
    {
        #region < DATA MEMBERS >

        // Me
        private User? _user;

        // My photo
        private Stream? stream;

        // Selected friend chat
        private int idUserChat = -1;

        // Repositories
        private UserRepository userRepository;
        private MessageRepository messageRepository;
        private ChatRepository chatRepository;

        #endregion

        #region < CONSTRUCTORS >

        public MainPage(string userAccount)
        {
            InitializeComponent();

            // Initialize repositories
            userRepository = UserRepository.GetInstance();
            messageRepository = MessageRepository.GetInstance();
            chatRepository = ChatRepository.GetInstance();

            // Get User Data
            this.InitializeUser(userAccount);

        }

        #endregion

        #region < UI EVENTS >

        private async void OnMenuClicked(object sender, EventArgs e)
        {
            var action = await DisplayActionSheet("Menu", "Cancel", null, "Exit", "Change photo");

            switch (action)
            {
                case "Exit":
                    // Logout
                    await userRepository.ModifyStatus(_user.Id, false);

                    App.Exit();

                    break;
                case "Change photo":
                    var popup = new ChangePhoto(_user);

                    await this.ShowPopupAsync(popup);

                    await RefreshProfile();

                    break;
            }
        }

        private async void OnFilterChanged(object sender, TextChangedEventArgs e)
        {

            await LoadPage(Filter.Text);
        }

        private async void OnChatSelected(object? sender, EventArgs e)
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

            // Get user data
            User? u = await userRepository.GetUser(id);

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
            await SeenMessage();
            await LoadPage("");
        }

        private async void SendMessage(object sender, EventArgs e)
        {
            // Valid message?
            if (!CheckInputs.TextInputs([MessageInput]))
            {
                return;
            }

            // Get chat data
            int idChat;
            Chat? chatEx = await chatRepository.GetChat(_user.Id, idUserChat);

            // New chat
            if (chatEx == null)
            {
                Chat _chat = await chatRepository.Create(_user.Id, idUserChat);

                idChat = _chat.Id;
            }
            // Update the chat
            else
            {
                idChat = chatEx.Id;
                await chatRepository.ModifyStatus(chatEx.Id, _user.Id);

                //if (chatEx.User1Id == _user.Id)
                //{
                //    chatEx.Seen1 = true;
                //    chatEx.Seen2 = false;
                //}
                //else
                //{
                //    chatEx.Seen2 = true;
                //    chatEx.Seen1 = false;
                //}
            }
            // Create the new message
            await messageRepository.Create(_user.Id, idChat, MessageInput.Text, DateTime.Now, false);

            MessageInput.Text = "";

            await LoadPage("");

        }

        private async void OnChatOptionsClicked(object sender, EventArgs e)
        {
            var action = await DisplayActionSheet("Options", "Cancel", null, "Delete chat", "Close chat");

            switch (action)
            {
                case "Delete chat":
                    await chatRepository.Delete(_user.Id, idUserChat);

                    await LoadPage("");

                    break;
                case "Close chat":
                    idUserChat = -1;

                    ChatOpen.IsVisible = false;

                    LandingChat.IsVisible = true;

                    await LoadPage("");

                    break;
            }
        }

        #endregion

        #region < PRIVATE METHODS >

        private async Task InitializeUser(string userAccount)
        {
            _user = await userRepository.GetUser(userAccount);
            if (_user == null)
            {
                App.Exit();
            }

            this.UpdateProfilePhoto();

            // Profile account
            UserAccount.Text = string.Concat(_user?.Name, " ", _user?.LastName);

            await LoadPage("");
        }

        private async Task RefreshProfile()
        {
            _user = await userRepository.GetUser(_user.Id);

            this.UpdateProfilePhoto();
        }

        private void UpdateProfilePhoto()
        {
            // Profile photo?
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
        }

        private async Task RefreshContacts(string filter)
        {
            // Clear screen
            ChatsStack.Clear();

            List<User> usersList = await userRepository.GetAll(_user.Account, filter);

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
                //Message? lastMessage = db.Chats
                //    .Where(c => (c.User1Id == _user.Id && c.User2Id == u.Id) || (c.User1Id == u.Id && c.User2Id == _user.Id))
                //    .SelectMany(c => c.Messages)
                //    .OrderByDescending(m => m.Date)
                //    .FirstOrDefault();
                Message? lastMessage = await chatRepository.GetLastMessage(_user.Id, u.Id);
                string lastMessageDate = "";
                if (lastMessage != null)
                {
                    lastMessageDate = lastMessage?.Date.Value.ToString("HH:mm");
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
                    if (lastMessage?.Text?.Length > 38)
                    {
                        lastMessageText = lastMessage.Text.Substring(0, 35) + "...";
                    }
                    else
                    {
                        lastMessageText = lastMessage?.Text;
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
                Chat? chat = await chatRepository.GetChat(_user.Id, u.Id);
                
                if (chat != null)
                {
                    bool seen;
                    if (chat.User1Id == _user.Id)
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
        }

        private async Task RefreshChat()
        {
            // Friend selected?
            if (idUserChat != -1)
            {
                // Clear screen
                ChatMessages.Clear();

                Chat? chat = await chatRepository.GetChat(_user.Id, idUserChat);

                // Chat existst?
                if (chat != null)
                {
                    // Get all messages
                    List<Message> messagesList = await messageRepository.GetAll(chat.Id);

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
            
            ScrollToBottom();
        }

        private async Task SeenMessage()
        {
            // Get chat data
            Chat? chat = await chatRepository.GetChat(_user.Id, idUserChat);
                
            if (chat != null)
            {
                // Update status
                await chatRepository.SeenChat(chat.Id, _user.Id);
                //if (chat.User1Id == _user.Id)
                //{
                //    chat.Seen1 = true;
                //}
                //else
                //{
                //    chat.Seen2 = true;
                //}

                // Get all unseen messages
                await messageRepository.ModifyStatus(chat.Id, idUserChat);
            }
        }
        
        private async void ScrollToBottom()
        {
            await ChatScrollView.ScrollToAsync(0, ChatMessages.Height, true);
        }

        #endregion

        #region < PUBLIC METHODS >

        public async Task LoadPage(string filter)
        {

            await RefreshContacts(filter);

            await RefreshChat();

        }


        #endregion

    }

}