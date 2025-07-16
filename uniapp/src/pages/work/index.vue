<!--
 * @Description: 工作台
 * @Author: huguodong
 * @Date: 2025-06-30 13:18:50
! -->
<template>
  <view :style="{ paddingTop: `${statusBarHeight}px` }">
    <view
      v-for="(userMenu, i) in userStore.userMobileMenus"
      :key="userMenu.id"
      :index="i"
      style="margin: 15upx; background-color: #fff"
    >
      <uni-section :title="userMenu.title" class="pt-3px">
        <template #decoration>
          <view style="margin-right: 5px">
            <svg-icon
              :icon="userMenu.icon"
              :color="userMenu.color"
              class="h-4 w-4 pt-3px"
            />
          </view>
        </template>
        <template #right>
          <!-- 伪面包屑 -->
          <text
            v-for="(item, index) in allSelData[userMenu.id]"
            :key="index"
            :style="{
              marginLeft: '5px',
              color:
                index === allSelData[userMenu.id].length - 1
                  ? '#8799a3'
                  : '#1890FF',
            }"
            @click="clickText(item, index, userMenu.id)"
          >
            {{
              item.title
                + (index === allSelData[userMenu.id].length - 1
                  ? ''
                  : ' | ')
            }}
          </text>
        </template>
      </uni-section>
      <view>
        <wd-grid :column="4" clickable>
          <wd-grid-item
            v-for="(item, j) in handleData(
              userMenu.id,
              userMenu.children,
            )"
            :key="handleKey(item, j)" custom-class="custom-item"
            :index="j"
            use-icon-slot :text="item.title" icon-size="36px" @itemclick="gridItemClick(userMenu.id, item)"
          >
            <template #icon>
              <simple-icon
                :background-color="item.color"
                :icon="item.icon"
                color="#FFFFFF"
                class="h-5 w-5"
              />
            </template>
          </wd-grid-item>
        </wd-grid>
      </view>
    </view>
  </view>
</template>

<route lang="jsonc" type="page">
{
  "layout": "tabbar",
  "style": {
    "navigationStyle": "custom",
    "navigationBarTitleText": "工作台"
  }
}
</route>

<script setup lang="ts">
import type { MobileUserCenter } from '@/api'
import SimpleIcon from '@/components/simple-icon/index.vue'
import SvgIcon from '@/components/svg-icon/index.vue'
import { useUserStore } from '@/store/modules'
import modal from '@/utils/modal'
import tab from '@/utils/tab'

const statusBarHeight = ref(30) // 默认给个20作为兜底

// 获取系统状态栏高度（建议放 onLoad 或直接 setup 中）
const systemInfo = uni.getSystemInfoSync()
statusBarHeight.value = systemInfo.statusBarHeight! + 30 || 30
const userStore = useUserStore()

const userMobileMenus = userStore.userMobileMenus
console.log(userMobileMenus)
// 当前选中的数据
const selData: { [key: string]: MobileUserCenter.MobileResource[] } = reactive({})
// 所有选中的数据
const allSelData: { [key: string]: MobileUserCenter.MobileResource[] } = reactive({})

if (userMobileMenus && userMobileMenus.length > 0) {
  userMobileMenus.forEach((item) => {
    allSelData[item.id] = []
    allSelData[item.id].push(item)
    selData[item.id] = []
  })
}
function clickText(item: MobileUserCenter.MobileResource, index: number, userMenuId: string | number) {
  if (item && item.children !== undefined && item.children.length > 0) {
    // 菜单进行更新
    handleData(userMenuId, item.children).forEach((itemData) => {
      itemData.key = itemData.key + 1
    })
    selData[userMenuId] = item.children
    // 已选中的部分数据进行删除
    allSelData[userMenuId].splice(
      index + 1,
      allSelData[userMenuId].length - index,
    )
  }
}

function handleData(userMenuId: string | number, userMenuChildren?: MobileUserCenter.MobileResource[]) {
  return selData[userMenuId] && selData[userMenuId].length > 0
    ? selData[userMenuId]
    : userMenuChildren || []
}
function handleKey(item: { key: any }, j: any) {
  item.key = j
  return item.key
}
function gridItemClick(userMenuId: string | number, item: MobileUserCenter.MobileResource) {
  if (item.children && item.children.length > 0) {
    // 菜单进行更新
    item.key = item.key + 1
    selData[userMenuId] = item.children
    // 向已选中的数据中添加新的数据
    allSelData[userMenuId].push(item)
  }
  else if (item.menuType === 'MENU') {
    tab.navigateToObject({
      url: item.path,
      fail(error: any) {
        modal.msg(
          `请将【${
            item.title
          }】的移动端路由地址(${
            item.path
          })与uniapp的page.json的path路径对应！`,
        )
        console.log(error)
      },
    })
  }
  else if (item.menuType === 'IFRAME') {
    tab.navigateTo(`/pages/common/webview/index?url=${item.path}`)
  }
  else if (item.menuType === 'LINK') {
    // #ifdef H5
    globalThis.location.href = item.path
    // #endif
    // #ifndef H5
    tab.navigateTo(`/pages/common/webview/index?url=${item.path}`)
    // #endif
  }
}
</script>

<style lang="scss" scoped>
:deep(.custom-item) {
  .wd-grid-item__text {
    margin-top: 25rpx; // 可根据需要调整间距大小
    text-align: center;
  }
}
</style>
