/**
 * @description
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
import router from "@/routers";
import { defineStore } from "pinia";
import { getUrlWithParams } from "@/utils";
import { useKeepAliveStore } from "./keepAlive";
import { TabsState, TabsMenuProps } from "@/stores/interface";
import piniaPersistConfig from "@/stores/helper/persist";

export const useTabsStore = defineStore({
  id: "simple-tabs",
  state: (): TabsState => ({
    tabsMenuList: []
  }),
  actions: {
    // Add Tabs
    async addTabs(tabItem: TabsMenuProps) {
      const keepAliveStore = useKeepAliveStore();
      if (this.tabsMenuList.every(item => item.path !== tabItem.path)) {
        // 如果tabItem的路径不等于当前tabsMenuList的路径，则添加tabItem
        this.tabsMenuList.push(tabItem);
      }
      // 如果tabItem的name不包含在keepAliveStore的keepAliveName中，并且tabItem的isKeepAlive为true，则添加keepAliveStore的keepAliveName
      if (!keepAliveStore.keepAliveName.includes(tabItem.name) && tabItem.isKeepAlive) {
        keepAliveStore.addKeepAliveName(tabItem.name);
      }
    },
    // Remove Tabs
    async removeTabs(tabPath: string, isCurrent: boolean = true) {
      const keepAliveStore = useKeepAliveStore();
      if (isCurrent) {
        this.tabsMenuList.forEach((item, index) => {
          if (item.path !== tabPath) return;
          const nextTab = this.tabsMenuList[index + 1] || this.tabsMenuList[index - 1];
          if (!nextTab) return;
          router.push(nextTab.path);
        });
      }
      this.tabsMenuList = this.tabsMenuList.filter(item => item.path !== tabPath);
      // remove keepalive
      const tabItem = this.tabsMenuList.find(item => item.path === tabPath);
      tabItem?.isKeepAlive && keepAliveStore.removeKeepAliveName(tabItem.name);
      // set tabs
      this.tabsMenuList = this.tabsMenuList.filter(item => item.path !== tabPath);
    },

    // Close Tabs On Side
    async closeTabsOnSide(path: string, type: "left" | "right") {
      const keepAliveStore = useKeepAliveStore();
      const currentIndex = this.tabsMenuList.findIndex(item => item.path === path);
      if (currentIndex !== -1) {
        const range = type === "left" ? [0, currentIndex] : [currentIndex + 1, this.tabsMenuList.length];
        this.tabsMenuList = this.tabsMenuList.filter((item, index) => {
          return index < range[0] || index >= range[1] || !item.close;
        });
      }
      // set keepalive
      const KeepAliveList = this.tabsMenuList.filter(item => item.isKeepAlive);
      keepAliveStore.setKeepAliveName(KeepAliveList.map(item => item.name));
    },
    // Close MultipleTab
    async closeMultipleTab(tabsMenuValue?: string) {
      const keepAliveStore = useKeepAliveStore();
      this.tabsMenuList = this.tabsMenuList.filter(item => {
        return item.path === tabsMenuValue || !item.close;
      });
      // set keepalive
      const KeepAliveList = this.tabsMenuList.filter(item => item.isKeepAlive);
      keepAliveStore.setKeepAliveName(KeepAliveList.map(item => item.name));
    },
    // Set Tabs
    async setTabs(tabsMenuList: TabsMenuProps[]) {
      this.tabsMenuList = tabsMenuList;
    },
    // Set Tabs Title
    async setTabsTitle(title: string) {
      this.tabsMenuList.forEach(item => {
        if (item.path == getUrlWithParams()) item.title = title;
      });
    }
  },
  persist: piniaPersistConfig("simple-tabs")
});
