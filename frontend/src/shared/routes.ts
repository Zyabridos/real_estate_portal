import {id} from "zod/v4/locales";

const apiRoutes = {
  health: (): string => ["health"].join("/"),
  agencies: {
    list: (): string => ["agencies"].join("/"),
    getById: (id: string): string => ["agencies", id].join("/"),
  },
  properties: {
    list: (): string => ["properties"].join("/"),
    getById: (id: string): string => ["properties", id].join("/"),
  },
  brokers: {
    list: (): string => ["brokers"].join("/"),
    getById: (id: string): string => ["brokers", id].join("/"),
  },
  leads: {
    create: (): string => ["leads"].join("/"),
    getById: (id: string): string => ["/leads", id].join("/"),
  },
};

const pagesRoutes = {
  home: (): string => "/",
  agencies: {
    list: (): string => "/agencies",
    details: (id: string): string => ["/agencies", id].join("/"),
  },
  brokers: {
    list: (): string => "/brokers",
    details: (id: string): string => ["/brokers", id].join("/"),
  },
  blog: (): string => "/blog",
  properties: {
    list: (): string => "/properties",
    details: (id: string): string => ["/properties", id].join("/"),
  },
  leads: {
    create: (propertyId: string) => ["/properties", propertyId, "lead"].join("/"),
    list: (): string => "/leads",
  },
};

const routes = { app: pagesRoutes, api: apiRoutes };
export default routes;
