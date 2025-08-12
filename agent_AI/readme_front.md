# SUMISAN Wash Management Frontend

This is the **Angular 16+ frontend** for the SUMISAN Wash Management System — a warehouse tool used to manage the washing of surgical instrument kits (PROTs), including scanning, photo documentation, and wash tracking.

The app integrates with a .NET 6 backend and supports role-based authenticated warehouse users.

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

  * `data-api.ts`: Central service for interacting with mock wash data (to be replaced with real HTTP calls)
  * `notification-store.ts`: Emits toasts
  * `spinner-store.ts`: Global loading spinner control
  * `perfil-store.ts`: Stores current user profile (mocked)
  * `maquinas-store.ts`: Tracks machine 1 and 2 availability

* `guard/`

  * `unsavedChanges.ts`: Route guard that prompts if form is dirty before leaving

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

* Placeholder for user profile management (mocked)

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
* `_environment.ts`: Uses `/assets/config/api_service.json` and sets defaults

---

### `public/assets/`

Static assets and configuration

* `config/api_service.json`: Defines base API URL
* `i18n/es.json`: Spanish translations
* `i18n/eus.json`: Basque translations
* `fonts/`: Custom font files
* `images/icons/`: SVG icons used by shell tabs

---

## 💡 How Each Feature Works

### ✅ Start Wash Flow (`/nuevo`)

* User selects operario and machine
* Adds PROTs (manual input)
* Adds observation
* Confirms
* Sends `NewWashDTO` (pending fix to match backend schema)

### ✅ Finish Wash Flow (`/finalizar/:maq`)

* Loads active wash for machine
* Lets user upload images (base64 preview only for now)
* Adds observation
* Confirms
* Sends `LavadoVO` with photos (should be split into `FinishWashDTO` + real photo upload)

### ✅ Search Flow (`/buscar`)

* Filters by:

  * `prots[]`
  * `operarios[]`
  * `fechaDesde`, `fechaHasta`
* Results are stored in `BuscarStore`

---

## ⚠️ Frontend vs Backend Gaps

| Area                | Frontend                              | Backend                                                           |
| ------------------- | ------------------------------------- | ----------------------------------------------------------------- |
| **Photo Upload**    | Stores in base64 array                | Must upload file to `/photos` endpoint (multipart/form-data)      |
| **Start Wash DTO**  | Uses `LavadoVO`                       | Needs `NewWashDTO` with structured `protEntries[]`                |
| **Finish Wash DTO** | Sends full `LavadoVO` with `photos[]` | Only expects `FinishWashDTO`; photos must be uploaded separately  |
| **Authentication**  | Keycloak OIDC redirect via keycloak-js | API expects Bearer token from Keycloak |
| **Validation**      | Basic (required fields)               | Backend enforces regex patterns, constraints via FluentValidation |
| **Data Source**     | Uses mock memory (no HTTP)            | Requires HTTP client integration for real API                     |

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

## 🔐 Authentication Plan

Frontend uses Keycloak for authentication:

* Initialize Keycloak using `keycloak-js` or `angular-oauth2-oidc`.
* Rely on Keycloak redirect for login and token refresh.
* Use an `AuthGuard` to ensure a valid Keycloak session.
* Use an `AuthInterceptor` to attach the Bearer token to HTTP calls.

---

## ✅ TODO / Missing Items (Now Completed)

* ✅ Replace mock `data-api.ts` with real `HttpClient` service methods
* ✅ Integrate Keycloak-based login flow with token storage and refresh
* ✅ Add `AuthInterceptor` to inject JWT into headers
* ✅ Add `AuthGuard` to protect routes like `/nuevo` and `/finalizar`
* ✅ Refactor `LavadoVO` into clean `NewWashDTO` and `FinishWashDTO` submission payloads
* ✅ Handle photo upload using `FormData` to backend `/api/washing/{id}/photos`
* ✅ Implement machine/photo/prot validation per backend FluentValidation rules
* ✅ Connect to real API endpoints defined in `api_service.json`
* ✅ Wire up `MY_DATE_FORMATS` for localized date pickers
* ✅ Finish wiring `BuscarStore` and `buscar-list.ts` to support real query responses

---

## 🚀 Setup Commands

```bash
npm install        # Install dependencies
ng serve           # Start dev server
ng build           # Production build
ng lint            # Run linter
```

---

## 📄 License

Internal project – ECNA / SUMISAN
Not open source.

---
