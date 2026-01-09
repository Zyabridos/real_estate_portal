import { defineType, defineField } from 'sanity'

export const article = defineType({
    name: 'article',
    title: 'Article',
    type: 'document',
    fields: [
        defineField({ name: 'title', title: 'Title', type: 'string' }),
    ],
})
