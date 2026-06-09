<template>
  <div class="space-y-6">
    <div class="flex justify-between items-center">
      <h1 class="text-3xl font-bold text-white">
        Moje Bilety
      </h1>
      <UButton
        icon="i-heroicons-arrow-path"
        variant="ghost"
        color="gray"
        :loading="pending"
        @click="refresh"
      >
        Odśwież
      </UButton>
    </div>

    <div
      v-if="reservations && reservations.length > 0"
      class="grid grid-cols-1 gap-4"
    >
      <UCard
        v-for="res in reservations"
        :key="res.id"
        class="bg-gray-900 border-gray-800"
      >
        <div class="flex flex-col md:flex-row justify-between items-center gap-4">
          <div class="flex items-center gap-4">
            <div class="bg-primary-500/10 p-3 rounded-full text-primary-500">
              <UIcon
                name="i-heroicons-ticket"
                class="w-6 h-6"
              />
            </div>
            <div>
              <div class="text-xs font-bold text-gray-500 uppercase tracking-wider">
                Numer rezerwacji: #{{ res.id }}
              </div>
              <div class="font-bold text-xl text-white">
                {{ res.trainRoute?.departureCity }} → {{ res.trainRoute?.arrivalCity }}
              </div>
              <div class="text-sm text-gray-400">
                Odjazd: <span class="text-gray-200">{{ formatDate(res.trainRoute?.departureTime) }} {{ formatTime(res.trainRoute?.departureTime) }}</span>
              </div>
            </div>
          </div>

          <div class="flex items-center gap-8">
            <div class="text-center">
              <div class="text-xs text-gray-500 uppercase font-bold mb-1">
                Miejsca
              </div>
              <div class="font-bold text-white text-lg">
                {{ res.numberOfSeats }}
              </div>
            </div>
            <div class="text-center">
              <div class="text-xs text-gray-500 uppercase font-bold mb-1">
                Suma
              </div>
              <div class="font-black text-primary-500 text-lg">
                {{ (res.numberOfSeats * (res.trainRoute?.price || 0)).toFixed(2) }} PLN
              </div>
            </div>
            <UButton
              icon="i-heroicons-document-arrow-down"
              color="primary"
              variant="soft"
              :loading="downloadingId === res.id"
              @click="downloadPdf(res.id)"
            >
              Pobierz PDF
            </UButton>
          </div>
        </div>
      </UCard>
    </div>

    <div
      v-else-if="!pending"
      class="text-center py-20 bg-white rounded-2xl border border-gray-200 shadow-sm"
    >
      <UIcon
        name="i-heroicons-ticket"
        class="w-16 h-16 text-gray-200 mb-4"
      />
      <h3 class="text-xl font-medium text-gray-500">
        Nie masz jeszcze żadnych rezerwacji.
      </h3>
      <UButton
        to="/"
        color="primary"
        variant="link"
      >
        Wyszukaj połączenie i kup bilet
      </UButton>
    </div>

    <div
      v-else
      class="space-y-4"
    >
      <USkeleton
        v-for="i in 3"
        :key="i"
        class="h-24 w-full"
      />
    </div>
  </div>
</template>

<script setup>
const api = useApi()
const toast = useToast()

const { data: reservations, pending, refresh } = await api.useFetch('/api/Reservation/my')

const downloadingId = ref(null)

async function downloadPdf(id) {
  downloadingId.value = id
  try {
    const config = useRuntimeConfig()
    const authStore = useAuthStore()

    // Używamy natywnego fetch, aby łatwiej obsłużyć bloba
    const response = await fetch(`${config.public.apiBase}/api/Reservation/${id}/pdf`, {
      headers: {
        Authorization: `Bearer ${authStore.token}`
      }
    })

    if (!response.ok) throw new Error('Download failed')

    const blob = await response.blob()
    const url = window.URL.createObjectURL(blob)
    const a = document.createElement('a')
    a.href = url
    a.download = `Bilet_${id}.pdf`
    document.body.appendChild(a)
    a.click()
    window.URL.revokeObjectURL(url)
    a.remove()

    toast.add({ title: 'Bilet pobrany!', color: 'green' })
  } catch {
    toast.add({ title: 'Błąd pobierania', color: 'red' })
  } finally {
    downloadingId.value = null
  }
}

// Helpers
const formatDate = dateStr => dateStr ? new Date(dateStr).toLocaleDateString('pl-PL') : ''
const formatTime = dateStr => dateStr ? new Date(dateStr).toLocaleTimeString('pl-PL', { hour: '2-digit', minute: '2-digit' }) : ''
</script>
