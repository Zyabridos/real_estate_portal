
# CMS (Sanity Studio)

This folder contains the Sanity Studio v3 used as a headless CMS for the Real Estate project.

The CMS is responsible for marketing and editorial content, such as blog articles, guides for buyers/sellers, and static informational pages.  
Business data (properties, brokers, leads) lives in the backend API and database.

---

## Tech Stack

- **Sanity Studio v3**
- **Docker / Docker Compose**

---

## What lives here

Typical CMS content:
- Blog articles (guides for buyers and sellers)
- Editorial content
- Author profiles
- Categories and tags
- Content linked to property types or user segments

This CMS does **not** store business or user-generated data.

---

## Folder Structure
```yaml
cms/
├── schemas/ # Sanity schemas (article, author, category, etc.)
├── sanity.config.ts # Sanity Studio configuration
├── sanity.cli.ts # Sanity CLI configuration
├── package.json
├── vite.config.ts
└── README.md
```

---

## Environment Variables

Rename `.env` file:
```bash
mv .env.example .env
```
Then set correct values to keys.

## Running locally (without Docker)
```bash
npm install
npm run dev
```
Sanity Studio will be available at:

```arduino
http://localhost:3333
```
## Running with Docker (recommended)
From the project root:

```bash
docker compose up cms
```
Or start everything:

```bash
docker compose up
```
Sanity Studio will be available at:

```arduino
http://localhost:3333
```
## Development Notes
- Sanity Studio uses Vite under the hood
- Hot reload is enabled by default
- Schemas are loaded automatically on change
- Schema changes require Studio restart only in rare cases

## Notes
- This CMS is intentionally decoupled from business logic
- All content is fetched via Sanity APIs (GROQ)