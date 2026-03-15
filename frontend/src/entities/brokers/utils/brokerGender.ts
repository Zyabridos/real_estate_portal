import femalePicture from "@/assets/images/defaultPictureFemale.png";
import malePicture from "@/assets/images/defaultPictureMale.png";
import neutralPicture from "@/assets/images/defaultPictureNeutral.png";

export type NormalizedBrokerGender =
  | "male"
  | "female"
  | "other"
  | "unspecified";

export function normalizeBrokerGender(value: unknown): NormalizedBrokerGender {
  if (typeof value === "number") {
    switch (value) {
      case 0:
        return "male";
      case 1:
        return "female";
      case 2:
        return "other";
      case 3:
        return "unspecified";
      default:
        return "unspecified";
    }
  }

  if (typeof value === "string") {
    const normalized = value.trim().toLowerCase();

    switch (normalized) {
      case "0":
      case "male":
        return "male";

      case "1":
      case "female":
        return "female";

      case "2":
      case "other":
        return "other";

      case "3":
      case "unspecified":
        return "unspecified";

      default:
        return "unspecified";
    }
  }

  return "unspecified";
}

export function getBrokerGenderLabelKey(value: unknown): string {
  const normalized = normalizeBrokerGender(value);
  return `brokers:common.gender.${normalized}`;
}

export function getBrokerFallbackImage(value: unknown): string {
  const normalized = normalizeBrokerGender(value);

  if (normalized === "male") {
    return malePicture;
  }

  if (normalized === "female") {
    return femalePicture;
  }

  return neutralPicture;
}
