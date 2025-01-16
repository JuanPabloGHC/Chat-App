# Chat-App

## Author
[Juan Pablo Gómez Haro Cabrera](https://github.com/JuanPabloGHC)

## Description
This application helps to communicate with others, is based on WhatsApp or Telegram, you have your contacts, messages, profile information.
In this MVP app, you can chat with everyone who has an account, you do not have to add them as a friends.

## Technologies
* [C#](https://learn.microsoft.com/en-us/dotnet/csharp/)
* [.NET MAUI](https://dotnet.microsoft.com/en-us/apps/maui)
* [Entity Framework](https://learn.microsoft.com/es-es/ef/)
* [SQL SERVER](https://www.microsoft.com/es-MX/sql-server)

## Tools
[Colors Generator](https://uicolors.app/create)

* Blue / Green: #438e96
* Gray: #7e9192
* Aqua: #1cabba

[.NET Community Toolkit](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/introduction)
* Popup

## Entities

### Users

* SQL
  
|          Id         |         Name         |        LastName      |        Account       |        Password       |    Status    |      Photo     |    Color   |
|---------------------|----------------------|----------------------|----------------------|-----------------------|--------------|----------------|------------|
| 🔑INTEGER NOT NULL | VARCHAR(30) NOT NULL | VARCHAR(30) NOT NULL | VARCHAR(20) NOT NULL | VARCHAR(MAX) NOT NULL | BIT NOT NULL | VARBINARY(MAX) | VARCHAR(8) |

* Code

```C#
[Table("Users")]
Class User {
  [Key]
  int Id { get; set; }
  string Name { get; set; }
  string LastName { get; set; }
  string Account {  get; set; }
  string Password { get; set; }
  bool Status { get; set; }
  byte[] Photo { get; set; }
  string Color { get; set; }
}
```

### Chats

* SQL
  
|          Id         |       User1Id       |       User2Id       |     Seen1    |     Seen2    |
|---------------------|---------------------|---------------------|--------------|--------------|
| 🔑INTEGER NOT NULL | FK INTEGER NOT NULL | FK INTEGER NOT NULL | BIT NOT NULL | BIT NOT NULL |

* Code

```C#
[Table("Chats")]
public class Chat
{
    [Key]
    int Id { get; set; }
    int User1Id { get; set; }
    [ForeignKey("User1Id")]
    User User1 { get; set; }
    int User2Id { get; set; }
    [ForeignKey("User2Id")]
    User User2 { get; set; }
    bool Seen1 { get; set; }
    bool Seen2 { get; set; }
    ICollection<Message> Messages { get; set; }
}
```

### Messages

* SQL
  
|          Id         |        UserId       |        ChatId       |      Text     |        Date       |    Status    |
|---------------------|---------------------|---------------------|---------------|-------------------|--------------|
| 🔑INTEGER NOT NULL | FK INTEGER NOT NULL | FK INTEGER NOT NULL | TEXT NOT NULL | DATETIME NOT NULL | BIT NOT NULL |

* Code

```C#
[Table("Messages")]
public class Message
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }
    [ForeignKey("UserId")]
    public User? User { get; set; }

    public int ChatId { get; set; }
    [ForeignKey("ChatId")]
    public Chat? Chat { get; set; }

    public string? Text { get; set; }

    public DateTime? Date { get; set; }

    public bool Status { get; set; }
}
```

## SQL commands

### Create DB
```SQL
CREATE DATABASE <DATABASE_NAME>;
```

### Create Users Table
```SQL
CREATE TABLE Users (
	Id INTEGER IDENTITY(1, 1) NOT NULL,
	Name VARCHAR(30) NOT NULL,
	LastName VARCHAR(30) NOT NULL,
	Account VARCHAR(20) NOT NULL,
	Password VARCHAR(MAX) NOT NULL,
	Status BIT NOT NULL,
	Photo VARBINARY(MAX),
	Color VARCHAR(8),

	PRIMARY KEY(Id)
)
```

### Create Chats Table
```SQL
CREATE TABLE Chats (
	Id INTEGER IDENTITY(1, 1) NOT NULL,
	User1Id INTEGER NOT NULL,
	User2Id INTEGER NOT NULL,
	Seen1 BIT NOT NULL,
	Seen2 BIT NOT NULL,

	PRIMARY KEY(Id),
	FOREIGN KEY(User1Id) REFERENCES Users(Id),
	FOREIGN KEY(User2Id) REFERENCES Users(Id)
)
```

### Create Messages Table
```SQL
CREATE TABLE Messages (
	Id INTEGER IDENTITY(1, 1) NOT NULL,
	UserId INTEGER NOT NULL,
	ChatId INTEGER NOT NULL,
	Text TEXT NOT NULL,
	Date DATETIME NOT NULL,
	Status BIT NOT NULL,

	PRIMARY KEY(Id),
	FOREIGN KEY (UserId) REFERENCES Users(Id),
	FOREIGN KEY (ChatId) REFERENCES Chats(Id)
)
```

## Connect Entity Framework to the DB
```C#
class Context : DbContext {
    public DbSet<User> Users { get; set; }

    public DbSet<Chat> Chats { get; set; }

    public DbSet<Message> Messages { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=<HOST>;Database=<BASEDATOS>;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true");
    }
}
```

## Architecture

```plain
└──📁/Chat-App
   ├──📁/Data
   │   ├──📁/Entities
   │   │  ├──📄Chat.cs
   │   │  │──📄Message.cs
   │   │  └──📄User.cs
   │   ├──📁/Interfaces
   │   │  ├──📄IChatRepository.cs
   │   │  │──📄IMessageRepository.cs
   │   │  └──📄IUserRepository.cs
   │   ├──📁/Repository
   │   │  ├──📄ChatRepository.cs
   │   │  │──📄MessageRepository.cs
   │   │  └──📄UserRepository.cs
   │   └──📄Context.cs
   │
   ├──📁/Functions
   │  ├──📄CheckInput.cs
   │  └──📄SecretHasher.cs
   |
   ├──📁/Pages
   │  ├──📄Login.xaml
   │  |   └──📄Login.xaml.cs
   │  └──📄Signup.xaml
   │      └──📄Signup.xaml.cs
   |
   ├──📁/Views
   │  └──📄ChangePhoto.xaml
   │      └──📄ChangePhoto.xaml.cs
   |
   ├──📄App.xaml
   |   └──📄App.xaml.cs
   ├──📄MainPage.xaml
   |   └──📄MainPage.xaml.cs
   └──📄MauiProgram.cs
```

* Folder Data/Entities: Save the structures of the tables and the connection to the database.
* Folder Data/Interfaces: Define the interface of the Repository Design Pattern.
* Folder Data/Repository: Manage the data in the data base of each entity. 
* Folder Functions: General functions that are used in many parts of the project like the validation of inputs.
* Folder Pages: Auth pages: Login and Signup.
* Folder Views: To create the Popups views.
* App: To charge the sources.
* MainPage: Here is the application, chats, contacts, profile data.
* MauiProgram: To build the app, including Maui framework, community toolkit component and connection to the database.

* .xaml: The page structure.
* .xaml.cs: The page logic .
