export interface AccessTokenProvider {
  getAccessToken(): Promise<string>;
}

export class ApiClient {
  private readonly apiBaseUrl: string;
  private readonly accessTokenProvider: AccessTokenProvider;

  public constructor(apiBaseUrl: string, accessTokenProvider: AccessTokenProvider) {
    this.apiBaseUrl = apiBaseUrl.replace(/\/$/, '');
    this.accessTokenProvider = accessTokenProvider;
  }

  public async fetchJson<T>(input: string, init?: RequestInit): Promise<T> {
    const response = await this.send(input, init);

    if (response.status === 204) {
      return undefined as T;
    }

    return (await response.json()) as T;
  }

  public async send(input: string, init?: RequestInit): Promise<Response> {
    const accessToken = await this.accessTokenProvider.getAccessToken();
    const headers = new Headers(init?.headers);

    headers.set('Authorization', `Bearer ${accessToken}`);

    if (!headers.has('Content-Type') && init?.body) {
      headers.set('Content-Type', 'application/json');
    }

    const response = await fetch(this.resolveUrl(input), {
      ...init,
      headers
    });

    if (response.status === 401 || response.status === 403) {
      throw new Error('The current user is not authorized to call the Secrets Dashboard API.');
    }

    if (!response.ok) {
      throw new Error(`The Secrets Dashboard API returned status ${response.status}.`);
    }

    return response;
  }

  private resolveUrl(path: string): string {
    if (/^https?:\/\//i.test(path)) {
      return path;
    }

    if (path.startsWith('/')) {
      return `${this.apiBaseUrl}${path}`;
    }

    return `${this.apiBaseUrl}/${path}`;
  }
}
