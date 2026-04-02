import type { Configuration, PopupRequest } from '@azure/msal-browser';
import { LogLevel } from '@azure/msal-browser';

const tenantId = import.meta.env.VITE_ENTRA_TENANT_ID?.trim();
const clientId = import.meta.env.VITE_ENTRA_CLIENT_ID?.trim();
const apiScope = import.meta.env.VITE_ENTRA_SCOPE?.trim();

export const isAuthConfigured = Boolean(tenantId && clientId);

export const msalConfig: Configuration = {
  auth: {
    authority: tenantId
      ? `https://login.microsoftonline.com/${tenantId}`
      : 'https://login.microsoftonline.com/common',
    clientId: clientId ?? '00000000-0000-0000-0000-000000000000',
    redirectUri: window.location.origin
  },
  cache: {
    cacheLocation: 'sessionStorage'
  },
  system: {
    loggerOptions: {
      loggerCallback: () => undefined,
      logLevel: LogLevel.Warning
    }
  }
};

export const loginRequest: PopupRequest = {
  scopes: apiScope ? [apiScope] : []
};

export const apiBaseUrl = import.meta.env.VITE_API_BASE_URL?.trim() ?? '';
