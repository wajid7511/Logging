# E-Commerce Logging System

A comprehensive .NET 8.0 solution that demonstrates SOLID principles with MongoDB storage, RabbitMQ messaging, and a Blazor dashboard for monitoring HTTP request/response logs.

## 🏗️ Architecture

The solution follows a clean, layered architecture with SOLID principles:

- **ECommerce.Domain**: Core entities and interfaces
- **ECommerce.Application**: Business logic and MediatR handlers
- **ECommerce.Infrastructure**: Data access, messaging, and external services
- **ECommerce.Api**: REST API with request/response logging middleware
- **ECommerce.Worker**: Background service for processing log messages
- **ECommerce.Blazor**: Web dashboard for viewing and filtering logs

## 🚀 Features

### Core Functionality
- **Product Management**: CRUD operations for e-commerce products
- **Request/Response Logging**: Automatic logging of all HTTP requests and responses
- **Asynchronous Logging**: RabbitMQ integration prevents request delays
- **MongoDB Storage**: Persistent storage of products and logs
- **Real-time Dashboard**: Blazor-based web interface for log monitoring

### Logging Dashboard Features
- **Real-time Statistics**: Total logs, success rates, error counts
- **Advanced Filtering**: By HTTP method, status code, path, trace ID, date range
- **Pagination**: Efficient handling of large log volumes
- **Detailed View**: Modal popup showing full request/response details
- **Visual Indicators**: Color-coded status codes and HTTP methods
- **Responsive Design**: Mobile-friendly interface

## 🛠️ Prerequisites

- .NET 8.0 SDK
- MongoDB (running on localhost:27017)
- RabbitMQ (running on localhost:5672)

## 📦 Installation & Setup

### 1. Clone and Build
```bash
git clone <repository-url>
cd Logging
dotnet restore
dotnet build
```

### 2. Start Services
```bash
# Terminal 1: Start the Worker service
dotnet run --project ECommerce.Worker

# Terminal 2: Start the API
dotnet run --project ECommerce.Api

# Terminal 3: Start the Blazor Dashboard
dotnet run --project ECommerce.Blazor
```

### 3. Access Applications
- **API**: http://localhost:5000
- **Blazor Dashboard**: http://localhost:5001
- **API Swagger**: http://localhost:5000/swagger

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

### RabbitMQ Settings
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

## 📊 Using the Dashboard

### Dashboard Overview
- **Statistics Cards**: View total logs, success rates, and error counts
- **Recent Activity**: See the latest 10 log entries
- **Method Distribution**: Visual breakdown of HTTP methods used

### Logs Page
- **Filters**: Narrow down logs by various criteria
- **Table View**: Paginated display of all logs
- **Details Modal**: Click "View" to see full request/response data
- **Export**: Copy log data for external analysis

### Filter Options
- **HTTP Method**: GET, POST, PUT, DELETE, PATCH
- **Status Code**: 200, 201, 400, 401, 404, 500
- **Path**: Search by API endpoint
- **Trace ID**: Find specific request chains
- **Date Range**: Filter by timestamp

## 🔍 API Endpoints

### Products
- `GET /api/products` - List all products
- `GET /api/products/{id}` - Get product by ID
- `POST /api/products` - Create new product
- `PUT /api/products/{id}` - Update product
- `DELETE /api/products/{id}` - Delete product

### Logs (via Dashboard)
- All HTTP requests are automatically logged
- Logs include request/response bodies, headers, and timing
- Stored in MongoDB collection: `http_logs`

## 🐛 Troubleshooting

### Common Issues

1. **MongoDB Connection Failed**
   - Ensure MongoDB is running on localhost:27017
   - Check connection string in appsettings.json

2. **RabbitMQ Connection Failed**
   - Ensure RabbitMQ is running on localhost:5672
   - Verify credentials in appsettings.json

3. **Logs Not Appearing in Dashboard**
   - Check if Worker service is running
   - Verify RabbitMQ queue has messages
   - Check MongoDB connection in Blazor app

4. **Build Errors**
   - Ensure .NET 8.0 SDK is installed
   - Run `dotnet restore` before building

### Debug Mode
Enable detailed logging by setting log level to "Information" in appsettings.json:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

## 🏛️ SOLID Principles Implementation

- **Single Responsibility**: Each class has one clear purpose
- **Open/Closed**: Extensible through interfaces and inheritance
- **Liskov Substitution**: Implementations can be swapped
- **Interface Segregation**: Focused, specific interfaces
- **Dependency Inversion**: High-level modules don't depend on low-level modules

## 🔄 Message Flow

1. **HTTP Request** → API
2. **Middleware** → Logs request/response
3. **RabbitMQ** → Publishes log message
4. **Worker** → Consumes message
5. **MongoDB** → Stores log entry
6. **Dashboard** → Displays logs in real-time

## 📈 Performance Features

- **Asynchronous Logging**: No impact on API response times
- **Pagination**: Efficient handling of large log volumes
- **Indexed Queries**: MongoDB indexes for fast filtering
- **Connection Pooling**: Optimized database connections

## 🚀 Deployment

### Production Considerations
- Use environment-specific configuration
- Implement proper authentication for dashboard
- Set up monitoring and alerting
- Configure backup strategies for MongoDB
- Use managed RabbitMQ service in production

### Docker Support
```bash
# Build and run with Docker Compose
docker-compose up -d
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Implement changes following SOLID principles
4. Add tests for new functionality
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🆘 Support

For issues and questions:
- Check the troubleshooting section
- Review application logs
- Open an issue in the repository

---

**Happy Logging! 🎉**
