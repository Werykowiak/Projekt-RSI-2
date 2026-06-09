<template>
  <div class="max-w-md mx-auto mt-12">
    <UCard>
      <template #header>
        <h1 class="text-2xl font-bold text-center">
          Logowanie
        </h1>
      </template>

      <form
        class="space-y-4"
        @submit.prevent="handleLogin"
      >
        <UFormField
          label="Email"
          name="email"
        >
          <UInput
            v-model="form.email"
            type="email"
            placeholder="twoj@email.com"
            icon="i-heroicons-envelope"
          />
        </UFormField>

        <UFormField
          label="Hasło"
          name="password"
        >
          <UInput
            v-model="form.password"
            type="password"
            icon="i-heroicons-lock-closed"
          />
        </UFormField>

        <UButton
          type="submit"
          block
          :loading="loading"
          size="lg"
        >
          Zaloguj się
        </UButton>
      </form>

      <template #footer>
        <div class="text-center text-sm text-gray-500">
          Nie masz konta? <NuxtLink
            to="/register"
            class="text-primary font-medium"
          >Zarejestruj się</NuxtLink>
        </div>
      </template>
    </UCard>
  </div>
</template>

<script setup>
const api = useApi()
const authStore = useAuthStore()
const toast = useToast()

const form = reactive({
  email: '',
  password: ''
})

const loading = ref(false)

async function handleLogin() {
  console.log('Login attempt started for:', form.email)
  loading.value = true
  try {
    const response = await api.post('/api/Auth/login', form)
    console.log('Login response received:', response)

    authStore.setToken(response.token)
    authStore.setUser(response.user)

    console.log('Store updated, showing toast')
    toast.add({ title: 'Zalogowano pomyślnie!', color: 'green' })

    await navigateTo('/', { replace: true })
  } catch (err) {
    console.error('Login error caught:', err)
    const errorMsg = err.data || err.message || 'Sprawdź dane i spróbuj ponownie'
    toast.add({ title: 'Błąd logowania', description: errorMsg, color: 'red' })
  } finally {
    loading.value = false
  }
}
</script>
