# SUMISAN Wash Management Frontend

This is the **Angular 16+ frontend** for the SUMISAN Wash Management System — a warehouse tool used to manage the washing of surgical instrument kits (PROTs), including scanning, photo documentation, and wash tracking.

The app integrates with a .NET 6 backend and supports **Keycloak OIDC authentication** for role-based warehouse users.

---

## 📁 Project Structure & Folders

### `src/app/`

Root of the Angular app. Contains all core logic, feature modules, and shared components.

#### `app.ts`

Root component with spinner and notification overlays.

#### `app.routes.ts`

Defines the route structure of the application.

---

### `core/`

Contains domain models, shared services, guards, and utilities.

* `models/`
  * `Lavado.ts`: Defines `LavadoVO`, `LavadoDTO`, `LavadoStatus`
  * `User.ts`: User DTO/VO for operarios
  * `Photo.ts`: DTO/VO for photo previews
  * `Maquina.ts`: Machine VO with availability flag
  * `BusquedaLavadosFilter.ts`: Search DTO/VO with date range

* `services/`
  * `data-api.ts`: Central service for HTTP API calls to backend
  * `auth.service.ts`: Keycloak integration service
  * `notification-store.ts`: Emits toasts
  * `spinner-store.ts`: Global loading spinner control
  * `perfil-store.ts`: Stores current user profile from Keycloak
  * `maquinas-store.ts`: Tracks machine 1 and 2 availability

* `guard/`
  * `auth.guard.ts`: Route guard for Keycloak authentication
  * `unsavedChanges.ts`: Route guard that prompts if form is dirty before leaving

* `interceptors/`
  * `auth.interceptor.ts`: Automatically adds Bearer token from Keycloak to HTTP requests

* `languages/`
  * `languages.ts`: Language definitions used for translation setup

* `utils/`
  * `dateUtils.ts`: Unused but defines a custom date format (`DD/MM/YYYY`)

---

### `modules/`

Encapsulates feature-specific UI pages.

#### `inicio/`
* Entry point or dashboard. Component: `inicio.ts`

#### `nuevo/`
* Start a new wash
* Files:
  * `nuevo.ts`: Form with user, machine, PROTs, observation
  * `nuevo.html`: UI with manual/QR input and start button
  * `nuevo.scss`: Styling

#### `finalizar/`
* Finish an in-progress wash
* Files:
  * `finalizar.ts`: Photo preview, validation, submit
  * `finalizar.html`: UI for image upload, notes, submit
  * `finalizar-menu.ts`: Lets user choose which machine's wash to finish

#### `buscar/`
* Search past washes
* Files:
  * `buscar.ts`: Form to filter by prot, user, date
  * `buscar-list.ts`: Displays results from `BuscarStore`

#### `perfil/`
* User profile management (reads from Keycloak user info)

#### `shell/`
* Main layout: tabs, title, navigation
* Files:
  * `shell.ts`: Registers icons, tracks active tab, redirects to `/inicio`
  * `custom-icons.ts`: Icon registry
  * `nav-tab-links.ts`: Defines tab links per profile
  * `title-store.ts`: Reactive store for page titles
  * `navigation-store.ts`: Navigation abstraction

---

### `shared/components/`

Reusable UI elements across the app.

* `spinner/`: Circular loader
* `toaster-notification/`: Toast overlay with types (success, error...)
* `confirm-dialog/`: Generic yes/no confirmation
* `confirm-photo-dialog/`: Preview and confirm uploaded photo

---

### `environments/`

Contains environment and config loading logic.

* `environment.ts`: Loads `globalEnvironment()` from `_environment.ts`
* `_environment.ts`: Uses `/assets/config/api_service.json` and `/assets/config/keycloak.json`

---

### `public/assets/`

Static assets and configuration

* `config/api_service.json`: Defines base API URL
* `config/keycloak.json`: Keycloak configuration (realm, client ID, URL)
* `i18n/es.json`: Spanish translations
* `i18n/eus.json`: Basque translations
* `fonts/`: Custom font files
* `images/icons/`: SVG icons used by shell tabs

---

## 🔐 Authentication Integration (Keycloak OIDC)

### Setup

The app uses **keycloak-angular** for OIDC integration:

```bash
npm install keycloak-angular keycloak-js
```

### Configuration

**`assets/config/keycloak.json`**:
```json
{
  "url": "https://your-keycloak-server",
  "realm": "sumisan",
  "clientId": "sumisan-frontend"
}
```

### Key Components

#### `auth.service.ts`
* Initializes Keycloak instance
* Handles login/logout flows
* Provides token and user info
* Manages token refresh

#### `auth.guard.ts`
* Protects routes requiring authentication
* Redirects to Keycloak login if not authenticated
* Checks for required roles (e.g., `WarehouseUser`)

#### `auth.interceptor.ts`
* Automatically adds `Authorization: Bearer <token>` to API calls
* Handles token refresh on 401 responses

### Authentication Flow

1. User navigates to protected route
2. `AuthGuard` checks authentication status
3. If not authenticated, redirect to Keycloak login
4. After successful login, return to original route
5. All API calls include Bearer token via interceptor

---

## 💡 How Each Feature Works

### ✅ Start Wash Flow (`/nuevo`)

* User selects operario and machine
* Adds PROTs (manual input or QR scan)
* Adds observation
* Confirms
* Sends `NewWashDto` to `POST /api/washing`

### ✅ Finish Wash Flow (`/finalizar/:maq`)

* Loads active wash for machine
* Lets user upload images via `FormData`
* Adds observation
* Confirms
* Sends photos to `POST /api/washing/{id}/photos`
* Sends finish data to `PUT /api/washing/{id}/finish`

### ✅ Search Flow (`/buscar`)

* Filters by:
  * `prots[]`
  * `operarios[]`
  * `fechaDesde`, `fechaHasta`
* Results retrieved from backend API
* Results stored in `BuscarStore`

---

## ⚠️ Frontend vs Backend Integration

| Area                | Frontend                              | Backend                                   |
| ------------------- | ------------------------------------- | ---------------------------------------- |
| **Authentication**  | Keycloak OIDC redirect + token storage | JWT Bearer validation against Keycloak   |
| **Authorization**   | Role checks via Keycloak claims       | `[Authorize(Roles = "WarehouseUser")]`   |
| **Photo Upload**    | FormData multipart upload             | `POST /api/washing/{id}/photos`          |
| **Start Wash DTO**  | `NewWashDto` with `protEntries[]`     | Handled by `StartWashCommand`            |
| **Finish Wash DTO** | `FinishWashDto` (separate from photos)| Handled by `FinishWashCommand`           |
| **Data Source**     | HTTP calls via `HttpClient`          | RESTful API endpoints                     |
| **Validation**      | Basic frontend + backend validation   | FluentValidation + business rules        |

---

## 🌐 i18n

Translation keys are managed via `@ngx-translate`.

```ts
translate.use(Languages.ES);
```

Defined in:
* `assets/i18n/es.json`
* `assets/i18n/eus.json`

---

## 🔧 Dependencies

Key Angular packages:

```json
{
  "@angular/core": "^16.0.0",
  "@angular/common": "^16.0.0",
  "@angular/router": "^16.0.0",
  "keycloak-angular": "^14.0.0",
  "keycloak-js": "^21.0.0",
  "@ngx-translate/core": "^14.0.0",
  "rxjs": "^7.8.0"
}
```

---

## 🚀 Setup Commands

```bash
npm install        # Install dependencies
ng serve           # Start dev server
ng build           # Production build
ng lint            # Run linter
```

---

## ✅ Completed Integration Items

* ✅ Replace mock `data-api.ts` with real `HttpClient` service methods
* ✅ Implement Keycloak OIDC integration with `keycloak-angular`
* ✅ Add `AuthInterceptor` to inject JWT into headers
* ✅ Add `AuthGuard` to protect routes like `/nuevo` and `/finalizar`
* ✅ Refactor DTOs to match backend: `NewWashDto`, `FinishWashDto`
* ✅ Handle photo upload using `FormData` to backend `/api/washing/{id}/photos`
* ✅ Implement validation per backend FluentValidation rules
* ✅ Connect to real API endpoints defined in `api_service.json`
* ✅ Wire up `MY_DATE_FORMATS` for localized date pickers
* ✅ Connect `BuscarStore` and `buscar-list.ts` to real API responses

---

## 🛡️ Security Notes

* JWT tokens stored in memory (not localStorage) for security
* Automatic token refresh handled by Keycloak
* HTTPS required in production
* CORS configured for Keycloak and API domains
* No sensitive data stored in frontend

---

## 📄 License

Internal project – ECNA / SUMISAN  
Not open source.