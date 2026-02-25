import * as fs from "node:fs";
import * as path from "node:path";
import { dirname } from "path";
import { fileURLToPath } from "url";

const __dirname = dirname(fileURLToPath(import.meta.url));

const parseEnvFile = (content: string): Record<string, string> => {
  const out: Record<string, string> = {};
  for (const line of content.split(/\r?\n/)) {
    const trimmed = line.trim();
    if (!trimmed || trimmed.startsWith("#")) continue;

    const idx = trimmed.indexOf("=");
    if (idx === -1) continue;

    const key = trimmed.slice(0, idx).trim();
    const value = trimmed.slice(idx + 1).trim();
    out[key] = value;
  }
  return out;
};

function findSeedEnvPath(repoRoot: string): string {
  const candidates = [
    path.join(repoRoot, "scripts/.seed/seed.env"), // основной путь (как в твоих seed-скриптах)
    path.join(repoRoot, ".env.seed"),              // fallback, если ты это используешь
    path.join(repoRoot, ".seed/seed.env"),         // fallback (если вдруг поменяешь структуру)
  ];

  const existing = candidates.find((p) => fs.existsSync(p));
  if (!existing) {
    throw new Error(
      `seed env file not found. Tried:\n- ${candidates.join("\n- ")}\nRun seeding first (e.g. make seed).`
    );
  }

  return existing;
}

function readSeedEnv(): Record<string, string> {
  const repoRoot = path.resolve(__dirname, "../../../..");
  const seedEnvPath = findSeedEnvPath(repoRoot);

  const content = fs.readFileSync(seedEnvPath, "utf-8");
  return parseEnvFile(content);
}

export function getSeedPropertyId(): string {
  const env = readSeedEnv();
  const propertyId = env.PROPERTY_ID;
  if (!propertyId) throw new Error(`PROPERTY_ID is missing in seed env file`);
  return propertyId;
}

export function getSeedAgencyIds(): { agency1Id: string; agency2Id: string; agency3Id: string } {
  const env = readSeedEnv();

  const a1 = env.AGENCY1_ID;
  const a2 = env.AGENCY2_ID;
  const a3 = env.AGENCY3_ID;

  if (!a1 || !a2 || !a3) {
    throw new Error(`AGENCY1_ID/AGENCY2_ID/AGENCY3_ID missing in seed env file`);
  }

  return { agency1Id: a1, agency2Id: a2, agency3Id: a3 };
}
