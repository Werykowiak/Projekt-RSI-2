import * as signalR from '@microsoft/signalr'

export const useSignalR = () => {
  const config = useRuntimeConfig()
  const connection = ref<signalR.HubConnection | null>(null)

  const init = () => {
    connection.value = new signalR.HubConnectionBuilder()
      .withUrl(`${config.public.apiBase}/bookingHub`, {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets
      })
      .withAutomaticReconnect()
      .build()

    connection.value.start()
      .then(() => console.log('SignalR Connected'))
      .catch(err => console.error('SignalR Connection Error: ', err))

    return connection.value
  }

  const onUpdateSeats = (callback: (routeId: number, availableSeats: number) => void) => {
    if (connection.value) {
      connection.value.on('UpdateSeats', callback)
    }
  }

  return {
    init,
    onUpdateSeats
  }
}
