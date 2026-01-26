import type { RouteRecordRaw } from "vue-router";

import routes from "@/shared/routes.ts"

import HomePage from "@/pages/HomePage.vue";
import NotFoundPage from "@/pages/system/NotFoundPage.vue";

import BlogListPage from "@/pages/blog/BlogListPage.vue";
import BlogDetailsPage from "@/pages/blog/BlogDetailsPage.vue";

import BrokersListPage from "@/pages/brokers/BrokersListPage.vue";
import BrokerDetailsPage from "@/pages/brokers/BrokerDetailsPage.vue";

import PropertyLeadCreatePage from "@/pages/leads/PropertyLeadCreatePage.vue";
import LeadsListPage from "@/pages/leads/LeadsListPage.vue";

import PropertiesListPage from "@/pages/properties/PropertiesListPage.vue";
import PropertyDetailsPage from "@/pages/properties/PropertyDetailsPage.vue";

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
