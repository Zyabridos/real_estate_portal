export type CategoryDto = {
  id: string;
  title: string
  slug: string
}

export type ArticleListItemDto = {
  id: string
  title: string
  slug: string
  excerpt?: string
  publishedAt?: string
  author?: { name: string }
  categories?: CategoryDto[]
  relatedPropertyType?: string // TODO: move to enum (union?)
  mainImageUrl?: string // TODO: consider { url?: string; alt?: string } structure
}

export interface ArticleDetailsDto extends ArticleListItemDto {
  content: unknown;
}
