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


export type ArticleDetailsDto = ArticleListItemDto & {
  content?: PortableTextBlock[];
};

export type PortableTextSpan = {
  _key?: string;
  _type: "span";
  text?: string;
};

export type PortableTextBlock = {
  _key?: string;
  _type: "block";
  style?: string; // "bold", "h1", etc
  children?: PortableTextSpan[];
};
