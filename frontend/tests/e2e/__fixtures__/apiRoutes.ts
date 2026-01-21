const apiBase = "/api";

export const apiRoutes = {
  leads: {
    pattern: () => `**${apiBase}/leads`, // kinda wildcard
    path: () => `${apiBase}/leads`,
  },
} as const;
