/**
 * @description 认证模块
 * @license Apache License Version 2.0
 * @Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
 * @remarks
 * SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
 * 1.请不要删除和修改根目录下的LICENSE文件。
 * 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
 * 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
 * 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
 * 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为不要删除和修改作者声明。
 * 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关
 * @see https://gitee.com/dotnetmoyu/SimpleAdmin
 */
import { defineStore } from "pinia";
import { getFlatMenuList, getShowMenuList, getAllBreadcrumbList } from "@/utils";
import { Login, UserCenter } from "@/api/interface";
import { loginApi, userCenterApi } from "@/api";
import { useUserStore } from "./user";
import { useTabsStore } from "./tabs";
import { useKeepAliveStore } from "./keepAlive";
import { useConfigStore } from "./config";
import { initDynamicRouter } from "@/routers/modules/dynamicRouter";
import { ElNotification } from "element-plus";
import { getTimeState } from "@/utils";
import router from "@/routers";

const name = "simple-auth"; // 定义模块名称

/* AuthState */
export interface AuthState {
  /** 登录的加载状态 */
  loginLoading: boolean;
  /** 是否开启选择模块 */
  showChooseModule: boolean;
  /** 模块列表 */
  moduleList: Login.ModuleInfo[];
  /** 菜单权限列表 */
  authButtonList: string[];
  /** 菜单权限列表 */
  authMenuList: Menu.MenuOptions[];
}

/** 认证模块 */
export const useAuthStore = defineStore({
  id: name,
  state: (): AuthState => ({
    loginLoading: false,
    showChooseModule: false,
    moduleList: [],
    authButtonList: [],
    authMenuList: []
  }),
  getters: {
    // 按钮权限列表
    authButtonListGet: state => state.authButtonList,
    // 菜单权限列表 ==> 这里的菜单没有经过任何处理
    authMenuListGet: state => state.authMenuList,
    // 菜单权限列表 ==> 左侧菜单栏渲染，需要剔除 isHide == true
    showMenuListGet: state => getShowMenuList(state.authMenuList),
    // 菜单权限列表 ==> 扁平化之后的一维数组菜单，主要用来添加动态路由
    flatMenuListGet: state => getFlatMenuList(state.authMenuList),
    // 递归处理后的所有面包屑导航列表
    breadcrumbListGet: state => getAllBreadcrumbList(state.authMenuList)
  },
  actions: {
    /** 设置模块列表 */
    SetModuleList(moduleList: Login.ModuleInfo[]) {
      this.moduleList = moduleList;
    },
    /** 选择应用模块 */
    async chooseModule(config: UserCenter.ResModuleDefault) {
      const userStore = useUserStore();
      userStore.setModule(config.id); //存储选择的模块
      await userCenterApi.setDefaultModule(config); //设置默认模块
      this.showChooseModule = false; //选择模块状态为关闭
    },
    /** 获取按钮列表 */
    async getAuthButtonList() {
      const userStore = useUserStore();
      const { userInfo } = userStore;
      this.authButtonList = userInfo?.buttonCodeList || [];
    },
    /** 获取菜单列表 */
    async getAuthMenuList(moduleId: number | string) {
      const { data } = await userCenterApi.getAuthMenuList({ id: moduleId });
      this.authMenuList = data;
    },
    /** 账号密码登录 */
    async loginPwd(model: Login.LoginForm) {
      this.loginLoading = true;
      this.setTenantId(model); // 存储租户id
      // 登录接口
      await loginApi
        .login(model)
        .then(res => {
          if (res.data) {
            this.loginSuccess(res.data); //登录成功
          }
        })
        .catch(err => {
          return Promise.reject(err);
        })
        .finally(() => {
          this.loginLoading = false;
        });
    },
    /** 账号密码登录 */
    async loginPhone(model: Login.PhoneLoginForm) {
      this.loginLoading = true;
      this.setTenantId(model); // 存储租户id
      // 登录接口
      await loginApi
        .loginByPhone(model)
        .then(res => {
          if (res.data) {
            this.loginSuccess(res.data); //登录成功
          }
        })
        .catch(err => {
          return Promise.reject(err);
        })
        .finally(() => {
          this.loginLoading = false;
        });
    },
    /** 登录成功后的操作 */
    async handleActionAfterLogin() {
      await initDynamicRouter()
        .then(path => {
          // 初始化动态路由
          const tabsStore = useTabsStore();
          const keepAliveStore = useKeepAliveStore();
          // 3.清空 tabs、keepAlive 数据
          tabsStore.setTabs([]);
          keepAliveStore.setKeepAliveName([]);
          // 4.跳转到首页
          router.push(path);
          ElNotification({
            title: getTimeState(),
            message: "欢迎回来 SimpleAdmin",
            type: "success",
            duration: 3000
          });
        })
        .catch(err => {
          console.log("[ err ] >", err);
          ElNotification({
            title: "系统错误",
            message: "系统错误,请联系系统管理员！",
            type: "warning",
            duration: 3000
          });
        });
    },
    /** 存储租户id供下次登录自动选择 */
    setTenantId(model: Login.LoginForm | Login.PhoneLoginForm) {
      // 如果是租户登录,存储id
      if (model.tenantId) {
        const configStore = useConfigStore();
        configStore.setTenantId(model.tenantId);
      }
    },
    /** 登录请求成功 */
    loginSuccess(data: Login.Login) {
      const { defaultModule, moduleList } = data;
      const userStore = useUserStore();
      this.SetModuleList(moduleList); // 设置模块列表
      if (moduleList.length === 1) {
        // 如果只有一个模块，直接登录
        userStore.setModule(moduleList[0].id); //存储选择的模块
        this.handleActionAfterLogin(); //登录成功后的操作
        return;
      } else if (defaultModule && moduleList.find(item => item.id === defaultModule)) {
        userStore.setModule(defaultModule); //存储选择的模块为默认模块
        this.handleActionAfterLogin(); //登录成功后的操作
      } else {
        this.showChooseModule = true; //开启选择模块
      }
    }
  }
});
