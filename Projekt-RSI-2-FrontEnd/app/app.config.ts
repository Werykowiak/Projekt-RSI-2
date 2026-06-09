export default defineAppConfig({
  ui: {
    primary: 'sky',
    gray: 'neutral',
    formField: {
      slots: {
        label: 'block font-medium text-gray-200' // Wymuszenie jasnego koloru etykiet
      }
    },
    card: {
      slots: {
        root: 'bg-gray-900 border-gray-800 divide-gray-800'
      }
    },
    modal: {
      slots: {
        overlay: 'bg-gray-950/75 backdrop-blur-sm'
      }
    }
  }
})
