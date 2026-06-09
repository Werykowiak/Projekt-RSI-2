<template>
  <div class="space-y-8">
    <!-- Hero / Search Section -->
    <div class="bg-gray-900 rounded-3xl p-8 md:p-12 shadow-2xl border border-gray-800 relative overflow-hidden">
      <div class="absolute top-0 right-0 -mt-20 -mr-20 w-64 h-64 bg-primary-500/10 rounded-full blur-3xl" />
      <div class="relative max-w-3xl">
        <h1 class="text-4xl md:text-5xl font-extrabold text-white mb-4 tracking-tight">
          Gdzie dziś pojedziemy? 🚂
        </h1>
        <p class="text-lg text-gray-400 mb-8">
          Wyszukaj najlepsze połączenia kolejowe i zarezerwuj bilet w kilka sekund.
        </p>

        <form
          class="grid grid-cols-1 md:grid-cols-4 gap-4 bg-gray-800/50 p-4 rounded-2xl shadow-lg border border-gray-700 backdrop-blur-sm"
          @submit.prevent="search"
        >
          <UFormField
            label="Skąd"
            name="from"
            class="md:col-span-1"
          >
            <UInput
              v-model="filters.from"
              placeholder="np. Warszawa"
              icon="i-heroicons-map-pin"
              color="gray"
              variant="outline"
            />
          </UFormField>
          <UFormField
            label="Dokąd"
            name="to"
            class="md:col-span-1"
          >
            <UInput
              v-model="filters.to"
              placeholder="np. Kraków"
              icon="i-heroicons-flag"
              color="gray"
              variant="outline"
            />
          </UFormField>
          <UFormField
            label="Kiedy"
            name="date"
            class="md:col-span-1"
          >
            <UInput
              v-model="filters.date"
              type="date"
              icon="i-heroicons-calendar"
              color="gray"
              variant="outline"
            />
          </UFormField>
          <div class="flex items-end">
            <UButton
              type="submit"
              icon="i-heroicons-magnifying-glass"
              block
              size="lg"
              :loading="pending"
            >
              Szukaj
            </UButton>
          </div>
        </form>
      </div>
    </div>

    <!-- Results Section -->
    <div
      v-if="routes && routes.length > 0"
      class="space-y-4"
    >
      <h2 class="text-2xl font-bold text-white px-2 flex items-center gap-2">
        <UIcon
          name="i-heroicons-list-bullet"
          class="text-primary-500"
        />
        Znalezione połączenia ({{ routes.length }})
      </h2>
      <div class="grid grid-cols-1 gap-4">
        <UCard
          v-for="route in routes"
          :key="route.id"
          class="hover:border-primary-500/50 transition-colors bg-gray-900 border-gray-800"
        >
          <div class="flex flex-col md:flex-row justify-between items-center gap-6">
            <div class="flex-grow grid grid-cols-1 md:grid-cols-3 gap-8 items-center w-full">
              <!-- Departure -->
              <div class="text-center md:text-left">
                <div class="text-3xl font-black text-white">
                  {{ formatTime(route.departureTime) }}
                </div>
                <div class="text-lg font-bold text-gray-200">
                  {{ route.departureCity }}
                </div>
                <div class="text-sm text-gray-500">
                  {{ formatDate(route.departureTime) }}
                </div>
              </div>

              <!-- Duration / Arrow -->
              <div class="flex flex-col items-center justify-center">
                <div class="text-[10px] font-black text-primary-500 uppercase tracking-[0.2em] mb-2 bg-primary-500/10 px-2 py-0.5 rounded">
                  Bezpośredni
                </div>
                <div class="flex items-center w-full max-w-[150px]">
                  <div class="h-[1px] flex-grow bg-gray-700" />
                  <UIcon
                    name="i-heroicons-chevron-right"
                    class="text-primary-500 mx-2"
                  />
                  <div class="h-[1px] flex-grow bg-gray-700" />
                </div>
              </div>

              <!-- Arrival -->
              <div class="text-center md:text-right">
                <div class="text-3xl font-black text-white">
                  {{ formatTime(route.arrivalTime) }}
                </div>
                <div class="text-lg font-bold text-gray-200">
                  {{ route.arrivalCity }}
                </div>
                <div class="text-sm text-gray-500">
                  {{ formatDate(route.arrivalTime) }}
                </div>
              </div>
            </div>

            <!-- Price & Booking -->
            <div class="flex flex-col items-center md:items-end gap-3 border-t md:border-t-0 md:border-l border-gray-800 pt-6 md:pt-0 md:pl-8 min-w-[180px]">
              <div class="text-3xl font-black text-primary-500">
                {{ route.price }} PLN
              </div>
              <div
                class="flex items-center gap-1.5 px-2 py-1 rounded-md bg-gray-800 text-xs font-bold"
                :class="route.availableSeats > 5 ? 'text-green-400' : 'text-orange-400'"
              >
                <div
                  class="w-1.5 h-1.5 rounded-full"
                  :class="route.availableSeats > 5 ? 'bg-green-400 animate-pulse' : 'bg-orange-400'"
                />
                Wolne miejsca: {{ route.availableSeats }}
              </div>
              <UButton
                v-if="authStore.isAuthenticated"
                color="primary"
                size="md"
                :disabled="route.availableSeats <= 0"
                class="w-full"
                @click="openBookingModal(route)"
              >
                {{ route.availableSeats > 0 ? 'Rezerwuj' : 'Brak miejsc' }}
              </UButton>
              <UButton
                v-else
                to="/login"
                variant="soft"
                color="gray"
                size="sm"
                class="w-full"
              >
                Zaloguj się
              </UButton>
            </div>
          </div>
        </UCard>
      </div>
    </div>

    <div
      v-else-if="!pending"
      class="text-center py-20 bg-gray-100 rounded-3xl border-2 border-dashed"
    >
      <UIcon
        name="i-heroicons-face-frown"
        class="w-16 h-16 text-gray-300 mb-4"
      />
      <h3 class="text-xl font-medium text-gray-500">
        Nie znaleźliśmy żadnych połączeń.
      </h3>
      <p class="text-gray-400">
        Spróbuj zmienić filtry wyszukiwania.
      </p>
    </div>

    <!-- Admin Section -->
    <template v-if="authStore.isAdmin">
      <div class="mt-12 p-8 bg-gray-900 rounded-3xl border border-primary-500/30">
        <div class="flex justify-between items-center mb-6">
          <h2 class="text-2xl font-bold text-white flex items-center gap-2">
            <UIcon
              name="i-heroicons-shield-check"
              class="text-primary-500"
            />
            Panel Administratora
          </h2>
          <UButton
            color="primary"
            icon="i-heroicons-plus"
            @click="openAdminModal()"
          >
            Dodaj połączenie
          </UButton>
        </div>
        <p class="text-gray-400 mb-4 text-sm">
          Jako administrator możesz dodawać nowe trasy do systemu.
        </p>
      </div>

      <!-- Admin Route Modal -->
      <UModal v-model:open="adminModal.isOpen">
        <template #content>
          <UCard>
            <template #header>
              <h3 class="text-xl font-bold">
                Dodaj nowe połączenie
              </h3>
            </template>
            <form
              class="space-y-4"
              @submit.prevent="saveRoute"
            >
              <div class="grid grid-cols-2 gap-4">
                <UFormField
                  label="Z"
                  name="departureCity"
                >
                  <UInput
                    v-model="adminModal.form.departureCity"
                    color="gray"
                    variant="outline"
                    required
                  />
                </UFormField>
                <UFormField
                  label="Do"
                  name="arrivalCity"
                >
                  <UInput
                    v-model="adminModal.form.arrivalCity"
                    color="gray"
                    variant="outline"
                    required
                  />
                </UFormField>
              </div>
              <div class="grid grid-cols-2 gap-4">
                <UFormField
                  label="Odjazd"
                  name="departureTime"
                >
                  <UInput
                    v-model="adminModal.form.departureTime"
                    type="datetime-local"
                    color="gray"
                    variant="outline"
                    required
                  />
                </UFormField>
                <UFormField
                  label="Przyjazd"
                  name="arrivalTime"
                >
                  <UInput
                    v-model="adminModal.form.arrivalTime"
                    type="datetime-local"
                    color="gray"
                    variant="outline"
                    required
                  />
                </UFormField>
              </div>
              <div class="grid grid-cols-2 gap-4">
                <UFormField
                  label="Cena (PLN)"
                  name="price"
                >
                  <UInput
                    v-model.number="adminModal.form.price"
                    type="number"
                    step="0.01"
                    color="gray"
                    variant="outline"
                    required
                  />
                </UFormField>
                <UFormField
                  label="Miejsca"
                  name="availableSeats"
                >
                  <UInput
                    v-model.number="adminModal.form.availableSeats"
                    type="number"
                    color="gray"
                    variant="outline"
                    required
                  />
                </UFormField>
              </div>
              <UButton
                type="submit"
                block
                :loading="adminModal.loading"
              >
                Zapisz trasę
              </UButton>
            </form>
          </UCard>
        </template>
      </UModal>
    </template>

    <!-- Booking Modal -->
    <UModal v-model:open="bookingModal.isOpen">
      <template #content>
        <UCard :ui="{ divide: 'divide-y divide-gray-100' }">
          <template #header>
            <div class="flex items-center justify-between">
              <h3 class="text-xl font-bold">
                Podsumowanie Rezerwacji
              </h3>
              <UButton
                color="gray"
                variant="ghost"
                icon="i-heroicons-x-mark-20-solid"
                class="-my-1"
                @click="bookingModal.isOpen = false"
              />
            </div>
          </template>

          <div
            v-if="bookingModal.route"
            class="space-y-4 py-4"
          >
            <div class="flex justify-between text-sm">
              <span class="text-gray-500">Trasa:</span>
              <span class="font-bold">{{ bookingModal.route.departureCity }} -> {{ bookingModal.route.arrivalCity }}</span>
            </div>
            <div class="flex justify-between text-sm">
              <span class="text-gray-500">Data i godzina:</span>
              <span>{{ formatDate(bookingModal.route.departureTime) }} {{ formatTime(bookingModal.route.departureTime) }}</span>
            </div>

            <UFormField label="Liczba pasażerów">
              <UInput
                v-model.number="bookingModal.seats"
                type="number"
                min="1"
                :max="bookingModal.route.availableSeats"
              />
            </UFormField>

            <div class="pt-4 border-t flex justify-between items-center">
              <span class="text-lg font-bold">Do zapłaty:</span>
              <span class="text-2xl font-black text-primary">{{ (bookingModal.route.price * bookingModal.seats).toFixed(2) }} PLN</span>
            </div>
          </div>

          <template #footer>
            <div class="flex gap-4">
              <UButton
                color="gray"
                variant="ghost"
                block
                @click="bookingModal.isOpen = false"
              >
                Anuluj
              </UButton>
              <UButton
                color="primary"
                block
                :loading="bookingModal.loading"
                @click="confirmBooking"
              >
                Potwierdzam zakup
              </UButton>
            </div>
          </template>
        </UCard>
      </template>
    </UModal>
  </div>
</template>

<script setup>
const api = useApi()
const authStore = useAuthStore()
const signalr = useSignalR()
const toast = useToast()

const filters = reactive({
  from: '',
  to: '',
  date: ''
})

const routes = ref([])
const pending = ref(false)

async function search() {
  pending.value = true
  try {
    const params = new URLSearchParams()
    if (filters.from) params.append('departureCity', filters.from)
    if (filters.to) params.append('arrivalCity', filters.to)
    if (filters.date) params.append('date', filters.date)

    routes.value = await api.get(`/api/TrainRoutes/search?${params.toString()}`)
  } catch {
    toast.add({ title: 'Błąd wyszukiwania', color: 'red' })
  } finally {
    pending.value = false
  }
}

// Initial search
onMounted(() => {
  search()

  // SignalR setup
  signalr.init()
  signalr.onUpdateSeats((routeId, availableSeats) => {
    const route = routes.value.find(r => r.id === routeId)
    if (route) {
      route.availableSeats = availableSeats
    }
  })
})

const bookingModal = reactive({
  isOpen: false,
  route: null,
  seats: 1,
  loading: false
})

function openBookingModal(route) {
  bookingModal.route = route
  bookingModal.seats = 1
  bookingModal.isOpen = true
}

async function confirmBooking() {
  bookingModal.loading = true
  try {
    await api.post('/api/Reservation/book', {
      trainRouteId: bookingModal.route.id,
      numberOfSeats: bookingModal.seats
    })
    toast.add({ title: 'Bilet kupiony!', description: 'Znajdziesz go w zakładce Moje Bilety', color: 'green' })
    bookingModal.isOpen = false
    // search() // Można odświeżyć, ale SignalR sam zaktualizuje liczbę miejsc!
  } catch (err) {
    toast.add({ title: 'Błąd rezerwacji', description: err.data || 'Nie udało się kupić biletu', color: 'red' })
  } finally {
    bookingModal.loading = false
  }
}

const adminModal = reactive({
  isOpen: false,
  loading: false,
  form: {
    departureCity: '',
    arrivalCity: '',
    departureTime: '',
    arrivalTime: '',
    price: 0,
    availableSeats: 0
  }
})

function openAdminModal() {
  adminModal.form = {
    departureCity: '',
    arrivalCity: '',
    departureTime: '',
    arrivalTime: '',
    price: 0,
    availableSeats: 0
  }
  adminModal.isOpen = true
}

async function saveRoute() {
  adminModal.loading = true
  try {
    await api.post('/api/TrainRoutes', adminModal.form)
    toast.add({ title: 'Dodano nową trasę!', color: 'green' })
    adminModal.isOpen = false
    search()
  } catch {
    toast.add({ title: 'Błąd dodawania trasy', color: 'red' })
  } finally {
    adminModal.loading = false
  }
}

// Helpers
const formatDate = dateStr => new Date(dateStr).toLocaleDateString('pl-PL')
const formatTime = dateStr => new Date(dateStr).toLocaleTimeString('pl-PL', { hour: '2-digit', minute: '2-digit' })
</script>
