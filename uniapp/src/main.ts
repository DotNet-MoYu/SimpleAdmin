import { createSSRApp } from 'vue'
import { hasAuth, hasAuthBatch } from '@/utils/auth'
import App from './App.vue'
import { requestInterceptor } from './http/interceptor'
// import { routeInterceptor } from './router/interceptor'
import router from './router' // 路由
import store from './store'
import '@/style/index.scss'
import 'virtual:uno.css'

export function createApp() {
  const app = createSSRApp(App)
  app.use(router)
  app.use(store)
  // app.use(routeInterceptor)
  app.use(requestInterceptor)

  // 添加全局权限检查方法
  app.config.globalProperties.$hasAuth = hasAuth
  app.config.globalProperties.$hasAuthBatch = hasAuthBatch

  // 提供全局权限检查方法
  app.provide('hasAuth', hasAuth)
  app.provide('hasAuthBatch', hasAuthBatch)

  return {
    app,
  }
}
