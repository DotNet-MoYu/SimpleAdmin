// 路由拦截
import type { Router } from 'uni-mini-router'
import XEUtils from 'xe-utils'
import { HOME_URL } from '@/const/systemConst'
import { useUserStore } from '@/store/modules'
import * as page from './page'

// 设置一些白名单
const whiteList = ['login', 'mine', 'loading']

export function userRouternext(router: Router) {
  router.beforeEach((to, from, next) => {
    console.log('开始路由拦截：', to, from)
    const userStore = useUserStore()
    // console.log('我是否进来路由前置收尾了');
    // next入参 false 以取消导航
    // 判断是否需要登录
    // 如果有token跳转首页
    if (userStore.accessToken) {
      console.log('我有token')
      // 判断一下是否白名单里面的东西 如果是的话就代表直接跳转无需判断是否有token
      // 如果在白名单里面
      if (

        page.NO_TOKEN.NO_TOKEN_WHITE_LIST.includes(to.path as string)
        || page.HAS_TOKEN.HAS_TOKEN_WHITE_LIST.includes(to.path as string)
      ) {
        console.log('白名单,直接next')
        next()
      }
      else {
        // 路径正则过滤：/pages/biz/user/index =》 /pages/biz/user/**
        console.log('用户菜单：', userStore.userMobileMenus)
        if (userStore.userMobileMenus) {
          const isVisit = XEUtils.findTree(
            userStore.userMobileMenus,
            (item) => {
              console.log('[ item.regType ] >', item.regType)
              // 确保每个条件都有返回值
              if (
                item.category === 'MENU'
                && item.menuType === 'MENU'
              ) {
                const itemPath = item.path
                // 不使用正则表达式（只有路径相同的时候才可以进行访问）
                if (item.regType === 'false') {
                  return to.path === itemPath
                }
                // 使用正则表达式规则（相同的文件夹路径下页面可进行访问）
                if (item.regType === 'true') {
                  const regExp = new RegExp(
                    `^${itemPath.substr(
                      0,
                      itemPath.lastIndexOf('/') + 1,
                    )}`,
                  )
                  return regExp.test(to.path as string)
                }
              }
              // 如果没有匹配的条件，返回false
              return false
            },
          )
          // 如果允许访问
          if (isVisit) {
            console.log('允许访问')
            next()
          }
          else {
            console.log('不是白名单,跳转主页')
            // 判断不是白名单 跳转主页
            next({
              name: 'loading',
            })
          }
        }
        else {
          next({
            name: 'loading',
          })
        }
      }
    }
    else {
      console.log('我没token')
      // 判断页面是否在白名单
      if (page.NO_TOKEN.NO_TOKEN_WHITE_LIST.includes(to.path as string)) {
        console.log('在白名单')
        next()
      }
      else {
        console.log('不是白名单 还没有token值 直接进去登录页面')
        // 判断不是白名单 还没有token值 直接进去登录页面
        next({
          name: 'loading',
        })
      }
    }
  })

  router.afterEach((to) => {
    const userStore = useUserStore()
    // console.log(from);
    console.log('我先执行还是他先执行')
    // console.log(authStore.isLogin, to.name, '我跳转了');
    if (!userStore.accessToken && to && to.name !== 'login') {
      // 如果没有登录且目标路由不是登录页面则跳转到登录页面
      if (whiteList.includes(to.name as string)) {
        // 在免登录白名单，直接进入
        // console.log('我在白名单里面');
      }
      else {
        router.push({
          name: 'loading',
          params: { ...to.query },
        })
      }
    }
    else if (userStore.accessToken && to && to.name === 'login') {
      // 如果已经登录且目标页面是登录页面则跳转至首页
      router.replaceAll(HOME_URL)
    }
  })
}
