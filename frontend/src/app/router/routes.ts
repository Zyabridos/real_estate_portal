import type { RouteRecordRaw } from "vue-router";

import routes from "@/shared/routes.ts"

import HomePage from "@/app/HomePage.vue";
import NotFoundPage from "@/features/system/not-found/page/NotFoundPage.vue";

import BlogListPage from "@/features/blog/list/page/BlogListPage.vue";
import BlogDetailsPage from "@/features/blog/details/page/BlogDetailsPage.vue";

import BrokersListPage from "@/features/brokers/list/page/BrokersListPage.vue";
import BrokerDetailsPage from "@/features/brokers/details/page/BrokerDetailsPage.vue";

import PropertyLeadCreatePage from "@/features/leads/create/page/PropertyLeadCreatePage.vue";
import LeadsListPage from "@/features/leads/list/page/LeadsListPage.vue";

import PropertiesListPage from "@/features/properties/list/page/PropertiesListPage.vue";
import PropertyDetailsPage from "@/features/properties/details/page/PropertyDetailsPage.vue";

const routeRecords: RouteRecordRaw[] = [
  { path: "/", name: "home", component: HomePage },

  { path: "/blog", name: "blog", component: BlogListPage },
  { path: "/blog/:slug", name: "blog-details", component: BlogDetailsPage },

  { path: "/brokers", name: "brokers", component: BrokersListPage },
  { path: "/brokers/:id", name: "broker-details", component: BrokerDetailsPage },

  { path: "/properties/:id/lead/", name: "leads-form", component: PropertyLeadCreatePage },
  { path: routes.app.leads.list(), name: "leads-list", component: LeadsListPage },

  { path: "/properties", name: "properties", component: PropertiesListPage },
  { path: "/properties/:id", name: "property-details", component: PropertyDetailsPage },

  { path: "/:pathMatch(.*)*", name: "notFound", component: NotFoundPage },
];

export default routeRecords;
