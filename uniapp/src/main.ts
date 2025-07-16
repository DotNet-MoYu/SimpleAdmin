import { VueQueryPlugin } from '@tanstack/vue-query'
import { createSSRApp } from 'vue'
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
  app.use(VueQueryPlugin)

  return {
    app,
  }
}
