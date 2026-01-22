// Note:
// Safe params: we NEVER interpolate user input into the query string.
// We pass values via GROQ params ($slug, $categorySlug, ...).

export type GroqQuery<TParams extends Record<string, unknown>> = {
  query: string;
  params: TParams;
};

type CategoriesParams = Record<never, never>;

type ArticlesParams = {
  categorySlug?: string | null;
};

type ArticleBySlugParams = {
  slug: string;
};

type RelatedArticlesParams = {
  relatedPropertyType: string;
  excludeSlug?: string | null;
  limit: number;
};

const ARTICLE_LIST_PROJECTION = `
{
  _id,
  title,
  "slug": slug.current,
  excerpt,
  publishedAt,
  relatedPropertyType,
  author->{
    name
  },
  categories[]->{
    _id,
    title,
    "slug": slug.current
  },
  mainImage{
    alt,
    "url": asset->url
  }
}
`.trim();

export const blogQueries = {
  getCategories(): GroqQuery<CategoriesParams> {
    return {
      query: /* groq */ `
        *[_type == "category"]
          | order(title asc)
          {
            _id,
            title,
            "slug": slug.current
          }
      `.trim(),
      params: {},
    };
  },

  // Articles for /blog list. Optional filter by category slug via params.
  getArticles(categorySlug?: string | null): GroqQuery<ArticlesParams> {
    return {
      query: /* groq */ `
        *[
          _type == "article"
          && (
            !defined($categorySlug)
            || $categorySlug == null
            || $categorySlug == ""
            || $categorySlug in categories[]->slug.current
          )
        ]
        | order(publishedAt desc, _createdAt desc)
        ${ARTICLE_LIST_PROJECTION}
      `.trim(),
      params: { categorySlug: categorySlug ?? null },
    };
  },

   // Single article for /blog/:slug
  getArticleBySlug(slug: string): GroqQuery<ArticleBySlugParams> {
    return {
      query: /* groq */ `
        *[_type == "article" && slug.current == $slug][0]
        {
          _id,
          title,
          "slug": slug.current,
          excerpt,
          publishedAt,
          relatedPropertyType,
          author->{
            name
          },
          categories[]->{
            _id,
            title,
            "slug": slug.current
          },
          mainImage{
            alt,
            "url": asset->url
          },
          content
        }
      `.trim(),
      params: { slug },
    };
  },

  // Related articles by relatedPropertyType
  getRelatedArticles(
    relatedPropertyType: string,
    excludeSlug?: string | null,
    limit = 3,
  ): GroqQuery<RelatedArticlesParams> {
    return {
      query: `
        *[
          _type == "article"
          && relatedPropertyType == $relatedPropertyType
          && (
            !defined($excludeSlug)
            || $excludeSlug == null
            || $excludeSlug == ""
            || slug.current != $excludeSlug
          )
        ]
        | order(publishedAt desc, _createdAt desc)
        [0...$limit]
        ${ARTICLE_LIST_PROJECTION}
      `.trim(),
      params: {
        relatedPropertyType,
        excludeSlug: excludeSlug ?? null,
        limit,
      },
    };
  },
} as const;
