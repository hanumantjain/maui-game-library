# Game Library - MAUI Application

A cross-platform game management application built with .NET MAUI that allows users to add, view, edit, and delete games with images, descriptions, genres, prices, and release dates.

## Features

✅ **Responsive UI** - Works on Desktop (macCatalyst), iOS, and Android
✅ **Game Management** - Add, view, edit, and delete games
✅ **Image Support** - Upload and display game images (base64 encoded)
✅ **Genre Selection** - Assign and filter games by genre
✅ **Real-time Updates** - Automatic list refresh after adding/deleting games
✅ **Data Validation** - Client-side and server-side validation
✅ **Confirmation Dialogs** - Delete confirmation to prevent accidental removal

## Project Structure

```
MauiApp1/
├── Client/                          # MAUI Frontend
│   ├── Views/
│   │   ├── Desktop/
│   │   │   ├── DesktopHomePage.xaml/.xaml.cs       # Desktop home with 3-column grid
│   │   │   ├── AddProductPage.xaml/.xaml.cs        # Add game form
│   │   │   ├── EditGamePage.xaml/.xaml.cs          # Edit game form
│   │   │   └── GameDetailPage.xaml/.xaml.cs        # Game detail view
│   │   └── Phone/
│   │       ├── PhoneHomePage.xaml/.xaml.cs         # Mobile home (vertical list)
│   │       ├── PhoneAddProductPage.xaml/.xaml.cs   # Mobile add form
│   │       ├── PhoneEditGamePage.xaml/.xaml.cs     # Mobile edit form
│   │       └── PhoneGameDetailPage.xaml/.xaml.cs   # Mobile detail view
│   ├── ViewModels/
│   │   ├── DesktopHomePageViewModel.cs
│   │   ├── PhoneHomePageViewModel.cs
│   │   ├── AddProductPageViewModel.cs
│   │   └── BaseViewModel.cs
│   ├── Services/
│   │   ├── IProductService.cs
│   │   └── ProductService.cs                        # HTTP client for API
│   ├── Models/
│   │   ├── Products.cs
│   │   ├── ProductTem.cs                            # Display model
│   │   └── Genre.cs
│   ├── Messages/
│   │   └── GameDeletedMessage.cs                    # Messaging for list refresh
│   ├── App.xaml/.xaml.cs
│   ├── AppShell.xaml
│   ├── MauiProgram.cs                               # DI configuration
│   └── Properties/launchSettings.json
│
├── GameLibrary/                     # Shared Models
│   ├── Models/
│   │   ├── Games.cs
│   │   └── Genre.cs
│   └── Responses/
│       └── ServiceResponse.cs
│
└── WebApplication1/                 # ASP.NET Core Backend
    ├── Controllers/
    │   └── GamesController.cs
    ├── Services/
    │   ├── IGameService.cs
    │   ├── IGenreService.cs
    │   ├── GameService.cs
    │   └── GenreService.cs
    ├── Data/
    │   ├── AppDbContext.cs
    │   └── Migrations/
    └── Program.cs
```

## Technology Stack

### Frontend
- **.NET MAUI** - Cross-platform mobile and desktop framework
- **CommunityToolkit.Mvvm** - MVVM patterns and messaging
- **Syncfusion Maui** - UI controls (SfTextInputLayout, SfComboBox, Calendar)
- **CommunityToolkit.Maui** - Toast notifications and alerts

### Backend
- **ASP.NET Core** - Web API framework
- **Entity Framework Core** - ORM for database access
- **SQL Server** - Database

## Getting Started

### Prerequisites
- .NET 10 SDK
- Visual Studio Code or Visual Studio
- Xcode (for macCatalyst/iOS development)
- Android SDK (for Android development)

### Installation

1. **Clone the repository**
```bash
git clone https://github.com/yourusername/game-library.git
cd MauiApp1
```

2. **Install dependencies**
```bash
dotnet restore
```

3. **Configure database connection** (in WebApplication1/appsettings.json)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=GameLibrary;Trusted_Connection=true;"
  }
}
```

4. **Run migrations**
```bash
cd WebApplication1
dotnet ef database update
cd ..
```

### Running the Application

#### macCatalyst (Desktop)
```bash
dotnet run -f net10.0-maccatalyst --project Client/Client.csproj
```

#### iOS Simulator
```bash
dotnet run -f net10.0-ios --project Client/Client.csproj
```
*Note: Requires Apple Developer account and code signing certificates*

#### Android Emulator
```bash
dotnet run -f net10.0-android --project Client/Client.csproj
```

#### Backend Server
```bash
cd WebApplication1
dotnet run
# Server will run on http://localhost:5068
```

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/Games` | Get all games |
| GET | `/api/Games/{id}` | Get game by ID |
| POST | `/api/Games` | Create new game |
| PUT | `/api/Games/{id}` | Update game |
| DELETE | `/api/Games/{id}` | Delete game |
| GET | `/api/Games/genre` | Get all genres |

## Usage

### Add a Game
1. Click "Add Games" in the flyout menu
2. Fill in game details (name, description, genre, price, release date)
3. Click "Add Image" to select an image
4. Click "Add Game" to save
5. App automatically navigates to homepage with the new game displayed

### View Game Details
1. Click on any game in the homepage grid/list
2. View full details including image, description, genre, price, and release date
3. Click "Edit" or "Delete" buttons as needed

### Edit a Game
1. Open game details
2. Click "Edit" button
3. Modify any fields
4. Click "Save" to apply changes
5. Returns to detail view

### Delete a Game
1. Open game details
2. Click "Delete" button
3. Confirm deletion in dialog
4. Game is removed and list automatically refreshes

## Architecture

### MVVM Pattern
- **Views** - XAML UI pages (DesktopHomePage, PhoneHomePage, etc.)
- **ViewModels** - Business logic and data binding (DesktopHomePageViewModel, AddProductPageViewModel, etc.)
- **Models** - Data structures (Products, Genre, ProductTem, etc.)
- **Services** - API communication (ProductService, IProductService)

### Messaging
- **WeakReferenceMessenger** - Loose coupling between ViewModels
- **GameDeletedMessage** - Notifies homepage to refresh when a game is deleted

### Dependency Injection
- Configured in **MauiProgram.cs**
- Services registered and resolved throughout the app
- DI container exposed on App instance for view-level access

## Building for Production

### Create Release Build
```bash
dotnet build -c Release -f net10.0-maccatalyst
```

### Publish App
```bash
dotnet publish -c Release -f net10.0-maccatalyst
```

## Troubleshooting

### iOS Code Signing Error
- Requires Apple Developer account
- Set up certificates in Xcode: Xcode → Preferences → Accounts
- Create iOS Development certificate

### Database Connection Issues
- Ensure SQL Server is running on localhost:1433
- Check connection string in appsettings.json
- Run migrations: `dotnet ef database update`

### API Connection Issues
- Ensure backend is running: `dotnet run` in WebApplication1
- Check that API URL is `http://localhost:5068`
- Verify CORS is enabled in backend

## File Handling

- **Images** are stored as base64 strings in the database
- **Max file size**: 5 MB
- **Supported formats**: PNG, JPG, JPEG
- **FilePicker timeout**: 10 seconds (macCatalyst workaround)

## Logging

- App logs are written to: `{CacheDirectory}/addimage.log`
- Debug output available in IDE console
- Server logs visible in console output

## Future Enhancements

- [ ] Search and filter games
- [ ] Game ratings and reviews
- [ ] User accounts and authentication
- [ ] Cloud storage for images
- [ ] Wishlist/favorites feature
- [ ] Game categories and tags
- [ ] Export game data to CSV/JSON

## License

This project is licensed under the MIT License - see LICENSE file for details.

## Author

Created with ❤️ using .NET MAUI

## Support

For issues, questions, or contributions, please open an issue on GitHub.
