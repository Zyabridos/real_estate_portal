export type PropertyImageObject = {
  src?: string | null;
  url?: string | null;
};

export type PropertyImageSource = {
  mainImageUrl?: string | null;
  imageUrls?: Array<string | null> | null;
  images?: Array<string | PropertyImageObject | null | undefined> | null;
};

type GetPropertyImageUrlsOptions = {
  fallbackImage: string;
  preferImageUrlsFirst?: boolean;
};

function normalizeUrl(value?: string | null): string | null {
  const normalized = value?.trim();
  return normalized ? normalized : null;
}

function pushUnique(target: string[], value?: string | null): void {
  const normalized = normalizeUrl(value);

  if (normalized && !target.includes(normalized)) {
    target.push(normalized);
  }
}

function pushImageUrls(
  target: string[],
  imageUrls?: Array<string | null> | null,
): void {
  for (const imageUrl of imageUrls ?? []) {
    pushUnique(target, imageUrl);
  }
}

function pushLegacyImages(
  target: string[],
  images?: Array<string | PropertyImageObject | null | undefined> | null,
): void {
  for (const image of images ?? []) {
    if (!image) continue;

    if (typeof image === "string") {
      pushUnique(target, image);
      continue;
    }

    pushUnique(target, image.src ?? image.url);
  }
}

export function getPropertyImageUrls(
  source: PropertyImageSource | null | undefined,
  options: GetPropertyImageUrlsOptions,
): string[] {
  const urls: string[] = [];

  if (options.preferImageUrlsFirst) {
    pushImageUrls(urls, source?.imageUrls);
    pushUnique(urls, source?.mainImageUrl);
  } else {
    pushUnique(urls, source?.mainImageUrl);
    pushImageUrls(urls, source?.imageUrls);
  }

  pushLegacyImages(urls, source?.images);

  if (!urls.length) {
    urls.push(options.fallbackImage);
  }

  return urls;
}

export function toGalleryImages(
  urls: string[],
  title?: string | null,
): Array<{ src: string; alt: string }> {
  const normalizedTitle = title?.trim() || "Property image";

  return urls.map((src, index) => ({
    src,
    alt: index === 0 ? normalizedTitle : `${normalizedTitle} ${index + 1}`,
  }));
}
