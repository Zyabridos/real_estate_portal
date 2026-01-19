import type { RouteRecordRaw } from "vue-router";
import HomePage from "@/pages/HomePage.vue";

const routes: RouteRecordRaw[] = [
  { path: "/", name: "home", component: HomePage },

  // evnt uncomment, but here is a template "while I am on it"
  // { path: "/properties", name: "properties", component: () => import("@/pages/PropertiesPage.vue") },
  // { path: "/brokers", name: "brokers", component: () => import("@/pages/BrokersPage.vue") },
  // { path: "/blog", name: "blog", component: () => import("@/pages/BlogPage.vue") },
  // { path: "/:pathMatch(.*)*", name: "notFound", component: () => import("@/pages/NotFoundPage.vue") },
];

export default routes;
