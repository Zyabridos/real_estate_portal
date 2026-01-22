import * as fs from "node:fs";
import * as path from "node:path";

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

export function getSeedPropertyId(): string {
  const repoRoot = path.resolve(__dirname, "../../../..");
  const seedEnvPath = path.join(repoRoot, "scripts/.seed/seed.env");

  if (!fs.existsSync(seedEnvPath)) {
    throw new Error(
      `seed.env not found at: ${seedEnvPath}. Ensure the path is correct and you have seeded the DB`
    );
  }

  const content = fs.readFileSync(seedEnvPath, "utf-8");
  const env = parseEnvFile(content);

  const propertyId = env.PROPERTY_ID;
  if (!propertyId) {
    throw new Error(`PROPERTY_ID is missing in ${seedEnvPath}`);
  }

  return propertyId;
}
