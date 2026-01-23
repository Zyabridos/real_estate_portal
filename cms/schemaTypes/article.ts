import { defineField, defineType } from "sanity";

export const article = defineType({
    name: "article",
    title: "Article",
    type: "document",
    fields: [
        defineField({ name: "title", title: "Title", type: "string", validation: (r) => r.required() }),
        defineField({
            name: "slug",
            title: "Slug",
            type: "slug",
            options: { source: "title", maxLength: 96 },
            validation: (r) => r.required(),
        }),
        defineField({ name: "excerpt", title: "Excerpt", type: "text" }),
        defineField({ name: "publishedAt", title: "Published at", type: "datetime" }),

        defineField({
            name: "author",
            title: "Author",
            type: "reference",
            to: [{ type: "author" }],
        }),
        defineField({
            name: "categories",
            title: "Categories",
            type: "array",
            of: [{ type: "reference", to: [{ type: "category" }] }],
        }),

        defineField({
            name: "relatedPropertyType",
            title: "Related property type",
            type: "string",
            description: "Used later for related articles on property details page",
        }),
    ],
});
