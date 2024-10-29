![Logo](https://i.imgur.com/srymhmQ.png)

# 🎬 IMDbClone RESTful API

## 📝 Description

The **IMDbClone RESTful API** is a backend application that replicates key features of IMDb, managing movie-related data, user authentication, ratings, reviews, and watchlists. It supports secure authentication and role-based access controls, allowing for flexible management of users and interactions.

---

## 🚀 Technologies

- **Programming Language**: C#
- **Framework**: ASP.NET Core
- **Database**: SQL Server
- **ORM**: Entity Framework Core
- **Authentication**: JWT (JSON Web Tokens)
- **Caching**: IMemoryCache
- **Web Scraping**: Python
- **External API**: TMDB API

---

## 🌐 Data Integration

- **Web Scraping**: Uses Python scripts to gather additional movie data from external sources, enhancing the database with up-to-date information.
- **TMDB API**: Integrates with The Movie Database (TMDB) API for real-time movie data fetching, ensuring users always have access to the latest information.

---

## 🏗 Architecture

The API follows a layered architecture that promotes modularity and separation of concerns, utilizing Unit of Work and Repository patterns for efficient data handling.

- **Business Layer**: Contains services and mapping profiles for business logic and DTO mappings.
- **Common Layer**: Stores constants, enums, settings, and utility classes.
- **Core Layer**: Defines data models, DTOs, and validation attributes.
- **Data Access Layer**: Manages database operations with Repository and Unit of Work patterns.
- **Web API Layer**: Exposes RESTful endpoints to handle client interactions.

---

## 📂 Backend Structure

### **Project Modules**

- **IMDbClone.Business**
  - **Services**: Interfaces and implementations for application functionalities like authentication, caching, and data management.
  - **Mapper**: Configures mappings between DTOs and models.
  
- **IMDbClone.Common**
  - **Constants**: Defines roles, HTTP status codes, and cache keys.
  - **Settings**: Manages configurations for caching and other settings.
  
- **IMDbClone.Core**
  - **DTOs**: Data transfer objects for movies, users, ratings, reviews, and watchlists.
  - **Models**: Entity models defining the application's data structure.
  - **Utilities**: Provides pagination, expression handling, and validation.
  
- **IMDbClone.DataAccess**
  - **Repository**: Implements data operations using Unit of Work for transactional integrity.
  
- **IMDbClone.WebAPI**
  - **Controllers**: Exposes API endpoints to handle requests and responses.

---

## 🎮 Controllers Overview


### 1. **AuthController** 🔒
Handles user authentication and profile management operations.

| Endpoint                  | Method | Description                                | Auth Required |
|---------------------------|--------|--------------------------------------------|---------------|
| `/api/auth/register`      | POST   | Register a new user                        | ❌            |
| `/api/auth/login`         | POST   | Login and receive JWT tokens               | ❌            |
| `/api/auth/refresh-token` | POST   | Refresh access tokens                      | ✅            |
| `/api/auth/profile`       | GET    | Retrieve profile of the logged-in user     | ✅            |
| `/api/auth/profile`       | PUT    | Update profile of the logged-in user       | ✅            |

### 2. **MovieController** 🎬
Manages movie data, including addition, retrieval, updating, and deletion.

| Endpoint                | Method | Description                                         |
|-------------------------|--------|-----------------------------------------------------|
| `/api/movies`           | GET    | Retrieve all movies with optional filters           |
| `/api/movies/{id}`      | GET    | Retrieve details for a specific movie               |
| `/api/movies`           | POST   | Add a new movie to the database                     |
| `/api/movies/{id}`      | PUT    | Update existing movie details                       |
| `/api/movies/{id}`      | DELETE | Remove a movie from the database                    |

### 3. **RatingController** ⭐
Controls movie ratings, allowing users to add, view, update, and delete ratings.

| Endpoint                | Method | Description                            |
|-------------------------|--------|----------------------------------------|
| `/api/ratings`          | GET    | Retrieve all ratings                   |
| `/api/ratings/{id}`     | GET    | Retrieve a specific rating             |
| `/api/ratings`          | POST   | Submit a new rating                    |
| `/api/ratings/{id}`     | PUT    | Update an existing rating              |
| `/api/ratings/{id}`     | DELETE | Delete a rating                        |

### 4. **ReviewController** 📝
Manages user reviews for movies, including retrieval, creation, updating, and deletion.

| Endpoint                | Method | Description                                         |
|-------------------------|--------|-----------------------------------------------------|
| `/api/review`           | GET    | Retrieve all reviews                                |
| `/api/review/{id}`      | GET    | Retrieve a specific review                          |
| `/api/review`           | POST   | Create a new review                                 |
| `/api/review/{id}`      | PUT    | Update an existing review                           |
| `/api/review/{id}`      | DELETE | Delete a review                                     |

### 5. **WatchlistController** 🎥
Handles watchlists for users, allowing them to manage their movie collections.

| Endpoint                | Method | Description                                         |
|-------------------------|--------|-----------------------------------------------------|
| `/api/watchlist`        | GET    | Retrieve all movies in the user’s watchlist         |
| `/api/watchlist/{id}`   | GET    | Retrieve details of a specific movie in watchlist   |
| `/api/watchlist`        | POST   | Add a movie to the user’s watchlist                 |
| `/api/watchlist/{id}`   | DELETE | Remove a movie from the user’s watchlist            |

### 6. **UserController** 👤  
Manages user profiles, roles, and administrative account operations.

| Endpoint                     | Method | Description                                    | Auth Required |
|------------------------------|--------|------------------------------------------------|---------------|
| `/api/users`                 | GET    | Retrieve all registered users with filtering   | ✅ |
| `/api/users/{userId}`        | GET    | Retrieve a specific user profile by ID         | ✅ |
| `/api/users/{userId}`        | DELETE | Delete a specific user from the system         | ✅ |
| `/api/users/{userId}/roles`  | GET    | Retrieve roles assigned to a specific user     | ✅ |
| `/api/users/{userId}/roles`  | POST   | Assign a role to a specific user               | ✅ |
| `/api/users/{userId}/roles`  | PUT    | Update roles assigned to a specific user       | ✅ |
| `/api/users/{userId}/roles`  | DELETE | Remove a role from a specific user             | ✅ |
| `/api/users/roles`           | GET    | Retrieve all available roles                   | ✅ |

---

## 📁 Project Structure

```
IMDbClone
│
├── IMDbClone.Business
│   ├── Mapper
│   │   └── MappingProfile.cs
│   ├── Services
│   │   ├── IServices
│   │   │   └── IUserService.cs       
│   │   ├── auth
│   │   ├── cache
│   │   ├── movie
│   │   ├── rating
│   │   ├── review
│   │   ├── watchlist
│   │   └── token
│   │   └── user                     
│   │       └── UserService.cs       
│
├── IMDbClone.Common
│   ├── Constants
│   │   ├── Roles.cs
│   │   ├── HttpStatusCodes.cs
│   │   └── CacheKeys.cs
│   └── Settings
│       └── CacheSettings.cs
│
├── IMDbClone.Core
│   ├── DTOs
│   │   ├── AuthDTOs
│   │   │   ├── LoginRequestDTO.cs
│   │   │   ├── LoginResponseDTO.cs
│   │   │   ├── RefreshTokenRequestDTO.cs
│   │   │   └── RegistrationRequestDTO.cs
│   │   ├── MovieDTOs
│   │   │   ├── CreateMovieDTO.cs
│   │   │   ├── MovieDTO.cs
│   │   │   ├── MovieSummaryDTO.cs
│   │   │   └── UpdateMovieDTO.cs
│   │   ├── RatingDTOs
│   │   │   ├── CreateRatingDTO.cs
│   │   │   ├── RatingDTO.cs
│   │   │   └── UpdateRatingDTO.cs
│   │   ├── ReviewDTOs
│   │   │   ├── CreateReviewDTO.cs
│   │   │   ├── ReviewDTO.cs
│   │   │   └── UpdateReviewDTO.cs
│   │   ├── UserDTOs
│   │   │   ├── UserDTO.cs
│   │   │   └── UserProfileDTO.cs
│   │   └── WatchlistDTOs
│   │       ├── CreateWatchlistDTO.cs
│   │       └── WatchlistDTO.cs
│   ├── Enums
│   │   └── Genre.cs
│   ├── Exceptions
│   │   └── ApiException.cs
│   ├── Models
│   │   ├── ApplicationUser.cs
│   │   ├── Movie.cs
│   │   ├── Rating.cs
│   │   ├── Review.cs
│   │   └── Watchlist.cs
│   ├── Responses
│   │   └── APIResponse.cs
│   ├── Utilities
│   │   ├── PaginatedResult.cs
│   │   └── ExpressionUtilities.cs
│   └── Validation
│       ├── FullNameAttribute.cs
│       └── ValidCastAttribute.cs
│
├── IMDbClone.DataAccess
│   ├── Data
│   │   └── ApplicationDbContext.cs
│   ├── DbInitializer
│   │   └── DBInitializer.cs
│   ├── Migrations
│   └── Repository
│       ├── IRepository
│       ├── WatchlistRepository.cs
│       ├── UserRepository.cs
│       ├── UnitOfWork.cs
│       ├── ReviewRepository.cs
│       ├── Repository.cs
│       ├── RatingRepository.cs
│       └── MovieRepository.cs
│
├── IMDbClone.WebAPI
│   ├── Controllers
│   │   ├── AuthController.cs
│   │   ├── MovieController.cs
│   │   ├── RatingController.cs
│   │   ├── ReviewController.cs
│   │   ├── UserController.cs             
│   │   └── WatchlistController.cs
│   ├── Program.cs
│   └── appsettings.json
```
---

## 📌 Getting Started

1. Clone the repository.
   ```bash
   git clone https://github.com/yourusername/IMDbClone-API.git
   ```
2. Install dependencies and set up the database.
3. Configure environment variables for database connection, JWT, TMDB API keys, and any other required settings.
4. Run the application using your preferred IDE or command line.

---
