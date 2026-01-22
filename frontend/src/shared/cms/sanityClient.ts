import { createClient, type SanityClient } from "@sanity/client";
import { env } from "@/shared/config/env";

function assertSanityConfig(): { projectId: string; dataset: string } {
  const { projectId, dataset } = env.sanity;
  if (!projectId || !dataset) {
    throw new Error(
      "Sanity missing PROJECT_ID and/or DATASET. Cannot create client.",
    );
  }
  return { projectId, dataset };
}

export const sanityClient: SanityClient = (() => {
  const { projectId, dataset } = assertSanityConfig();

  return createClient({
    projectId,
    dataset,
    apiVersion: env.sanity.apiVersion,
    useCdn: env.sanity.useCdn,
  });
})();
