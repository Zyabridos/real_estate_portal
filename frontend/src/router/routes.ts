import type { RouteRecordRaw } from "vue-router";
import HomePage from "@/pages/HomePage.vue";
import NotFoundPage from "@/pages/system/NotFoundPage.vue";

// import BlogListPage from "@/pages/blog/BlogListPage.vue";
import BrokersListPage from "@/pages/brokers/BrokersListPage.vue";
import BrokerDetailsPage from "@/pages/brokers/BrokerDetailsPage.vue";

import PropertiesListPage from "@/pages/properties/PropertiesListPage.vue";
import PropertyDetailsPage from "@/pages/properties/PropertyDetailsPage.vue";

const routes: RouteRecordRaw[] = [
  { path: "/", name: "home", component: HomePage },

  // { path: "/blog", name: "blog", component: BlogListPage },

  { path: "/brokers", name: "brokers", component: BrokersListPage },
  { path: "/brokers/:id", name: "broker-details", component: BrokerDetailsPage },

  { path: "/properties", name: "properties", component: PropertiesListPage },
  { path: "/properties/:id", name: "property-details", component: PropertyDetailsPage },

  { path: "/:pathMatch(.*)*", name: "notFound", component: NotFoundPage },
];

export default routes;
