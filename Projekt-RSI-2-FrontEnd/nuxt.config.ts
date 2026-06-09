// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  modules: ['@nuxt/ui', '@pinia/nuxt'],
  devtools: { enabled: true },

  css: ['~/assets/css/main.css'],

  colorMode: {
    preference: 'dark',
    fallback: 'dark',
    classSuffix: ''
  },

  runtimeConfig: {
    public: {
      apiBase: '' // Default, overridden by NUXT_PUBLIC_API_BASE environment variable
    }
  },

  future: {
    compatibilityVersion: 4
  },

  compatibilityDate: '2024-11-01'
})
