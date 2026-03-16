import fs from "node:fs";
import path from "node:path";
import { fileURLToPath } from "node:url";

type SeedEnv = Record<string, string>;

const currentFilePath = fileURLToPath(import.meta.url);
const currentDirPath = path.dirname(currentFilePath);

const findSeedEnvPath = (): string => {
  const candidates = [
    path.resolve(process.cwd(), "../scripts/.seed/seed.env"),
    path.resolve(process.cwd(), "../../scripts/.seed/seed.env"),
    path.resolve(process.cwd(), "../../../scripts/.seed/seed.env"),
    path.resolve(currentDirPath, "../../../../scripts/.seed/seed.env"),
  ];

  const found = candidates.find((candidate) => fs.existsSync(candidate));

  if (!found) {
    throw new Error(
      [
        "Seed env file was not found.",
        "Expected one of:",
        ...candidates.map((x) => `- ${x}`),
      ].join("\n"),
    );
  }

  return found;
};

const parseSeedEnvFile = (): SeedEnv => {
  const filePath = findSeedEnvPath();
  const raw = fs.readFileSync(filePath, "utf-8");

  return raw
    .split(/\r?\n/)
    .map((line) => line.trim())
    .filter((line) => line.length > 0 && !line.startsWith("#"))
    .reduce<SeedEnv>((acc, line) => {
      const eqIndex = line.indexOf("=");
      if (eqIndex === -1) return acc;

      const key = line.slice(0, eqIndex).trim();
      const value = line.slice(eqIndex + 1).trim();

      acc[key] = value;
      return acc;
    }, {});
};

const seedEnv = parseSeedEnvFile();

const getRequiredValue = (name: string, fallbackNames: string[] = []): string => {
  const keys = [name, ...fallbackNames];

  for (const key of keys) {
    const fromProcess = process.env[key];
    if (fromProcess && fromProcess.trim() !== "") return fromProcess.trim();

    const fromFile = seedEnv[key];
    if (fromFile && fromFile.trim() !== "") return fromFile.trim();
  }

  throw new Error(
    `Missing required seed env: ${name}${
      fallbackNames.length ? ` (fallbacks: ${fallbackNames.join(", ")})` : ""
    }`,
  );
};

const parseRequiredNumber = (name: string, fallbackNames: string[] = []): number => {
  const value = getRequiredValue(name, fallbackNames);
  const parsed = Number(value);

  if (!Number.isInteger(parsed) || parsed <= 0) {
    throw new Error(`Invalid numeric seed env: ${name}="${value}"`);
  }

  return parsed;
};

export const getSeedPropertyId = (): number =>
  parseRequiredNumber("PROPERTY_ID", ["PROPERTY1_ID"]);

export const getSeedAgencyIds = () => ({
  agency1Id: parseRequiredNumber("AGENCY1_ID"),
  agency2Id: parseRequiredNumber("AGENCY2_ID"),
  agency3Id: parseRequiredNumber("AGENCY3_ID"),
});

export const getSeedBrokerIds = () => ({
  broker1Id: parseRequiredNumber("BROKER1_ID"),
  broker2Id: parseRequiredNumber("BROKER2_ID"),
  broker3Id: parseRequiredNumber("BROKER3_ID"),
});
