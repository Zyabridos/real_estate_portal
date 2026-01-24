import { sanityClient } from "@/shared/cms/sanityClient";
import type { ArticleDetailsDto, ArticleListItemDto, CategoryDto } from "@/shared/types/blog";
import { blogQueries } from "@/shared/api/query/blogQueries";

// TODO: move normalizers to their own file/category

export class BlogServiceError extends Error {
  public readonly code = "SANITY_FETCH_FAILED" as const;
  public readonly cause?: unknown;

  constructor(message: string, cause?: unknown) {
    super(message);
    this.name = "BlogServiceError";
    this.cause = cause;
  }
}

const isRecord = (v: unknown): v is Record<string, unknown> =>
  typeof v === "object" && v !== null;

const readString = (obj: Record<string, unknown>, key: string): string | undefined => {
  const v = obj[key];
  return typeof v === "string" ? v : undefined;
};

const readOptionalString = (obj: Record<string, unknown>, key: string): string | undefined => {
  const v = obj[key];
  return v == null ? undefined : typeof v === "string" ? v : undefined;
};

const normalizeCategory = (input: unknown): CategoryDto | null => {
  if (!isRecord(input)) return null;

  const id = readString(input, "_id") ?? readString(input, "id");
  const title = readString(input, "title");
  const slug = readString(input, "slug");

  if (!id || !title || !slug) return null;

  return { id, title, slug };
};

const normalizeAuthor = (input: unknown): { name: string } | undefined => {
  if (!isRecord(input)) return undefined;

  const name = readString(input, "name");
  if (!name) return undefined;

  return { name };
};

const normalizeArticleListItem = (input: unknown): ArticleListItemDto | null => {
  if (!isRecord(input)) return null;

  const id = readString(input, "_id") ?? readString(input, "id");
  const title = readString(input, "title");
  const slug = readString(input, "slug");

  if (!id || !title || !slug) return null;

  const excerpt = readOptionalString(input, "excerpt");
  const publishedAt = readOptionalString(input, "publishedAt");
  const relatedPropertyType = readOptionalString(input, "relatedPropertyType");

  const author = normalizeAuthor(input["author"]);

  const rawCategories = Array.isArray(input["categories"]) ? input["categories"] : undefined;
  const categories = rawCategories
    ?.map(normalizeCategory)
    .filter((x): x is CategoryDto => x !== null);

  const mainImageUrl = readOptionalString(input, "mainImageUrl");

  return {
    id,
    title,
    slug,
    excerpt,
    publishedAt,
    relatedPropertyType,
    author,
    categories,
    mainImageUrl,
  };
};

const normalizePortableText = (input: unknown): ArticleDetailsDto["content"] => {
  if (!Array.isArray(input)) return undefined;

  return input
    .filter((b) => isRecord(b) && readString(b, "_type") === "block")
    .map((b) => {
      const children = Array.isArray(b.children)
        ? b.children
          .filter((c: any) => isRecord(c) && readString(c, "_type") === "span")
          .map((c: any) => ({
            _key: readOptionalString(c, "_key"),
            _type: "span" as const,
            text: readOptionalString(c, "text"),
          }))
        : [];

      return {
        _key: readOptionalString(b, "_key"),
        _type: "block" as const,
        style: readOptionalString(b, "style"),
        children,
      };
    });
};

const normalizeArticleDetails = (input: unknown): ArticleDetailsDto | null => {
  if (!isRecord(input)) return null;

  const base = normalizeArticleListItem(input);
  if (!base) return null;

  const content = normalizePortableText(input["content"]);

  return {
    ...base,
    content,
  };
};

// --- Public API ---

export const blogService = {
  // Returns categories for /blog filter.
  async getCategories(): Promise<CategoryDto[]> {
    const { query, params } = blogQueries.getCategories();

    try {
      const result = await sanityClient.fetch<unknown>(query, params);

      if (!Array.isArray(result)) {
        throw new BlogServiceError("Sanity returned non-array categories response.");
      }

      return result
        .map(normalizeCategory)
        .filter((x): x is CategoryDto => x !== null);
    } catch (err) {
      if (err instanceof BlogServiceError) throw err;
      throw new BlogServiceError("Failed to fetch categories from Sanity.", err);
    }
  },

  // Returns articles for /blog list.
  async getArticles(categorySlug?: string | null): Promise<ArticleListItemDto[]> {
    const { query, params } = blogQueries.getArticles(categorySlug);

    try {
      const result = await sanityClient.fetch<unknown>(query, params);

      if (!Array.isArray(result)) {
        throw new BlogServiceError("Sanity returned non-array articles response.");
      }

      return result
        .map(normalizeArticleListItem)
        .filter((x): x is ArticleListItemDto => x !== null);
    } catch (err) {
      if (err instanceof BlogServiceError) throw err;
      throw new BlogServiceError("Failed to fetch articles from Sanity.", err);
    }
  },

  // Single article for /blog/:slug (returns null if not found).
  async getArticleBySlug(slug: string): Promise<ArticleDetailsDto | null> {
    const { query, params } = blogQueries.getArticleBySlug(slug);

    try {
      const result = await sanityClient.fetch<unknown>(query, params);

      // GROQ [0] returns null if not found
      return normalizeArticleDetails(result);
    } catch (err) {
      if (err instanceof BlogServiceError) throw err;
      throw new BlogServiceError("Failed to fetch article by slug from Sanity.", err);
    }
  },
} as const;
