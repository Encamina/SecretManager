import {
  InteractionRequiredAuthError,
  PublicClientApplication,
  type AccountInfo,
  type IPublicClientApplication
} from '@azure/msal-browser';
import {
  MsalProvider,
  useIsAuthenticated,
  useMsal
} from '@azure/msal-react';
import {
  createContext,
  type PropsWithChildren,
  useCallback,
  useContext,
  useEffect,
  useMemo
} from 'react';
import { apiBaseUrl, isAuthConfigured, loginRequest, msalConfig } from '../../services/auth/msalConfig';
import { ApiClient } from '../../services/http/apiClient';

const msalInstance = new PublicClientApplication(msalConfig);
void msalInstance.initialize();

interface AuthContextValue {
  apiClient: ApiClient;
  getAccessToken(): Promise<string>;
  isAuthenticated: boolean;
  isConfigured: boolean;
  login(): Promise<void>;
  logout(): Promise<void>;
}

const AuthContext = createContext<AuthContextValue | null>(null);

export function AuthProvider({ children }: PropsWithChildren) {
  return (
    <MsalProvider instance={msalInstance}>
      <AuthContextProvider>{children}</AuthContextProvider>
    </MsalProvider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);

  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider.');
  }

  return context;
}

function AuthContextProvider({ children }: PropsWithChildren) {
  const { accounts, inProgress, instance } = useMsal();
  const isAuthenticated = useIsAuthenticated();

  useEffect(() => {
    const candidateAccount = instance.getActiveAccount() ?? accounts[0];

    if (candidateAccount) {
      instance.setActiveAccount(candidateAccount);
    }
  }, [accounts, instance]);

  const getAccessToken = useCallback(async () => {
    if (!isAuthConfigured) {
      throw new Error('Microsoft Entra ID is not configured for this environment.');
    }

    const account = getActiveAccount(instance);

    try {
      const response = await instance.acquireTokenSilent({
        ...loginRequest,
        account
      });

      return response.accessToken;
    } catch (error) {
      if (error instanceof InteractionRequiredAuthError) {
        const response = await instance.acquireTokenPopup({
          ...loginRequest,
          account
        });

        return response.accessToken;
      }

      throw error;
    }
  }, [instance]);

  const apiClient = useMemo(
    () =>
      new ApiClient(apiBaseUrl, {
        getAccessToken
      }),
    [getAccessToken]
  );

  const value = useMemo<AuthContextValue>(
    () => ({
      apiClient,
      getAccessToken,
      isAuthenticated: isAuthenticated && inProgress === 'none',
      isConfigured: isAuthConfigured,
      login: async () => {
        if (!isAuthConfigured) {
          throw new Error('Microsoft Entra ID is not configured for this environment.');
        }

        await instance.loginPopup(loginRequest);
      },
      logout: async () => {
        await instance.logoutPopup({
          mainWindowRedirectUri: window.location.origin
        });
      }
    }),
    [apiClient, getAccessToken, inProgress, instance, isAuthenticated]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

function getActiveAccount(instance: IPublicClientApplication): AccountInfo {
  const account = instance.getActiveAccount() ?? instance.getAllAccounts()[0];

  if (!account) {
    throw new Error('No signed-in account is available.');
  }

  instance.setActiveAccount(account);
  return account;
}
