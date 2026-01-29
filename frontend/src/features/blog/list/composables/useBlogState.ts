import { ref } from 'vue';
import type { ArticleListItemDto, CategoryDto, ArticleDetailsDto } from "@/entities/blog/model/types";

const articles = ref<ArticleListItemDto[]>([])

const categories = ref<CategoryDto[]>([])

const selectedCategory = ref<string | null>(null)
