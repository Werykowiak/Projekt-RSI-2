<template>
  <div class="max-w-md mx-auto mt-12">
    <UCard>
      <template #header>
        <h1 class="text-2xl font-bold text-center">
          Rejestracja
        </h1>
      </template>

      <form
        class="space-y-4"
        @submit.prevent="handleRegister"
      >
        <div class="grid grid-cols-2 gap-4">
          <UFormField
            label="Imię"
            name="firstName"
          >
            <UInput
              v-model="form.firstName"
              placeholder="Jan"
            />
          </UFormField>
          <UFormField
            label="Nazwisko"
            name="lastName"
          >
            <UInput
              v-model="form.lastName"
              placeholder="Kowalski"
            />
          </UFormField>
        </div>

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
          Zarejestruj się
        </UButton>
      </form>

      <template #footer>
        <div class="text-center text-sm text-gray-500">
          Masz już konto? <NuxtLink
            to="/login"
            class="text-primary font-medium"
          >Zaloguj się</NuxtLink>
        </div>
      </template>
    </UCard>
  </div>
</template>

<script setup>
const api = useApi()
const toast = useToast()

const form = reactive({
  email: '',
  password: '',
  firstName: '',
  lastName: ''
})

const loading = ref(false)

async function handleRegister() {
  loading.value = true
  try {
    await api.post('/api/Auth/register', form)
    toast.add({ title: 'Konto utworzone!', description: 'Możesz się teraz zalogować', color: 'green' })
    navigateTo('/login')
  } catch (err) {
    toast.add({ title: 'Błąd rejestracji', description: err.data || 'Coś poszło nie tak', color: 'red' })
  } finally {
    loading.value = false
  }
}
</script>
