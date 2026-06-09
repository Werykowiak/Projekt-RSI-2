import { defineStore } from 'pinia'

interface User {
  id: number
  email: string
  firstName: string
  lastName: string
  role?: string
}

export const useAuthStore = defineStore('auth', () => {
  const token = useCookie<string | null>('auth_token')
  const user = useCookie<User | null>('auth_user')

  const isAuthenticated = computed(() => !!token.value)
  // Backend teraz jawnie wysyła camelCase
  const isAdmin = computed(() => {
    if (!user.value) return false
    return user.value.role === 'Admin'
  })

  const userName = computed(() => {
    if (!user.value) return ''
    return user.value.firstName || ''
  })

  function setToken(newToken: string) {
    token.value = newToken
  }

  function setUser(newUser: User) {
    user.value = newUser
  }

  function logout() {
    token.value = null
    user.value = null
    navigateTo('/login')
  }

  return {
    token,
    user,
    isAuthenticated,
    isAdmin,
    userName,
    setToken,
    setUser,
    logout
  }
})
