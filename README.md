# MedPoint
.NET 7 Clean Architecture and TDD Demo with Docker Support

## Repository Description
A comprehensive demonstration of Clean Architecture and Test-Driven Development (TDD) in a .NET 7 web application, featuring Docker support for seamless deployment. MedPoint showcases best practices in building scalable and maintainable software, with both unit and integrated testing strategies implemented to ensure robustness and reliability.

The solution includes Swagger for API documentation and supports Postman for API testing, enhancing the developer experience. The frontend serves as a demo, highlighting key functionalities with limited scope compared to the full capabilities of the API.

## Introduction
MedPoint is a digital prescribing assistant web application that provides comprehensive information on branded, generic, and OTC drugs, including dosage, administration, and manufacturer details. Designed for healthcare providers, this platform simplifies the process of finding accurate drug information. The application is containerized using Docker, emphasizing ease of deployment and scalability.

## User Story
"As a healthcare provider, I want a reliable and easy-to-use digital assistant where I can quickly find comprehensive information about various drugs, including dosages, administration instructions, and manufacturer details, so I can accurately prescribe medications to my patients."

## Design Choices
- **Architecture**: Adopted Clean Architecture principles to ensure separation of concerns, independence of components, and an easily maintainable and scalable application structure.
- **Backend**: Developed with ASP.NET Core Web API, providing a robust and efficient backend to handle CRUD operations and user authentication.
- **Database**: Chose MongoDB for its flexibility and scalability, storing drug and user data in a schema-less format that can easily adapt to changing requirements.
- **Authentication**: Implemented JWT authentication for securing API endpoints, ensuring that sensitive operations are protected and accessible only by authorized users.
- **Testing**: Emphasized Test-Driven Development (TDD) methodologies, with comprehensive unit tests written using xUnit to cover the data access layer, business logic layer, and API endpoints.
- **Frontend**: A SPA (Single Page Application) implemented with React, designed as simply as possible to provide a user-friendly interface while utilizing Bootstrap for responsive design. It's important to note that the frontend demo does not exhaustively cover all API services. Developers are encouraged to use Swagger or Postman to discover and test additional endpoints and functionalities not represented in the frontend demo.
- **Swagger**: The project integrates Swagger to provide interactive API documentation, allowing for easy testing and exploration of the available endpoints directly from your browser. To access Swagger, navigate to https://localhost:<port>/swagger after starting the application.
- **Postman**: Support for Postman testing is available, enabling external testing and interaction with the API. A Postman collection can be exported from Swagger or manually created to explore the full capabilities of the API beyond what's demonstrated in the frontend.

## Technical Architecture
MedPoint leverages Clean Architecture, separating concerns into independent layers that reduce dependencies on externalities and enhance the system's maintainability. This architecture facilitates the integration of the React frontend with the ASP.NET Core backend through a well-defined API, all while interacting with a MongoDB database for data persistence. Docker containers encapsulate the application's environment, ensuring consistency across development and production setups.

## How to Run the Project
1. **Prerequisites**: Ensure Docker Desktop and Visual Studio 2022 are installed on your machine.
2. **Backend and Database Setup**: Open the solution in Visual Studio 2022, set the Docker Compose project as the startup project, and run it. This action spins up both the web application and a MongoDB container, ensuring a seamless development experience.
3. **Running Tests**: The Dockerfile includes steps to run unit tests, making it suitable for CI/CD pipelines where automated testing is crucial before deployment.
4. **Deployment**: The containerized application can be deployed to cloud services that support Docker, offering flexibility in hosting options. While the current setup is optimized for development with Visual Studio 2022, support for other environments like VS Code can be added as needed.

## Testing Strategy
The project adopts TDD methodologies, emphasizing the importance of testing from the outset. Unit tests validate the functionality of individual components, ensuring the application behaves as expected under various scenarios. This approach enhances code quality, reduces bugs, and facilitates maintenance and future enhancements.

## Additional Notes
- The frontend is intentionally kept simple to focus on functionality and ease of use, demonstrating how React can be leveraged to create engaging user experiences with minimal complexity.
- The use of Docker not only simplifies setup and deployment but also aligns with modern development practices, offering an efficient way to package and distribute applications.

## Acknowledgments
- Special thanks to the technical interview panel for this opportunity to demonstrate my approach to building scalable, maintainable, and user-friendly software solutions.
