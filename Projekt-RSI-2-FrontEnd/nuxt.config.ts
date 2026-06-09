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
      apiBase: 'https://localhost:7253' // Backend URL
    }
  },

  future: {
    compatibilityVersion: 4
  },

  compatibilityDate: '2024-11-01'
})
