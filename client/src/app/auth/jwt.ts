// decodes a JWT payload client-side; this reads the claims only, it does not verify the signature
export function decodeJwtPayload(token: string): Record<string, unknown> {
  const payload = token.split('.')[1];
  const json = atob(payload.replace(/-/g, '+').replace(/_/g, '/'));
  return JSON.parse(json);
}

export function getUserIdFromToken(token: string): number | null {
  const payload = decodeJwtPayload(token);
  const nameId = payload['nameid'];
  return typeof nameId === 'string' ? Number(nameId) : null;
}
