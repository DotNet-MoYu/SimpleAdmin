<template>
  <div class="home card">
    <!-- 如需恢复原欢迎图首页，取消下一行注释，并可按需注释广告面板 -->
    <!-- <img class="home-bg" src="@/assets/images/welcome.png" alt="welcome" /> -->
    <div class="ad-section">
      <template v-for="(item, index) in adCards" :key="item.title">
        <a v-if="item.action !== 'package'" :class="['ad-card', `ad-card--${index + 1}`]" :href="item.link" target="_blank" rel="noopener noreferrer">
          <div class="ad-meta">
            <span class="ad-tag">{{ item.tag }}</span>
            <span class="ad-note">{{ item.note }}</span>
          </div>
          <div class="ad-title-row">
            <span class="ad-icon">{{ item.icon }}</span>
            <span class="ad-title">{{ item.title }}</span>
          </div>
          <div class="ad-desc">{{ item.desc }}</div>
          <div class="ad-price">{{ item.price }}</div>
          <ul class="ad-points">
            <li v-for="point in item.points" :key="point">{{ point }}</li>
          </ul>
          <div class="ad-link">
            <span>{{ item.cta }}</span>
            <span class="ad-arrow">→</span>
          </div>
        </a>
        <div
          v-else
          :class="['ad-card', `ad-card--${index + 1}`]"
          role="button"
          tabindex="0"
          @click="openPackageDialog"
          @keydown.enter.prevent="openPackageDialog"
          @keydown.space.prevent="openPackageDialog"
        >
          <div class="ad-meta">
            <span class="ad-tag">{{ item.tag }}</span>
            <span class="ad-note">{{ item.note }}</span>
          </div>
          <div class="ad-title-row">
            <span class="ad-icon">{{ item.icon }}</span>
            <span class="ad-title">{{ item.title }}</span>
          </div>
          <div class="ad-desc">{{ item.desc }}</div>
          <div class="ad-price">{{ item.price }}</div>
          <ul class="ad-points">
            <li v-for="point in item.points" :key="point">{{ point }}</li>
          </ul>
          <div class="ad-link">
            <span>{{ item.cta }}</span>
            <span class="ad-arrow">→</span>
          </div>
        </div>
      </template>
    </div>
    <el-dialog v-model="packageVisible" title="AI 套餐信息" width="720px" append-to-body class="package-dialog">
      <el-tabs v-model="activePackageTab" stretch class="package-tabs">
        <el-tab-pane v-for="group in packageGroups" :key="group.name" :label="group.label" :name="group.name">
          <div class="package-table-wrap">
            <table class="package-table">
              <thead>
                <tr>
                  <th>价格</th>
                  <th>额度</th>
                  <th>天数</th>
                  <th>说明</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="plan in group.plans" :key="`${group.name}-${plan.price}`">
                  <td class="package-col-price">{{ plan.price }}</td>
                  <td>{{ plan.quota }}</td>
                  <td>{{ plan.days }}</td>
                  <td>{{ plan.detail }}</td>
                </tr>
              </tbody>
            </table>
          </div>
        </el-tab-pane>
      </el-tabs>
      <template #footer>
        <div class="package-footer">
          <span class="package-footer__tip">如需开通或咨询，请咨询群主。</span>
          <el-button type="primary" @click="packageVisible = false">知道了</el-button>
        </div>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts" name="home">
import { ref } from "vue";

type AdCard = {
  icon: string;
  tag: string;
  note: string;
  title: string;
  desc: string;
  price: string;
  points: string[];
  cta: string;
  action?: "package";
  link?: string;
};

type PackagePlan = {
  price: string;
  quota: string;
  days: string;
  detail: string;
};

type PackageGroup = {
  name: "metered" | "daily";
  label: string;
  plans: PackagePlan[];
};

const packageVisible = ref(false);
const activePackageTab = ref<PackageGroup["name"]>("metered");

const openPackageDialog = () => {
  activePackageTab.value = "metered";
  packageVisible.value = true;
};

const packageGroups: PackageGroup[] = [
  {
    name: "metered",
    label: "按量付费",
    plans: [
      { price: "20元", quota: "600 美刀额度", days: "90天", detail: "入门试用" },
      { price: "45元", quota: "1500 美元额度", days: "90天", detail: "按量套餐" },
      { price: "88元", quota: "3500 美元额度", days: "90天", detail: "按量套餐" }
    ]
  },
  {
    name: "daily",
    label: "按天付费",
    plans: [
      { price: "10元", quota: "无限流量", days: "1天", detail: "按天套餐" },
      { price: "28元", quota: "45美刀/日", days: "30天", detail: "按天套餐" },
      { price: "38元", quota: "90美刀/日", days: "30天", detail: "按天套餐" },
      { price: "58元", quota: "135美刀/日", days: "30天", detail: "按天套餐" },
      { price: "78元", quota: "200美刀/日", days: "30天", detail: "按天套餐" }
    ]
  }
];

const adCards: AdCard[] = [
  {
    icon: "A",
    tag: "网络加速",
    note: "邀请推荐",
    title: "性价比机场推荐",
    desc: "面向日常访问和开发场景的稳定线路。",
    price: "12 元 / 月 · 128G",
    points: ["月付门槛低，适合轻量到中度使用。", "纯净IP，畅用AI。", "覆盖常见访问场景，上手成本低。"],
    cta: "查看方案",
    link: "https://www.zou666.net/#/register?code=hfkhGLG5"
  },
  {
    icon: "AI",
    tag: "AI 工具",
    note: "邀请推荐",
    title: "性价比AI中转站推荐",
    desc: "无需梯子，快速接入常用模型调用能力。",
    price: "支持 Codex 中转 · 价格低到爆",
    points: ["无需梯子即可访问，接入门槛更低。", "低门槛接入，开箱即用。", "价格低到爆，适合长期使用。"],
    cta: "查看套餐",
    action: "package"
  },
  {
    icon: "C",
    tag: "云服务器",
    note: "邀请推荐",
    title: "腾讯云轻量服务器推荐",
    desc: "适合部署后台管理系统、Web 服务和轻量业务应用。",
    price: "低门槛上云 · 适合个人与小团队",
    points: ["适合部署 .NET API 与前端静态资源。", "配置简单，适合快速上线项目。", "对外包、私活和商用项目都比较友好。"],
    cta: "查看方案",
    link: "https://curl.qcloud.com/Uw9jZRJr"
  },
  {
    icon: "T",
    tag: "云产品",
    note: "邀请推荐",
    title: "腾讯云云产品优惠推荐",
    desc: "云服务器、云数据库、COS、CDN、短信等云产品特惠热卖中。",
    price: "项目配套常用云资源 · 一站式选购",
    points: ["适合项目上线后的存储、加速与消息能力补齐。", "云服务器、数据库、对象存储等资源覆盖更全。", "适合商用项目、外包交付和长期运维场景。"],
    cta: "查看优惠",
    link: "https://curl.qcloud.com/Im4ILEeM"
  }
];
</script>

<style scoped lang="scss">
@use "./index";
</style>
