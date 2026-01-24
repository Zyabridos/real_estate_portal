import { ref } from 'vue';
import type { ArticleListItemDto, CategoryDto, ArticleDetailsDto } from "@/shared/types/blog.ts";

const articles = ref<ArticleListItemDto[]>([])

const categories = ref<CategoryDto[]>([])

const selectedCategory = ref<string | null>(null)
