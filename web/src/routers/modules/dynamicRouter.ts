/**
 * @description 动态路由
 * @license Apache License Version 2.0
 * @remarks
 * SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
 * 1.请不要删除和修改根目录下的LICENSE文件。
 * 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
 * 3.分发源码时候，请注明软件出处 https://gitee.com/zxzyjs/SimpleAdmin
 * 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
 * 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为不要删除和修改作者声明。
 * 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关
 * @see https://gitee.com/zxzyjs/SimpleAdmin
 */
import { LOGIN_URL, HOME_URL } from "@/config";
import { RouteRecordRaw } from "vue-router";
import { ElNotification } from "element-plus";
import { useUserStore, useAuthStore, useDictStore } from "@/stores/modules";
import router from "@/routers";
// 引入 views 文件夹下所有 vue 文件
const modules = import.meta.glob("@/views/**/*.vue");

/**
 * @description 初始化动态路由
 */
export const initDynamicRouter = async () => {
  const userStore = useUserStore();
  const authStore = useAuthStore();
  const dictStore = useDictStore();
  /** 路由初始化错误 */
  const routerError = (isNotice: boolean = true, title: string = "无权限访问", message: string = "当前账号无任何菜单权限，请联系系统管理员！") => {
    if (!isNotice) {
      ElNotification({
        title: title,
        message: message,
        type: "warning",
        duration: 3000
      });
    }
    userStore.clearToken();
    router.replace(LOGIN_URL);
    return Promise.reject("No permission");
  };
  try {
    const chooseModule = userStore.chooseModuleGet; // 获取当前选择模块
    let homePath: string = HOME_URL; // 首页路径
    const data = await userStore.getUserInfo(); // 获取用户信息
    if (chooseModule && data?.moduleList.find(item => item.id === chooseModule)) {
      authStore.SetModuleList(data.moduleList); // 设置模块列表
      // 1.获取菜单列表 && 按钮权限列表
      await authStore.getAuthMenuList(chooseModule);
      await authStore.getAuthButtonList();
      // 2.判断当前用户有没有菜单权限
      if (!authStore.authMenuListGet.length) {
        return routerError();
      }
      await dictStore.setDictTree(); //  设置字典树
      console.log("authMenuListGet", authStore.authMenuListGet);

      const home = authStore.authMenuListGet.filter(item => item.isHome === true); // 获取首页
      if (home.length > 0) {
        homePath = home[0].path; // 设置第一个首页项作为首页路径
      } else {
        console.log(111);

        //如果不存在首页设置第一个菜单为首页
        let firstMenu = authStore.authMenuListGet[0].children;
        if (firstMenu && firstMenu.length > 0) {
          homePath = firstMenu[0].path as string;
        }
      }
    } else {
      return routerError(false); // 如果当前选择模块不存在
    }

    // 3.添加动态路由
    authStore.flatMenuListGet.forEach(item => {
      item.children && delete item.children;
      if (item.component && typeof item.component == "string") {
        item.component = modules["/src/views/" + item.component + ".vue"];
      }
      if (item.meta.isFull) {
        router.addRoute(item as unknown as RouteRecordRaw);
      } else {
        router.addRoute("layout", item as unknown as RouteRecordRaw);
      }
    });
    return Promise.resolve(homePath);
  } catch (error) {
    // 当按钮 || 菜单请求出错时，重定向到登陆页
    userStore.clearToken();
    router.replace(LOGIN_URL);
    return Promise.reject(error);
  }
};
