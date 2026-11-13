# Servicer frontend

React (Vite) + Tailwind v4 + React Router, talking to the .NET API.

## Run

```
npm install
cp .env.example .env     # set VITE_API_URL to your API's https URL
npm run dev              # http://localhost:5173
```

## Before it will talk to your API

1. **CORS.** In `Program.cs`, before `builder.Build()`:
   ```csharp
   builder.Services.AddCors(o => o.AddPolicy("web", p =>
       p.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod()));
   ```
   and after `var app = builder.Build();`, before `UseAuthentication()`:
   ```csharp
   app.UseCors("web");
   ```
2. **Routes and field names.** Open `src/api/endpoints.js` and make every route match your controllers.
   Request field names (email, password, problemDescription, ...) are in the pages and `src/api/services.js`.
3. **Role in the token.** The UI reads the role from the JWT (`Admin`, `Worker`, `User`).
   Add the role claim when you build the token (this is your pending role-based auth step).

## Mocked for now

Worker search, availability toggle, payments, ratings and the admin dashboard use `src/api/mock.js`
(stored in localStorage) because those endpoints do not exist yet. Flags are in `USE_MOCK`
in `src/api/endpoints.js`; replace each mock call in `src/api/services.js` when the real endpoint is ready.
