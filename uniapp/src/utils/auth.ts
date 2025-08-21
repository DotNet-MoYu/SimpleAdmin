/**
 * @description 全局权限检查工具
 * @author SimpleAdmin
 */
import { inject } from 'vue'

import { useAuthStore } from '@/store/modules/auth'

/**
 * 检查用户是否有指定权限
 * @param permissions 权限码，可以是字符串或字符串数组
 * @param mode 检查模式，'some' 表示有任意一个权限即可，'every' 表示必须拥有所有权限
 * @returns 是否有权限
 */
export function hasAuth(permissions: string | string[], mode: 'some' | 'every' = 'some'): boolean {
  const authStore = useAuthStore()
  const currentPageRoles = authStore.authButtonList ?? []

  if (typeof permissions === 'string') {
    return currentPageRoles.includes(permissions)
  }

  if (Array.isArray(permissions) && permissions.length > 0) {
    const checkFn = mode === 'every' ? 'every' : 'some'
    return permissions[checkFn](permission => currentPageRoles.includes(permission))
  }

  return false
}

/**
 * 批量检查权限
 * @param permissionMap 权限映射对象
 * @returns 权限检查结果对象
 */
export function hasAuthBatch(permissionMap: Record<string, string | string[]>): Record<string, boolean> {
  const result: Record<string, boolean> = {}

  Object.keys(permissionMap).forEach((key) => {
    result[key] = hasAuth(permissionMap[key])
  })

  return result
}

/**
 * 权限检查 Hook (组合式函数)
 * 可以在 setup 中使用
 */
export function useAuth() {
  // 尝试从 inject 获取，如果没有就直接使用函数
  const injectedHasAuth = inject('hasAuth', hasAuth)
  const injectedHasAuthBatch = inject('hasAuthBatch', hasAuthBatch)

  return {
    hasAuth: injectedHasAuth,
    hasAuthBatch: injectedHasAuthBatch,
    // 便捷方法
    checkAuth: injectedHasAuth,
    checkAuthBatch: injectedHasAuthBatch,
  }
}

/**
 * 权限检查装饰器（用于方法）
 * @param permissions 权限码
 * @param mode 检查模式
 * @returns 装饰器函数
 */
export function authRequired(permissions: string | string[], mode: 'some' | 'every' = 'some') {
  return function (target: any, propertyKey: string, descriptor: PropertyDescriptor) {
    const originalMethod = descriptor.value

    descriptor.value = function (...args: any[]) {
      if (hasAuth(permissions, mode)) {
        return originalMethod.apply(this, args)
      }
      else {
        console.warn(`权限不足，无法执行 ${propertyKey} 方法，需要权限: ${JSON.stringify(permissions)}`)
        uni.showToast({
          title: '权限不足',
          icon: 'none',
        })
        return false
      }
    }

    return descriptor
  }
}
