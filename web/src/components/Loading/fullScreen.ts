import { ElLoading } from "element-plus";

/* 全局请求 loading */
let loadingInstance: ReturnType<typeof ElLoading.service>;

/**
 * @description 开启 Loading
 * */
const startLoading = () => {
  loadingInstance = ElLoading.service({
    fullscreen: true,
    lock: true,
    text: "Loading",
    background: "rgba(0, 0, 0, 0.7)"
  });
};

/**
 * @description 结束 Loading
 * */
const endLoading = () => {
  loadingInstance.close();
};

/**
 * @description 显示全屏加载
 * */
let needLoadingRequestCount = 0;
export const showFullScreenLoading = () => {
  // 如果需要加载请求数量为0，则开始加载动画
  if (needLoadingRequestCount === 0) {
    startLoading();
  }
  // 加载请求数量加1
  needLoadingRequestCount++;
};
/**
 * @description 隐藏全屏加载
 * */
export const tryHideFullScreenLoading = () => {
  // 如果需要加载请求次数小于等于0，则直接返回
  if (needLoadingRequestCount <= 0) return;
  // 将需要加载请求次数减1
  needLoadingRequestCount--;
  // 如果需要加载请求次数等于0，则结束加载
  if (needLoadingRequestCount === 0) {
    endLoading();
  }
};
