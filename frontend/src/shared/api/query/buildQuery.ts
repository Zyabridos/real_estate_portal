const buildQuery = (params: Record<string, unknown>): Record<string, string> => {
  const q: Record<string, string> = {};

  for (const [k, v] of Object.entries(params)) {
    if (v === undefined || v === null) continue;
    if (typeof v === "string" && v.trim() === "") continue;

    q[k] = String(v);
  }

  return q;
}

export default buildQuery;
