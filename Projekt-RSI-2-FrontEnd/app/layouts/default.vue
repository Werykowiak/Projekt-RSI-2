<template>
  <div class="min-h-screen bg-gray-950 text-gray-100 flex flex-col font-sans">
    <nav class="bg-gray-900/50 backdrop-blur-md sticky top-0 z-50 shadow-lg border-b border-gray-800">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between h-16 items-center">
          <div class="flex items-center gap-2">
            <UIcon
              name="i-heroicons-paper-airplane"
              class="w-8 h-8 text-primary-500"
            />
            <NuxtLink
              to="/"
              class="text-xl font-bold tracking-tight bg-gradient-to-r from-primary-400 to-primary-600 bg-clip-text text-transparent"
            >
              Dawidzior & Weryk Trains
            </NuxtLink>
          </div>

          <div class="flex items-center gap-2 md:gap-4">
            <UButton
              to="/"
              variant="ghost"
              color="gray"
              icon="i-heroicons-magnifying-glass"
              class="hover:bg-gray-800"
            >
              <span class="hidden sm:inline">Wyszukaj</span>
            </UButton>

            <template v-if="authStore.isAuthenticated">
              <UButton
                to="/reservations"
                variant="ghost"
                color="gray"
                icon="i-heroicons-ticket"
                class="hover:bg-gray-800"
              >
                <span class="hidden sm:inline">Moje Bilety</span>
              </UButton>

              <div class="h-6 w-[1px] bg-gray-800 mx-1 hidden md:block" />

              <div class="flex items-center gap-1 md:gap-3">
                <span class="text-sm font-medium text-gray-400 hidden lg:inline">
                  Witaj, <span class="text-primary-400">{{ authStore.userName }}</span>
                </span>
                <UButton
                  variant="ghost"
                  color="red"
                  icon="i-heroicons-arrow-left-on-rectangle"
                  class="hover:bg-red-500/10"
                  @click="authStore.logout"
                >
                  <span class="hidden md:inline">Wyloguj</span>
                </UButton>
              </div>
            </template>

            <template v-else>
              <UButton
                to="/login"
                variant="ghost"
                class="hover:bg-gray-800"
              >
                Zaloguj
              </UButton>
              <UButton
                to="/register"
                color="primary"
              >
                Zarejestruj
              </UButton>
            </template>
          </div>
        </div>
      </div>
    </nav>

    <main class="flex-grow max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8 w-full">
      <slot />
    </main>

    <footer class="bg-gray-900 border-t border-gray-800 py-8 mt-auto">
      <div class="max-w-7xl mx-auto px-4 text-center text-gray-500 text-sm">
        <p>&copy; 2026 Dawidzior & Weryk Trains. Wszystkie prawa zastrzeżone. 😎</p>
        <p class="mt-2 text-gray-600">
          Komfortowa podróż w zasięgu ręki.
        </p>
      </div>
    </footer>
  </div>
</template>

<script setup>
const authStore = useAuthStore()
</script>
