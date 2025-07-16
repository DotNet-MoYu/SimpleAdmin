/**
 * tabbar 选择的策略，更详细的介绍见 tabbar.md 文件
 * 0: 'NO_TABBAR' `无 tabbar`
 * 1: 'NATIVE_TABBAR'  `完全原生 tabbar`
 * 2: 'CUSTOM_TABBAR_WITH_CACHE' `有缓存自定义 tabbar`
 * 3: 'CUSTOM_TABBAR_WITHOUT_CACHE' `无缓存自定义 tabbar`
 *
 * 温馨提示：本文件的任何代码更改了之后，都需要重新运行，否则 pages.json 不会更新导致错误
 */
export const TABBAR_MAP = {
  NO_TABBAR: 0,
  NATIVE_TABBAR: 1,
  CUSTOM_TABBAR_WITH_CACHE: 2,
  CUSTOM_TABBAR_WITHOUT_CACHE: 3,
}
// TODO：通过这里切换使用tabbar的策略
export const selectedTabbarStrategy = TABBAR_MAP.NATIVE_TABBAR

// selectedTabbarStrategy==NATIVE_TABBAR(1) 时，需要填 iconPath 和 selectedIconPath
// selectedTabbarStrategy==CUSTOM_TABBAR(2,3) 时，需要填 icon 和 iconType
// selectedTabbarStrategy==NO_TABBAR(0) 时，tabbarList 不生效
export const tabbarList = [
  {
    iconPath: 'static/tabbar/home.png',
    selectedIconPath: 'static/tabbar/home_.png',
    pagePath: 'pages/home/index',
    text: '首页',
  },
  {
    iconPath: 'static/tabbar/work.png',
    selectedIconPath: 'static/tabbar/work_.png',
    pagePath: 'pages/work/index',
    text: '工作台',
  },
  // {
  //   pagePath: 'pages/msg/index',
  //   iconPath: 'static/tabbar/msg.png',
  //   selectedIconPath: 'static/tabbar/msg_.png',
  //   text: '消息',
  // },
  {
    pagePath: 'pages/mine/index',
    iconPath: 'static/tabbar/mine.png',
    selectedIconPath: 'static/tabbar/mine_.png',
    text: '我的',
  },
]

// NATIVE_TABBAR(1) 和 CUSTOM_TABBAR_WITH_CACHE(2) 时，需要tabbar缓存
export const cacheTabbarEnable = selectedTabbarStrategy === TABBAR_MAP.NATIVE_TABBAR
  || selectedTabbarStrategy === TABBAR_MAP.CUSTOM_TABBAR_WITH_CACHE

const _tabbar = {
  color: '#999999',
  selectedColor: '#018d71',
  backgroundColor: '#F8F8F8',
  borderStyle: 'black',
  height: '50px',
  fontSize: '10px',
  iconWidth: '24px',
  spacing: '3px',
  list: tabbarList,
}

// 0和1 需要显示底部的tabbar的各种配置，以利用缓存
export const tabBar = cacheTabbarEnable ? _tabbar : undefined
