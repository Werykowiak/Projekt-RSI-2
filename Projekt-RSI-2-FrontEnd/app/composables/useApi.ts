import type { NitroFetchOptions } from 'nitropack'

export const useApi = () => {
  const config = useRuntimeConfig()
  const authStore = useAuthStore()

  /* eslint-disable @typescript-eslint/no-explicit-any */
  const fetchOptions = (options: NitroFetchOptions<any> = {}) => {
    const headers: Record<string, string> = {
      ...(options.headers as Record<string, string>)
    }

    if (authStore.token) {
      headers['Authorization'] = `Bearer ${authStore.token}`
    }

    return {
      baseURL: config.public.apiBase,
      ...options,
      headers
    }
  }

  return {
    get: <T>(url: string, options: NitroFetchOptions<any> = {}) => $fetch<T>(url, fetchOptions({ ...options, method: 'GET' })),
    post: <T>(url: string, body: any, options: NitroFetchOptions<any> = {}) => $fetch<T>(url, fetchOptions({ ...options, method: 'POST', body })),
    put: <T>(url: string, body: any, options: NitroFetchOptions<any> = {}) => $fetch<T>(url, fetchOptions({ ...options, method: 'PUT', body })),
    delete: <T>(url: string, options: NitroFetchOptions<any> = {}) => $fetch<T>(url, fetchOptions({ ...options, method: 'DELETE' })),
    // Wbudowane useFetch dla reaktywności w komponentach
    useFetch: <T>(url: string, options: any = {}) => useFetch<T>(url, fetchOptions(options) as any)
  }
  /* eslint-enable @typescript-eslint/no-explicit-any */
}
