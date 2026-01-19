import type { RouteRecordRaw } from "vue-router";
import BrokersListPags from "@/pages/brokers/BrokersListPage.vue"
import HomePage from "@/pages/HomePage.vue";
import NotFoundPage from "@/pages/system/NotFoundPage.vue";

import BlogListPage from "@/pages/blog/BlogListPage.vue";
import BrokersListPage from "@/pages/brokers/BrokersListPage.vue";
import PropertiesListPage from "@/pages/properties/PropertiesListPage.vue";

const routes: RouteRecordRaw[] = [
  { path: "/", name: "home", component: HomePage },
  { path: "/:pathMatch(.*)*", name: "notFound", component: NotFoundPage },
  { path: "/blog", name: "blog", component: BlogListPage },
  { path: "/brokers", name: "brokers", component: BrokersListPage },
  { path: "/properties", name: "properties", component: PropertiesListPage },
];

export default routes;
