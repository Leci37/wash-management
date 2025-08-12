

# 🧩 SUMISAN Frontend (Angular 16+)

> **Current Status**: ✅ **IMPLEMENTED** - Complete UI/UX with components, services, and routing ready for backend integration

## 📋 Implementation State

### ✅ What's Fully Implemented
- [x] **Complete Angular 16+ Structure** - Modern architecture with Signals
- [x] **Feature Modules** - All wash management flows (nuevo, finalizar, buscar, perfil)
- [x] **Shell Navigation** - Bottom tab bar with dynamic routing
- [x] **Core Services** - Data API, stores, guards, utilities
- [x] **Shared Components** - Reusable UI elements (dialogs, spinner, notifications)
- [x] **Styling System** - SCSS structure with themes, variables, mixins
- [x] **Internationalization** - Spanish (es) and Basque (eus) support
- [x] **Assets Management** - Fonts, icons, configuration files
- [x] **Environment Configuration** - Dynamic API service configuration

### ⚠️ Integration Pending
- [ ] **Real HTTP Calls** - Currently using mock data service (`data-api.ts`)
- [ ] **JWT Authentication** - Login form exists, needs backend integration
- [ ] **Photo Upload** - FormData implementation needs real endpoint
- [ ] **Error Handling** - Global HTTP error interceptor needs backend error format

## 🏗️ Project Architecture

```
frontend/
├── 📄 package.json                    # Angular 16+ dependencies
├── 📄 angular.json                   # Build configuration  
├── 📄 tsconfig.json                  # TypeScript configuration
├── 
├── public/assets/                    # Static resources
│   ├── config/api_service.json      # ✅ API endpoint configuration
│   ├── fonts/                       # ✅ Havelock + Montserrat fonts
│   ├── i18n/                        # ✅ es.json, eus.json translations
│   └── images/icons/                 # ✅ SVG icons for navigation
│
├── src/app/                          # Main application
│   ├── 📄 app.ts                    # ✅ Root component with spinner/notifications
│   ├── 📄 app.routes.ts             # ✅ Route configuration
│   ├── 📄 app.config.ts             # ✅ App providers and services
│   │
│   ├── core/                         # ✅ Domain logic and shared services
│   │   ├── models/                   # ✅ TypeScript interfaces
│   │   │   ├── Lavado.ts            # Wash DTO/VO with status enum
│   │   │   ├── User.ts              # User profile model
│   │   │   ├── Photo.ts             # Photo preview model
│   │   │   ├── Maquina.ts           # Machine availability model
│   │   │   └── BusquedaLavadosFilter.ts # Search filter model
│   │   │
│   │   ├── services/                 # ✅ Business services
│   │   │   ├── data-api.ts          # ⚠️ MOCK DATA - Needs real HTTP service
│   │   │   ├── notification-store.ts # ✅ Toast notification system
│   │   │   ├── spinner-store.ts     # ✅ Global loading spinner
│   │   │   ├── perfil-store.ts      # ✅ User profile management
│   │   │   └── maquinas-store.ts    # ✅ Machine availability tracking
│   │   │
│   │   ├── guard/                    # ✅ Route protection
│   │   │   └── unsavedChanges.ts    # Form dirty state guard
│   │   │
│   │   └── utils/                    # ✅ Helper functions
│   │       └── dateUtils.ts         # Custom date formatting
│   │
│   ├── modules/                      # ✅ Feature modules
│   │   ├── shell/                    # ✅ Main layout and navigation
│   │   │   ├── pages/shell.*        # Bottom tab bar, title management
│   │   │   ├── custom-icons.ts      # Icon registry for tabs
│   │   │   ├── nav-tab-links.ts     # Tab configuration per profile
│   │   │   ├── title-store.ts       # Reactive page titles
│   │   │   └── navigation-store.ts  # Navigation abstraction
│   │   │
│   │   ├── inicio/                   # ✅ Dashboard/home page
│   │   │   └── pages/inicio.*       # Main wash management dashboard
│   │   │
│   │   ├── nuevo/                    # ✅ Start new wash flow
│   │   │   └── pages/nuevo.*        # Form: user, machine, PROTs, observation
│   │   │
│   │   ├── finalizar/                # ✅ Finish wash flow
│   │   │   ├── pages/finalizar.*    # Photo upload, validation, submit
│   │   │   ├── pages/finalizar-menu.* # Choose machine to finish
│   │   │   └── finalizar.routes.ts  # Nested routing
│   │   │
│   │   ├── buscar/                   # ✅ Search completed washes
│   │   │   ├── pages/buscar.*       # Filter form (PROT, user, date)
│   │   │   ├── pages/buscar-list.*  # Results display
│   │   │   └── buscar-store.ts      # Search state management
│   │   │
│   │   └── perfil/                   # ✅ User profile (placeholder)
│   │       └── pages/perfil.*       # Profile management UI
│   │
│   └── shared/components/            # ✅ Reusable UI components
│       ├── spinner/                  # Loading indicator
│       ├── toaster-notification/     # Toast messages with types
│       ├── confirm-dialog/           # Generic confirmation dialog
│       └── confirm-photo-dialog/     # Photo preview confirmation
│
└── styles/                           # ✅ SCSS architecture
    ├── base/                        # Variables, mixins, utilities
    └── global/                      # Global styles, themes, components
```

## 🎯 Key Features Implemented

### 🔐 Authentication Flow
- **Mechanism**: Keycloak OIDC redirect handled by Keycloak library
- **Components**: Keycloak service initializes session and refreshes tokens
- **Status**: ⚠️ UI ready, waiting for Keycloak configuration

### 🚿 Start New Wash (`/nuevo`)
- **Features**: 
  - ✅ User selection dropdown
  - ✅ Machine selection (filtered by availability)
  - ✅ QR Scanner modal with camera controls
  - ✅ Manual PROT entry with validation
  - ✅ PROT list management (add/remove)
  - ✅ Observation text input
  - ✅ Form validation and submission
- **Status**: ✅ Complete UI, needs real API calls

### 📸 Finish Wash (`/finalizar/:maq`)
- **Features**:
  - ✅ Load active wash for selected machine
  - ✅ User selection dropdown
  - ✅ Photo upload (camera + gallery)
  - ✅ Photo preview and management
  - ✅ Observation text input
  - ✅ Validation (requires ≥1 photo)
- **Status**: ✅ Complete UI, needs FormData upload to real endpoint

### 🔍 Search Washes (`/buscar`)
- **Features**:
  - ✅ PROT filter (multiple selection)
  - ✅ User filter (multiple selection)  
  - ✅ Date range picker (desde/hasta)
  - ✅ Results list with pagination
  - ✅ Search state management
- **Status**: ✅ Complete UI, needs real API integration

### 🏠 Dashboard (`/inicio`)
- **Features**:
  - ✅ Main wash management buttons
  - ✅ Machine status indicators
  - ✅ Dynamic button states (disabled when max washes reached)
  - ✅ Navigation to start/finish flows
- **Status**: ✅ Complete UI, needs real machine status API

## 🔧 Technology Stack

### Core Framework
```json
{
  "angular": "^16.0.0",
  "typescript": "~5.0.0",
  "rxjs": "~7.5.0"
}
```

### Key Dependencies
- **@angular/material** - UI components and theming
- **@ngx-translate** - Internationalization (es/eus)
- **@ionic/angular** - Mobile-friendly components (if used)
- **capacitor** - Camera and native device integration (if used)

### Development Tools
- **ESLint** + **Prettier** - Code quality and formatting
- **Jasmine** + **Karma** - Unit testing framework
- **Angular CLI** - Build and development tools

## 🚀 Development Setup

### Prerequisites
- Node.js 18+
- Angular CLI 16+
- VS Code with Angular extensions

### Quick Start
```bash
# Install dependencies
npm install

# Start development server
ng serve

# Run tests
npm run test

# Build for production
npm run build
```

### Environment Configuration

Update `public/assets/config/api_service.json`:
```json
{
  "apiBaseUrl": "https://localhost:7001/api"
}
```

## 🔌 Backend Integration Points

### 1. Replace Mock Data Service

**Current**: `core/services/data-api.ts` uses in-memory mock data
**Needs**: Real HTTP service with proper DTOs

```typescript
// Example transformation needed
export class RealDataApiService {
  constructor(private http: HttpClient) {}

  async startWash(dto: NewWashDto): Promise<WashingResponseDto> {
    return this.http.post<WashingResponseDto>('/api/washing', dto).toPromise();
  }

  async uploadPhoto(washId: number, file: File): Promise<void> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<void>(`/api/washing/${washId}/photos`, formData).toPromise();
  }
}
```

### 2. Keycloak Authentication Integration

**Components Ready**: Keycloak service bootstrap
**Needs**:
- Configure `keycloak-js` or `keycloak-angular` with realm/client settings
- AuthGuard to verify Keycloak session
- HTTP interceptor for token attachment

```typescript
// Example skeleton
export class AuthService {
  init(): Promise<boolean>;
  logout(): void;
  getToken(): string | undefined;
  isAuthenticated(): boolean;
}
```

### 3. Photo Upload System

**Current**: Base64 preview system implemented
**Needs**: FormData upload to `/api/washing/{id}/photos`

```typescript
// Current implementation in finalizar component
uploadPhoto(file: File) {
  const formData = new FormData();
  formData.append('file', file);
  // Send to real API endpoint
}
```

### 4. Error Handling

**Needs**: Global HTTP error interceptor
```typescript
export class ErrorInterceptor implements HttpInterceptor {
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError((error: HttpErrorResponse) => {
        // Handle 401, 403, 400, 500 errors
        // Show appropriate notifications
        return throwError(error);
      })
    );
  }
}
```

## 📱 Mobile Features

### QR Scanner
- **Implementation**: Modal with camera controls
- **Features**: Flash toggle, zoom, gallery import, close
- **Status**: ✅ UI complete, needs real QR scanning library integration

### Photo Capture
- **Implementation**: Camera and gallery access
- **Features**: Take photo, select from gallery, preview, delete
- **Status**: ✅ UI complete, needs real device camera integration

### Responsive Design
- **Implementation**: Mobile-first approach with touch-friendly interfaces
- **Features**: Bottom navigation, large touch targets, optimized layouts
- **Status**: ✅ Complete responsive design

## 🌐 Internationalization

### Languages Supported
- **Spanish (es)**: `public/assets/i18n/es.json` ✅
- **Basque (eus)**: `public/assets/i18n/eus.json` ✅

### Usage
```typescript
// In components
constructor(private translate: TranslateService) {}

// Switch language
this.translate.use('es'); // or 'eus'

// Use translations
this.translate.get('NUEVO_LAVADO').subscribe(text => console.log(text));
```

## 🎨 Styling System

### SCSS Architecture
```scss
// styles/base/
_variables.scss    // Colors, spacing, breakpoints
_mixins.scss       // Reusable SCSS functions
_index.scss        // Base imports

// styles/global/  
_theme.scss        // Material theme customization
_buttons.scss      // Button styles
_dialog.scss       // Modal and dialog styles
_structure.scss    // Layout utilities
```

### Custom Fonts
- **Havelock Titling** - Headers and branding
- **Montserrat** - Body text and UI elements

## 🧪 Testing Strategy

### Unit Tests
```bash
# Run tests
npm run test

# Run tests with coverage
npm run test:coverage

# Run tests in CI mode
npm run test:ci
```

### Component Testing
Each component has corresponding `.spec.ts` files for unit testing.

### E2E Testing (Recommended)
Consider adding Cypress or Playwright for end-to-end testing of complete wash flows.

## 🔄 State Management

### Store Pattern
- **notification-store.ts** - Toast notifications
- **spinner-store.ts** - Global loading states
- **perfil-store.ts** - User profile data
- **maquinas-store.ts** - Machine availability
- **buscar-store.ts** - Search results and filters

### Data Flow
```
Component → Store → Service → Backend API
    ↑                               ↓
    ←——————— Store Update ←————————————
```

## 🚀 Production Build

```bash
# Build for production
npm run build

# Serve production build locally
npx http-server dist/controlmat-desk
```

### Build Optimization
- **Tree Shaking** - Removes unused code
- **AOT Compilation** - Ahead-of-time template compilation  
- **Bundle Splitting** - Lazy loading for feature modules
- **Asset Optimization** - Image compression and minification

## 📚 Related Documentation

- [Backend API](../backend/README.md) - .NET 6 CQRS implementation
- [Architecture Guide](../docs/architecture/) - System design patterns
- [API Integration](../docs/api/) - Endpoint specifications
- [Database Schema](../docs/database/) - Data models and relationships

---

**Ready for Backend Integration!** 🔌 The frontend is complete and waiting for real API connections to transform from a beautiful prototype into a fully functional wash management system.