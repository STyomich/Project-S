# Project-S Agent Guidelines

This document helps AI agents be immediately productive in the Project-S codebase. Project-S is a full-stack microservices application with a .NET backend and Angular frontend.

## Project Overview

**Project-S** is a cloud-native, event-driven microservices platform:
- **Architecture**: Microservices with API Gateway routing
- **Backend**: .NET 10 (C# 13) layered services
- **Frontend**: Angular 21 with standalone components
- **Communication**: REST APIs + RabbitMQ event bus
- **Databases**: PostgreSQL (Users), MySQL (Notifications), MongoDB (Utilities)
- **Caching**: Redis for session/data caching

### Services Map
```
┌──────────────┐
│ Angular App  │ (port 4200)
└──────┬───────┘
       │ HTTP
┌──────▼──────────────────┐
│ ProjectS.Gateway        │ (port 5000) - Ocelot routing + JWT
└──────┬──────┬──────┬────┘
       │      │      │
 (8080)│      │(8081)│  (8082)
┌──────▼──┐ ┌─▼─────────┐ ┌─▼─────────────┐
│ Users   │ │Notification│ │ Utilities     │
│Service  │ │Service     │ │Service        │
└─────────┘ └────────────┘ └───────────────┘
 (Postgres) (MySQL)       (MongoDB)
```

## Quick Start Commands

### Backend - Build & Test
```bash
# Build individual service
cd src/ProjectS.UsersService
dotnet build

# Run tests
dotnet test UsersService.Application.Tests/UsersService.Application.Tests.csproj

# Run with watch mode
dotnet watch run

# Full Docker stack (all services + databases)
cd src && docker-compose up -d
```

### Frontend - Build & Test
```bash
# Install dependencies (if needed)
npm install

# Development server (localhost:4200)
npm start

# Build for production
npm run build

# Run unit tests (Vitest)
npm test

# Generate new component
ng generate component features/users/components/my-component
```

### Database Connections (docker-compose running)
```bash
# PostgreSQL (Users) - psql -h localhost -U postgres -d usersdb
# MySQL (Notifications) - mysql -h 127.0.0.1 -u root -p notificationsdb
# MongoDB (Utilities) - mongosh "mongodb://localhost:27017/Utilities"
# Redis - redis-cli -p 6379
# RabbitMQ Management - http://localhost:15672 (guest/guest)
```

## Architectural Patterns

### Backend - Clean Architecture (4-Layer)
Every .NET service follows this structure:
```
ServiceName.API (Controllers, Middlewares, Auth)
    ↓ depends on
ServiceName.Application (Services, DTOs, Validation)
    ↓ depends on
ServiceName.Infrastructure (DbContext, Cache, Messaging)
    ↓ depends on
ServiceName.Domain (Entities, Events, Value Objects)
```

**Service Registration Pattern** (in Program.cs):
```csharp
builder.Services.AddApiServices();              // API layer (CORS, HttpClient)
builder.Services.AddApplicationServices();      // Application layer (validators, mappers)
builder.Services.AddInfrastructureServices();   // Infrastructure (DB, cache, messaging)
```

### Frontend - Component Architecture
- **Standalone components** (no NgModules)
- **Feature modules** in `src/app/features/`
- **Core services** in `src/app/core/` (singleton services)
- **HTTP interceptors** for centralized auth token injection

**Feature Module Structure**:
```
features/users/
├── components/        (user-login-form, user-register-form)
├── services/          (users.service.ts)
├── models/            (user DTOs, interfaces with .dto/.model suffixes)
└── pages/             (user pages if needed)
```

### Authentication Flow
1. User logs in via `/login` endpoint
2. Backend (UsersService) validates, issues JWT token
3. Token stored in **HttpOnly, SameSite=Strict cookie** (not localStorage)
4. **authInterceptor** (frontend) extracts token from cookie, adds to every request
5. **Gateway** (Ocelot) validates JWT, routes to microservices
6. Services validate token claims

### Event-Driven Communication
- **UsersService** publishes events via RabbitMQ (e.g., UserUpdatedEmailEvent)
- **NotificationsService** consumes events via hosted service
- **Outbox Pattern**: Changes + message published in single DB transaction
- Environment variables define exchange names: `RABBITMQ_USERS_EXCHANGE: user.exchange`

### Resilience Patterns (Polly)
Each service implements 4 resilience policies:
- **RetryPolicy**: 3 exponential backoff retries
- **CircuitBreakerPolicy**: Break after 5 failures, 30s recovery
- **TimeoutPolicy**: 5-second request timeout
- **FallbackPolicy**: ServiceUnavailable when all policies fail

### Global Exception Handling
Backend middleware catches exceptions and maps to HTTP responses:
- `ValidationException` → 422 Unprocessable Entity
- `ArgumentException` → 404 Not Found
- `InvalidOperationException` → 400 Bad Request
- Unhandled exceptions → 500 Internal Server Error

## Code Conventions

### Backend (.NET)
- **Naming**: PascalCase for classes, interfaces, methods; camelCase for local variables
- **File structure**: 1 public class per file, matches class name
- **DTOs**: Separate from entities, suffixed as `CreateUserRequest`, `UserResponse`
- **Repositories**: Abstract behind interfaces (`IUserRepository`), registered in IoC
- **Validation**: FluentValidation library, auto-registered in Application layer
- **Database**: Entity Framework Core, migrations in `Infrastructure/Migrations/`
- **Connection strings**: Use environment variables (12-factor app compliance)

**Example service structure**:
```csharp
namespace ProjectS.UsersService.Application.Services;

public interface IUserService
{
    Task<UserResponse> GetUserAsync(int id);
    Task CreateUserAsync(CreateUserRequest request);
}

public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly IValidator<CreateUserRequest> _validator;
    
    public UserService(IUserRepository repository, IValidator<CreateUserRequest> validator)
    {
        _repository = repository;
        _validator = validator;
    }
}
```

### Frontend (Angular)
- **File naming**: kebab-case (e.g., `user-login-form.ts`)
- **Class naming**: PascalCase (e.g., `UserLoginForm`)
- **Selector prefix**: `app-` (e.g., `app-user-login-form`)
- **DTOs**: Files suffixed `.dto.ts` (e.g., `user-login-request.dto.ts`)
- **Models**: Files suffixed `.model.ts` (e.g., `user-short-info.model.ts`)
- **Observables**: Suffixed with `$` (e.g., `currentUser$: Observable<UserShortInfo>`)
- **Reactive Forms**: Use `FormBuilder` with typed groups

**Example component**:
```typescript
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { UsersService } from '../services/users.service';

@Component({
  selector: 'app-user-login-form',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './user-login-form.component.html'
})
export class UserLoginForm implements OnInit {
  loginForm: FormGroup;
  
  constructor(
    private fb: FormBuilder,
    private usersService: UsersService
  ) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]]
    });
  }
  
  onSubmit() {
    if (this.loginForm.valid) {
      this.usersService.login(this.loginForm.value).subscribe({
        next: (response) => { /* handle success */ },
        error: (error) => { /* handle error */ }
      });
    }
  }
}
```

## Common Development Tasks

### Adding a New Backend Endpoint

1. **Define Domain Entity** (UsersService.Domain/Entities/)
   ```csharp
   public class User : BaseEntity
   {
       public string Email { get; set; }
       public string FirstName { get; set; }
   }
   ```

2. **Create DTOs** (Application layer):
   ```csharp
   public record CreateUserRequest(string Email, string FirstName);
   public record UserResponse(int Id, string Email);
   ```

3. **Add Repository Method** (Infrastructure/Repositories/):
   ```csharp
   public async Task<User> AddUserAsync(User user) => 
       await _context.Users.AddAsync(user);
   ```

4. **Create Service** (Application/Services/):
   ```csharp
   public async Task<UserResponse> CreateUserAsync(CreateUserRequest request)
   {
       var user = new User { Email = request.Email, FirstName = request.FirstName };
       await _repository.AddUserAsync(user);
       return _mapper.Map<UserResponse>(user);
   }
   ```

5. **Add Validator** (Application/ - auto-registered):
   ```csharp
   public class CreateUserValidator : AbstractValidator<CreateUserRequest>
   {
       public CreateUserValidator()
       {
           RuleFor(x => x.Email).NotEmpty().EmailAddress();
       }
   }
   ```

6. **Register Service** (API/Extensions/ApplicationServiceExtensions.cs):
   ```csharp
   builder.Services.AddScoped<IUserService, UserService>();
   ```

7. **Create Controller** (API/Controllers/):
   ```csharp
   [ApiController]
   [Route("api/[controller]")]
   public class UsersController : ControllerBase
   {
       [HttpPost]
       public async Task<ActionResult<UserResponse>> CreateUser([FromBody] CreateUserRequest request)
       {
           var result = await _userService.CreateUserAsync(request);
           return CreatedAtAction(nameof(GetUser), new { id = result.Id }, result);
       }
   }
   ```

8. **Register in Gateway** (ProjectS.Gateway/ocelot.json) if public endpoint

### Adding a New Frontend Component

1. **Generate component**:
   ```bash
   ng generate component features/users/components/user-detail
   ```

2. **Create DTO files**:
   ```typescript
   // user.model.ts
   export interface User {
     id: number;
     email: string;
     firstName: string;
   }
   ```

3. **Implement service method** in `UsersService`:
   ```typescript
   getUser(id: number): Observable<User> {
     return this.http.get<User>(`/api/users/${id}`)
       .pipe(
         shareReplay(1),
         tap(user => console.log('User loaded:', user))
       );
   }
   ```

4. **Build component with reactive forms**:
   ```typescript
   export class UserDetail implements OnInit {
     user$: Observable<User>;
     
     constructor(private usersService: UsersService) {}
     
     ngOnInit() {
       this.user$ = this.usersService.getUser(1);
     }
   }
   ```

5. **Template binding**:
   ```html
   <div *ngIf="user$ | async as user">
     <h2>{{ user.firstName }}</h2>
     <p>{{ user.email }}</p>
   </div>
   ```

### Publishing Events from Backend

1. **Define event** (Domain/Events/):
   ```csharp
   public class UserEmailUpdatedEvent : DomainEvent
   {
       public int UserId { get; set; }
       public string NewEmail { get; set; }
   }
   ```

2. **Raise from entity**:
   ```csharp
   public void UpdateEmail(string newEmail)
   {
       Email = newEmail;
       RaiseDomainEvent(new UserEmailUpdatedEvent { UserId = Id, NewEmail = newEmail });
   }
   ```

3. **Publish via message bus** (automatically via Outbox pattern)
   - Outbox table captures event in DB transaction
   - Background processor (`OutboxProcessor`) publishes to RabbitMQ

### Consuming RabbitMQ Events (NotificationsService)

1. **Define message contract** (Shared/UsersService.Contracts/):
   ```csharp
   public record UserEmailUpdatedMessage(int UserId, string NewEmail);
   ```

2. **Create consumer** (Infrastructure/Messaging/):
   ```csharp
   public class UserEmailUpdatedEventHandler : IEventHandler<UserEmailUpdatedMessage>
   {
       public async Task Handle(UserEmailUpdatedMessage message, IServiceProvider serviceProvider)
       {
           // Send email notification
       }
   }
   ```

3. **Register in DI** (API/Program.cs):
   ```csharp
   builder.Services.AddMessageBusHandlers();
   ```

## Environment Variables

### Required (all services - set in docker-compose.yml or .env)
```
POSTGRES_HOST=postgres.db
POSTGRES_PORT=5432
POSTGRES_DATABASE=usersdb
POSTGRES_USER=postgres
POSTGRES_PASSWORD=password

MYSQL_HOST=mysql.db
MYSQL_PORT=3306
MYSQL_DATABASE=notificationsdb

MONGODB_CONNECTION_STRING=mongodb://mongo:27017
MONGODB_DATABASE=Utilities

REDIS_HOST=redis.cache
REDIS_PORT=6379

RABBITMQ_HOST=rabbitmq.bus
RABBITMQ_PORT=5672
RABBITMQ_USERNAME=guest
RABBITMQ_PASSWORD=guest

TokenKey=your-secret-key-for-jwt-signing
```

### Frontend Environment Config
- **Development**: `src/app/environments/environments.local.ts`
  - `API_URL: 'http://localhost:5000/api'`
- Currently no separate prod config (recommend adding)

## Common Pitfalls & Solutions

### Backend
- **DbContext not registered**: Add `AddInfrastructureServices()` in Program.cs
- **Validation not triggered**: Ensure validators auto-registered via `AddValidatorsFromAssemblyContaining<>`
- **CORS errors**: Check CORS policy in API service extensions
- **JWT expired tokens**: Client should check token expiry, retry login if needed
- **Missing RabbitMQ connection**: All services fail startup if message bus unavailable - ensure docker-compose services running

### Frontend
- **HttpOnly cookie not sent**: Ensure `HttpClientModule` provides credentials (`withCredentials: true`)
- **CORS issues with Bearer token**: Gateway must allow Origin + Authorization headers
- **Interceptor not applied**: Verify service has `providedIn: 'root'` and component imports correct module
- **Form validation not working**: Ensure validators imported from `@angular/forms` and applied via `Validators` class

## Testing

### Backend Testing
```bash
# Run tests for a specific service
cd src/ProjectS.UsersService
dotnet test UsersService.Application.Tests/UsersService.Application.Tests.csproj

# Test with code coverage
dotnet test /p:CollectCoverage=true
```

### Frontend Testing
```bash
# Run all unit tests
npm test

# Watch mode for development
npm test -- --watch

# Test specific file
npm test -- user-login-form.spec.ts
```

## Deployment

### Automated CI/CD with GitHub Actions (Recommended)

The project includes a comprehensive GitHub Actions workflow (`ci-cd-azure.yml`) that automates:

**Workflow Stages:**
1. **Test** (all branches) - Unit tests for backend (.NET) and frontend (Angular)
2. **Build** (push to dev/main) - Docker images for all services
3. **Deploy** (conditional):
   - `main` branch → Production (Azure Kubernetes Service)
   - `dev` branch → Development (Azure Container Instances)
4. **Cleanup** - Removes old images from registry

**Setup Requirements:**
- Configure GitHub Secrets with Azure credentials and environment variables
- Setup Azure Container Registry (ACR)
- Create Azure Service Principal for deployment
- Setup Kubernetes manifests in `k8s/` directory

**See [deployment/CI-CD-SETUP.md](deployment/CI-CD-SETUP.md) for complete setup instructions.**

### Manual Docker Image Build
```bash
# Navigate to service root
cd src/ProjectS.UsersService

# Build image
docker build -t project-s-users-service:latest -f Dockerfile .

# Push to Azure Container Registry
az acr build --registry projectsregistry --image project-s-users-service:latest .
```

### Docker Compose Orchestration (Local Development)
```bash
cd src

# Start all services locally
docker-compose up -d

# View logs
docker-compose logs -f users.api

# Stop all
docker-compose down
```

### Kubernetes Deployment (Production)
```bash
# Apply all manifests
kubectl apply -f k8s/

# Check deployment status
kubectl get deployments -n project-s-prod

# View pod logs
kubectl logs -n project-s-prod deployment/project-s-gateway

# Scale service
kubectl scale deployment project-s-gateway --replicas=3 -n project-s-prod
```

## Useful Files & References

| File | Purpose |
|------|---------|
| [src/docker-compose.yml](src/docker-compose.yml) | Infrastructure orchestration (databases, message bus, services) |
| [src/ProjectS.Gateway/ocelot.json](src/ProjectS.Gateway/ocelot.json) | API routing rules and JWT auth configuration |
| [src/ProjectS.Gateway/Program.cs](src/ProjectS.Gateway/Program.cs) | Gateway service registration and middleware setup |
| [src/ProjectS.UsersService/UsersService.API/Program.cs](src/ProjectS.UsersService/UsersService.API/Program.cs) | Service DI container & middleware pattern (apply to other services) |
| [frontend/project-s-frontend-app/src/app/core/services/users.service.ts](frontend/project-s-frontend-app/src/app/core/services/users.service.ts) | Core authentication service with state management |
| [frontend/project-s-frontend-app/angular.json](frontend/project-s-frontend-app/angular.json) | Angular build configuration and project structure |

## Questions or Issues?

- **Backend service not starting**: Check docker-compose logs for database/RabbitMQ connection errors
- **Frontend API calls failing**: Verify Gateway is running on port 5000 and CORS configured
- **Tests failing**: Ensure all dependencies installed (`dotnet restore`, `npm install`)
- **Database migrations**: Check Infrastructure/Migrations/ for pending migrations, run `dotnet ef database update`
