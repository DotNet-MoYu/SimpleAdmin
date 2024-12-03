<template>
  <div class="message">
    <el-tooltip content="消息通知" placement="bottom" effect="light">
      <div>
        <el-popover placement="bottom" :width="310" trigger="hover">
          <template #reference>
            <div>
              <el-badge v-if="unRead > 0" :value="unRead" class="item">
                <i :class="'iconfont icon-xiaoxi'" class="toolBar-icon"></i>
              </el-badge>
              <el-badge v-else class="item">
                <i :class="'iconfont icon-xiaoxi'" class="toolBar-icon"></i>
              </el-badge>
            </div>
          </template>
          <el-tabs v-model="activeName">
            <el-tab-pane
              v-for="(tab, index) in messageTypeOptions"
              :key="index"
              :label="tab.label + `(${messageStore.getUnReadCount(tab.value)})`"
              :name="tab.value"
            >
              <div class="message-list">
                <div class="message-item" v-for="(msg, index2) in getNewUnRead(tab.value)" :key="index2">
                  <img v-if="tab.value === MessageTypeDictEnum.INFORM" src="@/assets/images/msg01.png" alt="" class="message-icon" />
                  <img v-else-if="tab.value === MessageTypeDictEnum.NOTICE" src="@/assets/images/msg03.png" alt="" class="message-icon" />
                  <img v-else src="@/assets/images/msg02.png" alt="" class="message-icon" />
                  <div class="message-content">
                    <!-- 如果未读class用message-title-black -->
                    <span :class="{ 'message-title': msg.read, 'message-title-black': !msg.read }">{{ msg.subject }}</span>
                    <span class="message-date">{{ msg.sendTimeFormat }}</span>
                  </div>
                </div>
                <!-- div居中 -->
                <div class="flex-center pt-2">
                  <el-link type="primary" :underline="false" @click="toMessage">去处理</el-link>
                </div>
              </div>
            </el-tab-pane>
          </el-tabs>
        </el-popover>
      </div>
    </el-tooltip>
  </div>
</template>

<script setup lang="ts">
import { useDictStore, useMessageStore, useCenterStore } from "@/stores/modules";
import { SysDictEnum, MessageTypeDictEnum } from "@/enums";
import { SysMessage } from "@/api";
import { useRouter } from "vue-router";
import { USER_CENTER_URL } from "@/config";

const router = useRouter();
const centerStore = useCenterStore();
const dictStore = useDictStore();
const messageStore = useMessageStore();
const unRead = ref<number>(0); //未读数量
const newUnRead = ref<SysMessage.SysMessageInfo[]>([]);

// 消息类型选项
const messageTypeOptions = dictStore.getDictList(SysDictEnum.MESSAGE_CATEGORY);
const activeName = ref(messageTypeOptions[0].value);

onMounted(async () => {
  await getUnRead();
  console.log(messageStore.unReadInfoGet);
});

/** 监听未读消息数量变化 */
messageStore.$subscribe((mutations, state) => {
  unRead.value = state.unReadCount > 99 ? 99 : state.unReadCount;
  newUnRead.value = state.newUnRead;
});

/** 获取未读信息 */
async function getUnRead() {
  await messageStore.getUnReadInfo();
}

/** 获取当前消息类型的未读消息 */
function getNewUnRead(category: string) {
  return newUnRead.value.filter(item => item.category === category);
}

/** 显示更多 */
function toMessage() {
  centerStore.setMessage(); //显示更多
  //跳转到个人中心的消息页
  router.replace(USER_CENTER_URL);
}
</script>

<style scoped lang="scss">
.el-link {
  margin-right: 8px;
}
.message-empty {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 260px;
  line-height: 45px;
}
.message-list {
  display: flex;
  flex-direction: column;
  .message-item {
    display: flex;
    align-items: center;
    padding: 15px 0;
    border-bottom: 1px solid var(--el-border-color-light);
    &:last-child {
      border: none;
    }
    .message-icon {
      width: 40px;
      height: 40px;
      margin: 0 20px 0 5px;
    }
    .message-content {
      display: flex;
      flex-direction: column;
      .message-title {
        margin-bottom: 2px;
      }
      .message-title-black {
        margin-bottom: 2px;
        font-weight: bold;
      }
      .message-date {
        font-size: 12px;
        color: var(--el-text-color-secondary);
      }
    }
  }
}
</style>
