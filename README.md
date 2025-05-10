📚 LibraryManagement
LibraryManagement is a web-based library management system built with ASP.NET Core. This project includes features for managing books, users, book loans, and notably, user profile image upload functionality.

🚀 Features
📖 Book Management: Add, update, delete, and search books.

👤 User Management: Register, login, update personal information.

📅 Loan Management: Track book borrowing and returning.

🖼️ Profile Image Upload: Users can upload and update their profile pictures.

🛠️ Technologies Used
Backend: ASP.NET Core 8, Entity Framework Core

Database: SQL Server

Authentication: JWT (JSON Web Token)

API Testing: Swagger UI

Image Storage: Local file storage for user profile images

🧩 Project Structure
bash
Sao chép
Chỉnh sửa
├── Controllers/            # Handles HTTP requests
├── DTOs/                   # Data Transfer Objects
├── Models/                 # Data models
├── Repositories/           # Data access logic
├── Services/               # Business logic
├── Middlewares/            # Custom middleware
├── Profiles/               # AutoMapper configuration
├── Migrations/             # EF Core migrations
├── Utilities/              # Helper utilities
├── appsettings.json        # App configuration
├── Program.cs              # Application entry point
└── README.md               # Project documentation
🖼️ Profile Image Upload Feature
Users can upload profile images via the API. Images are stored on the server, and their paths are saved in the database.

Endpoint: Upload Profile Image
URL: POST /api/users/upload-profile-image

Headers:

Authorization: Bearer <token>

Content-Type: multipart/form-data

Body:

image: The profile image file

Sample Successful Response
json
Sao chép
Chỉnh sửa
{
  "message": "Profile image uploaded successfully.",
  "imageUrl": "https://yourdomain.com/images/profiles/user123.jpg"
}
🔧 Setup and Run
Clone the project:

bash
Sao chép
Chỉnh sửa
git clone -b feature/user-profile-image-upload https://github.com/daohd2003/LibraryManagement.git
Configure the database in appsettings.json.

Run EF Core migrations:

bash
Sao chép
Chỉnh sửa
dotnet ef database update
Start the application:

bash
Sao chép
Chỉnh sửa
dotnet run
Access the Swagger UI at:

bash
Sao chép
Chỉnh sửa
https://localhost:5001/swagger
📸 Screenshots


🤝 Contributing
Contributions are welcome! Feel free to open an issue or submit a pull request to improve this project.

📄 License
This project is licensed under the MIT License.

If you have any questions or feedback, feel free to contact the maintainer at daoha20102003@gmail.com.
