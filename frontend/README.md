
Frontend application for the **Real Estate Mini Portal** project.

This app is responsible for rendering the user interface, fetching data from the backend API, and displaying editorial content from Sanity CMS.

---

## Tech Stack

- Vue 3
- TypeScript
- Vue Router
- Pinia
- Tailwind CSS
- Vite

---

## Responsibilities

- Display property listings and property details
- Show brokers and their properties
- Submit lead (interest) forms
- Render blog articles from Sanity CMS
- Handle routing, UI state, and basic error handling

---

## Project Structure

```text
src/
  assets/        # Static assets
  components/    # Reusable UI components
  pages/         # Route pages
  router/        # Vue Router configuration
  stores/        # Pinia stores
  services/      # API & CMS clients
  styles/        # Global styles
```
## Development
Install dependencies:

```bash
npm install
```
Start the development server:
```bash
npm run dev
```
The application will be available at:

```arduino
http://localhost:5173
```

## Build
Create a production build:

```bash
npm run build
```
Preview the production build locally:

```bash
npm run preview
```

## Notes
- Authentication is intentionally not implemented
- This project focuses on architecture, integrations, and UI structure
- Detailed implementation notes are available in the code and commits