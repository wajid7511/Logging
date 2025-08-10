# 🛒 E-Commerce Logging System

A comprehensive .NET Core e-commerce application with advanced HTTP request/response logging capabilities, built using clean architecture principles and modern technologies.

## 🚀 Features

- **HTTP Request/Response Logging**: Complete capture and storage of all HTTP interactions
- **Real-time Dashboard**: Visual analytics and monitoring of application traffic
- **Advanced Log Filtering**: Search and filter logs by method, path, status code, and more
- **Raw MongoDB Data View**: Debug and inspect raw document data
- **Clean Architecture**: Separation of concerns with Domain, Application, Infrastructure, and API layers
- **MongoDB Integration**: Scalable document storage for logs and products
- **RabbitMQ Messaging**: Asynchronous message processing
- **Blazor UI**: Modern, responsive web interface
- **Worker Service**: Background processing for log analysis

## 🏗️ Architecture

```
ECommerce/
├── 📁 ECommerce.Domain/          # Domain entities and interfaces
├── 📁 ECommerce.Application/      # Application logic and contracts
├── 📁 ECommerce.Infrastructure/  # Data access and external services
├── 📁 ECommerce.Api/             # REST API endpoints
├── 📁 ECommerce.Blazor/          # Web UI application
└── 📁 ECommerce.Worker/          # Background processing service
```

### Clean Architecture Layers

- **Domain Layer**: Core business entities (`Product`, `RequestResponseLog`)
- **Application Layer**: Business logic and use cases
- **Infrastructure Layer**: MongoDB persistence, RabbitMQ messaging
- **API Layer**: HTTP endpoints and middleware
- **Presentation Layer**: Blazor web interface

## 🛠️ Technology Stack

- **.NET 8.0**: Latest .NET framework
- **MongoDB**: Document database for logs and products
- **RabbitMQ**: Message broker for asynchronous processing
- **Blazor Server**: Interactive web UI framework
- **MongoDB.Driver**: Official MongoDB C# driver
- **MediatR**: Mediator pattern implementation
- **Bootstrap**: CSS framework for responsive design

## 📋 Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [MongoDB](https://www.mongodb.com/try/download/community) (running locally or accessible)
- [RabbitMQ](https://www.rabbitmq.com/download.html) (optional, for messaging features)

## 🚀 Quick Start

### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/ecommerce-logging.git
cd ecommerce-logging
```

### 2. Configure MongoDB

Update the connection string in `ECommerce.Blazor/appsettings.json`:

```json
{
  "MongoDbSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "ecommerce"
  }
}
```

### 3. Restore Dependencies

```bash
dotnet restore
```

### 4. Build the Solution

```bash
dotnet build
```

### 5. Run the Applications

#### Start the Blazor Web Application
```bash
dotnet run --project ECommerce.Blazor --urls "http://localhost:5001"
```

#### Start the API (in another terminal)
```bash
dotnet run --project ECommerce.Api --urls "http://localhost:5000"
```

#### Start the Worker Service (optional)
```bash
dotnet run --project ECommerce.Worker
```

## 🌐 Access the Application

- **Blazor UI**: http://localhost:5001
- **API**: http://localhost:5000
- **Dashboard**: http://localhost:5001/dashboard
- **Logs**: http://localhost:5001/logs
- **Debug**: http://localhost:5001/debug

## 📊 Key Features Explained

### HTTP Logging Middleware

The system automatically captures all HTTP requests and responses using custom middleware:

```csharp
// Automatically logs all HTTP interactions
app.UseMiddleware<RequestResponseLoggingMiddleware>();
```

### Log Storage

Logs are stored in MongoDB with the following structure:

```json
{
  "_id": "string",
  "timestampUtc": "2024-01-01T00:00:00Z",
  "traceId": "unique-trace-identifier",
  "method": "GET",
  "path": "/api/products",
  "statusCode": 200,
  "requestBody": "request content",
  "responseBody": "response content",
  "requestHeaders": {},
  "responseHeaders": {}
}
```

### Advanced Filtering

The logging system supports sophisticated filtering:

- **HTTP Method**: GET, POST, PUT, DELETE
- **Path Patterns**: URL path matching
- **Status Codes**: HTTP response codes
- **Date Ranges**: Time-based filtering
- **Trace IDs**: Request correlation

## 🔧 Configuration

### MongoDB Settings

```json
{
  "MongoDbSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "ecommerce"
  }
}
```

### RabbitMQ Settings (Optional)

```json
{
  "RabbitMqSettings": {
    "HostName": "localhost",
    "UserName": "guest",
    "Password": "guest",
    "Exchange": "ecommerce.logs",
    "Queue": "http_logs",
    "RoutingKey": "http.log"
  }
}
```

## 📁 Project Structure

```
ECommerce/
├── 📁 ECommerce.Domain/
│   ├── 📁 Entities/
│   │   ├── Product.cs
│   │   └── RequestResponseLog.cs
│   └── 📁 Interfaces/
│       └── IProductRepository.cs
├── 📁 ECommerce.Application/
│   ├── 📁 Contracts/
│   ├── 📁 Products/
│   │   ├── 📁 Commands/
│   │   └── 📁 Queries/
├── 📁 ECommerce.Infrastructure/
│   ├── 📁 Persistence/
│   │   ├── MongoDbContext.cs
│   │   ├── ProductRepository.cs
│   │   └── RequestResponseLogRepository.cs
│   └── 📁 Messaging/
│       └── RabbitMqClient.cs
├── 📁 ECommerce.Api/
│   ├── 📁 Controllers/
│   ├── 📁 Middleware/
│   └── 📁 Configurations/
├── 📁 ECommerce.Blazor/
│   ├── 📁 Pages/
│   ├── 📁 Services/
│   └── 📁 Shared/
└── 📁 ECommerce.Worker/
    └── 📁 Consumers/
```

## 🧪 Testing

### Run Tests

```bash
dotnet test
```

### Manual Testing

1. **Start the application**
2. **Navigate to different pages** to generate logs
3. **Check the Dashboard** for real-time statistics
4. **Use the Logs page** to filter and search logs
5. **Inspect raw data** in the Debug page

## 🔍 Troubleshooting

### Common Issues

#### MongoDB Connection
- Ensure MongoDB is running on the configured port
- Check connection string in `appsettings.json`
- Verify network access and firewall settings

#### Port Conflicts
- Change ports in `launchSettings.json` if conflicts occur
- Use different URLs for multiple instances

#### Build Errors
- Ensure .NET 8.0 SDK is installed
- Run `dotnet restore` before building
- Check for missing package references

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- Built with [.NET 8.0](https://dotnet.microsoft.com/)
- Database powered by [MongoDB](https://www.mongodb.com/)
- UI framework by [Blazor](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)
- Message broker by [RabbitMQ](https://www.rabbitmq.com/)

## 📞 Support

For support and questions:

- Create an [Issue](https://github.com/yourusername/ecommerce-logging/issues)
- Contact: email2wajidkhan@gmail.com
- Documentation: [Wiki](https://github.com/yourusername/ecommerce-logging/wiki)

---

**⭐ Star this repository if you find it helpful!**
