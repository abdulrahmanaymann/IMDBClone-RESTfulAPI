# IMDbClone RESTful API

## Description

The **IMDbClone RESTful API** is a backend application designed to provide a robust platform for managing movie-related data, user authentication, and user interactions similar to the well-known IMDb website. The API supports operations for movies, ratings, reviews, watchlists, and user management, leveraging secure authentication and role-based access controls.

## Technologies

- **Programming Language**: C#
- **Framework**: ASP.NET Core
- **Database**: SQL Server
- **ORM**: Entity Framework Core
- **Authentication**: JWT (JSON Web Tokens)
- **Caching**: IMemoryCache
- **Web Scraping**: Python
- **External API**: TMDB API

## Web Scraping and Data Fetching

The IMDbClone RESTful API also integrates Python for web scraping and data fetching:

- **Web Scraping**: Python scripts are used to gather additional movie-related data from various websites, enhancing the database with up-to-date information.
- **TMDB API Integration**: The API fetches real-time movie data from The Movie Database (TMDB) API, ensuring users have access to the latest movie information.

## Architecture

The architecture of the IMDbClone RESTful API follows a layered structure to promote separation of concerns and maintainability, utilizing the Unit of Work and Repository patterns for efficient data access:

1. **Business Layer**: Contains services and mapping profiles responsible for business logic and DTO mappings.
2. **Common Layer**: Holds constants, enums, settings, and utility classes used throughout the application.
3. **Core Layer**: Defines data models, DTOs, and validation attributes.
4. **Data Access Layer**: Implements the Repository pattern for database interactions and manages the database context through the Unit of Work pattern, ensuring atomic operations and simplified transaction management.
5. **Web API Layer**: Exposes RESTful endpoints through controllers, handling HTTP requests and responses.

## Backend Structure

The backend consists of several key components:

- **IMDbClone.Business**
  - **Mapper**: 
    - `MappingProfile.cs`: Configures mappings between DTOs and models.
  - **Services**: 
    - Contains interfaces and implementations for handling various aspects of the application, including authentication, caching, and data management.

- **IMDbClone.Common**
  - **Constants**: Defines roles, HTTP status codes, and cache keys for consistent usage across the application.
  - **Settings**: Manages cache and other configuration settings.

- **IMDbClone.Core**
  - **DTOs**: Comprehensive data transfer objects for handling authentication, movies, ratings, reviews, users, and watchlists.
  - **Models**: Entity models that represent the application's data structure.
  - **Utilities**: Classes for pagination, expression handling, and validation.

- **IMDbClone.DataAccess**
  - **Repository**: Implements the repository pattern for data operations, including unit of work management and entity repositories.

- **IMDbClone.WebAPI**
  - **Controllers**: Handle HTTP requests and route them to the appropriate services.

## Controllers

### 1. **AuthController**
The `AuthController` handles user authentication and profile management. It exposes the following endpoints:

#### Endpoints

- **Register** (`POST /api/auth/register`): 
  - **Description**: Allows users to create a new account by providing registration details. 
  - **Response**: Returns a response indicating the result of the registration process.

- **Login** (`POST /api/auth/login`): 
  - **Description**: Authenticates users and generates a JWT token upon successful login. 
  - **Response**: Returns the authentication tokens if the credentials are valid.

- **Refresh Token** (`POST /api/auth/refresh-token`): 
  - **Description**: Provides a mechanism to refresh expired authentication tokens by submitting the existing access and refresh tokens. 
  - **Response**: Returns new tokens if the refresh process is successful.

- **User Profile** (`GET /api/auth/profile`): 
  - **Description**: Retrieves the profile information of the currently logged-in user. 
  - **Response**: Requires authentication.

- **Update Profile** (`PUT /api/auth/profile`): 
  - **Description**: Allows the logged-in user to update their profile information.
  - **Response**: Requires authentication and returns the updated profile details.

### 2. **MovieController**
The `MovieController` handles operations related to movies in the IMDb Clone API. Below are the available endpoints and their functionalities:

#### Endpoints

- **GET /api/movies**
  - **Description**: Retrieves a list of all movies.
  - **Query Parameters**: 
    - `page` (optional): The page number for pagination.
    - `pageSize` (optional): The number of movies per page.
    - `genre` (optional): Filter movies by genre.
    - `title` (optional): Filter movies by title (case-insensitive).
  - **Response**: Returns a paginated list of movies.

- **GET /api/movies/{id}**
  - **Description**: Fetches detailed information about a specific movie.
  - **Path Parameters**:
    - `id`: The unique identifier of the movie.
  - **Response**: Returns the details of the movie, including title, release date, rating, and reviews.

- **POST /api/movies**
  - **Description**: Allows the addition of new movies to the database.
  - **Request Body**: Movie details such as title, release date, genre, etc.
  - **Response**: Returns the created movie's details with a 201 Created status.

- **PUT /api/movies/{id}**
  - **Description**: Updates existing movie details.
  - **Path Parameters**:
    - `id`: The unique identifier of the movie to be updated.
  - **Request Body**: Updated movie details.
  - **Response**: Returns the updated movie's details.

- **DELETE /api/movies/{id}**
  - **Description**: Removes a movie from the database.
  - **Path Parameters**:
    - `id`: The unique identifier of the movie to be deleted.
  - **Response**: Returns a success message upon deletion.

### 3. **RatingController**
The `RatingController` manages operations related to movie ratings in the IMDb Clone API. Below are the available endpoints and their functionalities:

#### Endpoints

- **GET /api/ratings**
  - **Description**: Retrieves all ratings from the database.
  - **Response**: Returns an API response containing a list of all ratings.

- **GET /api/ratings/{id}**
  - **Description**: Retrieves a specific rating by its ID.
  - **Path Parameters**:
    - `id`: The ID of the rating to retrieve.
  - **Response**: Returns an API response containing the requested rating.

- **POST /api/ratings**
  - **Description**: Allows users to submit a rating for a movie.
  - **Request Body**: A `CreateRatingDTO` object containing the rating information, including the `MovieId` and the rating value.
  - **Response**: Returns an API response indicating the result of the creation with a 201 Created status.

- **PUT /api/ratings/{id}**
  - **Description**: Updates an existing rating submitted by a user.
  - **Path Parameters**:
    - `id`: The ID of the rating to update.
  - **Request Body**: An `UpdateRatingDTO` object containing the updated rating information.
  - **Response**: Returns an API response indicating the result of the update.

- **DELETE /api/ratings/{id}**
  - **Description**: Removes a user's rating for a movie.
  - **Path Parameters**:
    - `id`: The ID of the rating to delete.
  - **Response**: Returns a success message with a 204 No Content status upon successful deletion.

### 4. **ReviewController**
The `ReviewController` manages movie reviews in the IMDbClone API. It provides endpoints for retrieving, creating, updating, and deleting reviews. The controller interacts with the `IReviewService` and `IMovieService` interfaces to handle business logic and data access.

#### Endpoints

- **GET /api/review**
  - **Description**: Retrieves all reviews from the database.
  - **Response**: Returns a list of reviews with a 200 OK status.

- **GET /api/review/{id}**
  - **Description**: Retrieves a single review by its ID.
  - **Path Parameters**:
    - `id`: The ID of the review to retrieve.
  - **Response**: Returns the specified review with a 200 OK status.

- **POST /api/review**
  - **Description**: Creates a new review for a specified movie.
  - **Request Body**: A `CreateReviewDTO` object containing the review details.
  - **Response**: Returns the created review with a 201 Created status.

- **PUT /api/review/{id}**
  - **Description**: Updates an existing review by its ID.
  - **Path Parameters**:
    - `id`: The ID of the review to update.
  - **Request Body**: An `UpdateReviewDTO` object containing the updated review details.
  - **Response**: Returns the updated review with a 200 OK status.

- **DELETE /api/review/{id}**
  - **Description**: Deletes a review by its ID.
  - **Path Parameters**:
    - `id`: The ID of the review to delete.
  - **Response**: Returns a 204 No Content status upon successful deletion.

### 5. **WatchlistController**
The `WatchlistController` is responsible for managing the watchlists of authenticated users in the IMDbClone application. This includes retrieving, adding, and removing movies from the user's watchlist.

#### Endpoints

- **GET /api/watchlist**
  - **Description**: Retrieves all watchlists for the authenticated user. You can optionally filter the results by genre or movie title.
  - **Response**: Returns a list of movies in the user's watchlist.

- **GET /api/watchlist/{id}**
  - **Description**: Retrieves the details of a specific movie in the user's watchlist by its ID.
  - **Path Parameters**:
    - `id`: The ID of the movie in the watchlist.
  - **Response**: Returns the movie details if found.

- **POST /api/watchlist**
  - **Description**: Adds a movie to the user's watchlist.
  - **Request Body**: A `CreateWatchlistDTO` object containing the movie ID.
  - **Response**: Returns a success message and the updated watchlist.

- **DELETE /api/watchlist/{id}**
  - **Description**: Removes a movie from the user's watchlist by its ID.
  - **Path Parameters**:
    - `id`: The ID of the movie to remove.
  - **Response**: Returns a success message confirming the removal.
