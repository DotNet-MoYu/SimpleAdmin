import { createRouter } from 'uni-mini-router'
import { userRouternext } from './Routeinterception'

const router = createRouter({
  routes: [...ROUTES], // 路由表信息
})

userRouternext(router) // 使用路由拦截

export default router
